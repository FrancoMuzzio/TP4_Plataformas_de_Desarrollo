using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_plataformas_de_desarrollo.Data;
using WebApplication_plataformas_de_desarrollo.Models;

namespace WebApplication_plataformas_de_desarrollo.Controllers
{
    public class PlazosFijosController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public PlazosFijosController(MiContexto context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _context.usuarios
                    .Include(u => u.tarjetas)
                    .Include(u => u.cajas)
                    .Include(u => u.pf)
                    .Include(u => u.pagos)
                    .Load();
            _context.cajas
                .Include(c => c.movimientos)
                .Include(c => c.titulares)
                .Load();
            _context.tarjetas.Load();
            _context.pagos.Load();
            _context.movimientos.Load();
            _context.plazosFijos.Load();
            usuarioLogueado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("IdUsuario")).FirstOrDefault();
        }

        // GET: PlazosFijos
        public async Task<IActionResult> Index()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var miContexto = _context.plazosFijos.Include(p => p.titular);
            return View(await miContexto.ToListAsync());
        }

        // GET: PlazosFijos/Create
        public IActionResult Create()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cajas = new SelectList(usuarioLogueado.cajas);
            return View();
        }

        // POST: PlazosFijos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int monto, int cbu)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Create", "Home");
            }
            try
            {
                if (monto < 1000)
                {
                    return RedirectToAction("Depositar", "PlazosFijos", new { mensaje = "Monto insuficiente para crear el pf." });
                }
                CajaDeAhorro? caja = _context.cajas.Where(caja => caja.cbu == cbu).FirstOrDefault();
                if (caja == null)
                {
                    return RedirectToAction("Depositar", "PlazosFijos", new { mensaje = "No se encontró la caja." });
                }
                if (caja.saldo < monto)
                {
                    return RedirectToAction("Depositar", "PlazosFijos", new { mensaje = "Fondos insuficientes." });
                }
                caja.saldo -= monto;
                Movimiento movimientoNuevo = new Movimiento(caja, "Alta plazo fijo", monto);
                _context.movimientos.Add(movimientoNuevo);
                caja.movimientos.Add(movimientoNuevo);
                _context.Update(caja);
                PlazoFijo nuevoPlazoFijo = new PlazoFijo(usuarioLogueado, monto, DateTime.Now.AddMonths(1), 90, caja.cbu);
                _context.plazosFijos.Add(nuevoPlazoFijo);
                _context.Update(caja);
                _context.Update(usuarioLogueado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Error.");
            }
        }

        // GET: PlazosFijos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p.titular)
                .FirstOrDefaultAsync(m => m.id == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // POST: PlazosFijos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.plazosFijos == null)
            {
                return Problem("Entity set 'MiContexto.plazosFijos'  is null.");
            }
            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo != null)
            {
                _context.plazosFijos.Remove(plazoFijo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
