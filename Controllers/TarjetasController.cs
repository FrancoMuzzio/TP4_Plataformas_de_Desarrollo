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
    public class TarjetasController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public TarjetasController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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

        // GET: Tarjetas
        public async Task<IActionResult> Index()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (usuarioLogueado.isAdmin)
            {
                return View(await _context.tarjetas.ToListAsync());
            }
            else
            {
                return View(await _context.tarjetas.Where(c => c.titular==usuarioLogueado).ToListAsync());
            }
        }

       //get
        public IActionResult Create()
        {

            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Random random = new Random();
                int nuevoNumero = random.Next(100000000, 999999999);
                while (_context.tarjetas.Any(tarjeta => tarjeta.numero == nuevoNumero))
                {  // Mientras haya alguna tarjeta con ese numero se crea otro numero
                    nuevoNumero = random.Next(100000000, 999999999);
                    Debug.WriteLine("El número de tarjeta generado ya existe, creado uno nuevo...");
                }
                int nuevoCodigo = random.Next(100, 999); //Creo un codigo de tarjeta aleatorio
                Tarjeta nuevo = new Tarjeta(usuarioLogueado.id, nuevoNumero, nuevoCodigo, 20000, 0);
                nuevo.titular = usuarioLogueado;
                _context.tarjetas.Add(nuevo);
                _context.Update(usuarioLogueado);
                _context.SaveChanges();
                return RedirectToAction("Index", "Tarjetas");
             //   return Problem;

            }
            catch
            {
                return Problem();
            }
        }


        // GET: Tarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            {
                if (id == null || _context.tarjetas == null)
                {
                    return NotFound();
                }

                var tarjeta = await _context.tarjetas
                    .Include(t => t.titular)
                    .FirstOrDefaultAsync(m => m.id == id);
                if (tarjeta == null)
                {
                    return NotFound();
                }

                return View(tarjeta);
            }
        }

        // POST: Tarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                Tarjeta? tarjetaARemover = await _context.tarjetas.FindAsync(id);
                if (tarjetaARemover == null)
                {
                    return RedirectToAction("Delete", "Tarjetas", new { mensaje = "No existe la tarjeta." });

                }
                if (tarjetaARemover.consumo != 0) // La condición para eliminar es que no tenga consumos sin pagar.
                {
                    return RedirectToAction("Delete", "Tarjetas", new { mensaje = "Para eliminar no debe tener consumos sin pagar" });

                }
                _context.tarjetas.Remove(tarjetaARemover); //Borro la tarjeta de la lista de tarjetas del Banco
                tarjetaARemover.titular.tarjetas.Remove(tarjetaARemover);//Borro la tarjeta de la lista de tarjetas del usuario.
                _context.Update(tarjetaARemover.titular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                // return true;
            }
            catch
            {
                return Problem("Error.");
            }


        }

      
    }
}
