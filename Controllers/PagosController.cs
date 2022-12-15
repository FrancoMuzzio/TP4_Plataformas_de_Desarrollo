using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_plataformas_de_desarrollo.Data;
using WebApplication_plataformas_de_desarrollo.Models;

namespace WebApplication_plataformas_de_desarrollo.Controllers
{
    public class PagosController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public PagosController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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

        // GET: Pagos
        public async Task<IActionResult> Index(string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (usuarioLogueado.isAdmin)
            {
                return View(await _context.pagos.OrderByDescending(p => p.id).ToListAsync());
            }
            else
            {
                return View(await _context.pagos.Where(p => p.usuario == usuarioLogueado).OrderByDescending(p=>p.id).ToListAsync());
            }
        }

        // GET: Pagos/Create
        public IActionResult Create(string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["id_usuario"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre, float monto)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Pago nuevoPago = new Pago(usuarioLogueado, nombre, monto);
                _context.pagos.Add(nuevoPago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Error.");
            }
        }

        // GET: Pagos/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Pago? pago = await _context.pagos.FindAsync(id);
            if(pago == null)
            {
                return RedirectToAction("Index", "Pagos", new { mensaje = "Pago no encontrado." });
            }
            _context.pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Pagos");
        }

        // GET: Pagos/PagarConTarjeta/5
        public async Task<IActionResult> PagarConTarjeta(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewBag.Tarjetas = new SelectList(usuarioLogueado.tarjetas.Select(t => new
            {
                numero = t.numero,
                text_to_show = "Numero: " + t.numero + " - Dinero disponible: " + (t.limite-t.consumo)
            }), "numero", "text_to_show");
            return View(pago);
        }
        // GET: Pagos/PagarConCaja/5
        public async Task<IActionResult> PagarConCaja(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewBag.Cajas = new SelectList(usuarioLogueado.cajas.Select(c => new
            {
                numero = c.cbu,
                text_to_show = "CBU: " + c.cbu + " - Saldo: " + c.saldo
            }), "numero", "text_to_show");
            return View(pago);
        }

        // POST: Pagos/Pagar/5
        [HttpPost, ActionName("Pagar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(int id, int numero, string from)
        {
            Pago? pago = await _context.pagos.FindAsync(id);
            if (pago == null)
            {
                return RedirectToAction("PagarConCaja", "Pagos", new { id = id, mensaje = "No se encuentra el pago." });
            }
            if (from == "PagarConCaja") { 
                if(numero == 0) {
                    return RedirectToAction("PagarConCaja", "Pagos", new { id = id, mensaje = "Seleccione una caja de ahorros." });
                }
                CajaDeAhorro? caja = await _context.cajas.Where(c=>c.cbu == numero).FirstOrDefaultAsync();
                if (pago.pagado)
                {
                    return RedirectToAction("PagarConCaja", "Pagos", new { id = id, mensaje = "El pago ya fue pagado anteriormente." });
                }
                if (caja != null)
                {
                    if (caja.saldo < pago.monto)
                    {
                        return RedirectToAction("PagarConCaja", "Pagos", new { id = id, mensaje = "La caja tiene saldo insuficiente." });
                    }
                    caja.saldo -= pago.monto;
                    pago.pagado = true;
                    Movimiento movimientoNuevo = new Movimiento(caja, "Pago de Tarjeta " + pago.nombre, pago.monto);
                    _context.movimientos.Add(movimientoNuevo);
                    caja.movimientos.Add(movimientoNuevo);
                    pago.metodo = "Caja de ahorro";
                    _context.Update(caja);
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                } 
            }
            else
            {
                if (numero == 0)
                {
                    return RedirectToAction("PagarConTarjeta", "Pagos", new { id = id, mensaje = "Seleccione una tarjeta." });
                }
                Tarjeta? tarjeta = await _context.tarjetas.Where(t => t.numero == numero).FirstOrDefaultAsync();
                if (tarjeta == null)
                {
                    return RedirectToAction("PagarConTarjeta", "Pagos", new { id = id, mensaje = "No se encuentra la tarjeta." });
                }
                if ((tarjeta.limite - tarjeta.consumo) < pago.monto)
                {
                    return RedirectToAction("PagarConTarjeta", "Pagos", new { id = id, mensaje = "Esta compra superaria el limite de la tarjeta." });
                }
                tarjeta.consumo += pago.monto;
                pago.pagado = true;
                pago.metodo = "Tarjeta";
                _context.Update(tarjeta);
                _context.Update(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return Problem();
            
        }

    }
}
