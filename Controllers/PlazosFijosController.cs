using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (usuarioLogueado.isAdmin)
            {
                return View(await _context.plazosFijos.OrderBy(pf => pf.pagado).ToListAsync());
            }
            else
            {
                return View(await _context.plazosFijos.Where(c => c.titular == usuarioLogueado).OrderBy(pf => pf.pagado).ToListAsync());
            }
        }

        // GET: PlazosFijos/Create
        public IActionResult Create(string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cajas = new SelectList(usuarioLogueado.cajas.Select(c=> new
            {
                cbu = c.cbu,
                text_to_show = "CBU: "+c.cbu+" - Saldo: "+c.saldo
            }), "cbu", "text_to_show");
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
                Debug.WriteLine("MONTO: " + monto);
                Debug.WriteLine("CBU: "+ cbu);
                if (monto < 1000)
                {
                    return RedirectToAction("Create", "PlazosFijos", new { mensaje = "Monto insuficiente para crear el plazo fijo." });
                }
                CajaDeAhorro? caja = _context.cajas.Where(caja => caja.cbu == cbu).FirstOrDefault();
                if (caja == null)
                {
                    return RedirectToAction("Create", "PlazosFijos", new { mensaje = "No se encontró la caja." });
                }
                if (caja.saldo < monto)
                {
                    return RedirectToAction("Create", "PlazosFijos", new { mensaje = "Fondos insuficientes." });
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
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            PlazoFijo? pf = await _context.plazosFijos.FindAsync(id);
            if (pf == null)
            {
                return RedirectToAction("Index", "PlazosFijos", new { mensaje = "Plazo fijo no encontrado." });
            }
            _context.plazosFijos.Remove(pf);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "PlazosFijos");
        }

        public async Task<IActionResult> Pagar()
        {

            foreach (PlazoFijo pFijo in usuarioLogueado.pf.Where(p=>p.fechaFin < DateTime.Today && !p.pagado))
            {
                DateTime fechaIni = pFijo.fechaIni;
                DateTime fechaFin = pFijo.fechaFin;
                if (DateTime.Now.CompareTo(fechaFin) >= 0 && pFijo.pagado == false) //Esto no se si va a alreves
                {
                    double cantDias = (fechaFin - fechaIni).TotalDays;
                    float montoFinal = (pFijo.monto + pFijo.monto * (float)(90.0 / 365.0) * (float)cantDias);
                    decimal bar = Convert.ToDecimal(montoFinal);
                    montoFinal = (float)Math.Round(bar, 2);//redondeo a 2 decimales
                    CajaDeAhorro caja = _context.cajas.Where(c => c.cbu == pFijo.cbu).FirstOrDefault();
                    pFijo.pagado = true;
                    caja.saldo += montoFinal;
                    Movimiento movimientoNuevo = new Movimiento(caja, "Pago plazo fijo", montoFinal);
                    _context.movimientos.Add(movimientoNuevo);
                    caja.movimientos.Add(movimientoNuevo);
                    _context.Update(pFijo);
                    _context.Update(caja);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
