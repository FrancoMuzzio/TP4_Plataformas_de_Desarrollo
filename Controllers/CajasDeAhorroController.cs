using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_plataformas_de_desarrollo.Data;
using WebApplication_plataformas_de_desarrollo.Models;

namespace WebApplication_plataformas_de_desarrollo.Controllers
{
    public class CajasDeAhorroController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public CajasDeAhorroController(MiContexto context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            Debug.WriteLine(httpContextAccessor.HttpContext.Session.GetInt32("IdUsuario"));
            usuarioLogueado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("IdUsuario")).FirstOrDefault();
        }

        // GET: CajasDeAhorro
        public async Task<IActionResult> Index()
        {
              return View(await _context.cajas.ToListAsync());
        }

        // GET: CajasDeAhorro/Create
        public IActionResult Create()
        {
            Random random = new Random();
            int nuevoCbu = random.Next(100000000, 999999999);
            while (_context.cajas.Any(caja => caja.cbu == nuevoCbu))
            {  // Mientras haya alguna caja con ese CBU se crea otro CBU
                nuevoCbu = random.Next(100000000, 999999999);
                Debug.WriteLine("El CBU generado ya existe, creado uno nuevo...");
            }
            //Ahora sí lo agrego en la lista
            CajaDeAhorro nuevo = new CajaDeAhorro(nuevoCbu, usuarioLogueado);
            _context.cajas.Add(nuevo);
            _context.Update(usuarioLogueado);
            _context.SaveChanges();
            return RedirectToAction("Index", "CajasDeAhorro");
        }
 
        // GET: CajasDeAhorro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(cajaDeAhorro);
        }

        // POST: CajasDeAhorro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CajaDeAhorro? cajaARemover = await _context.cajas.FindAsync(id);
                if (cajaARemover == null)
                {
                    return Problem("No existe caja.");
                }
                if (cajaARemover.saldo != 0)
                {
                    return Problem("Tiene saldo, no se puede eliminar.");
                }
                foreach (Usuario titular in cajaARemover.titulares) 
                {
                    titular.cajas.Remove(cajaARemover);  //Saco la caja de ahorro de los titulares.
                }
                _context.cajas.Remove(cajaARemover); //Saco la caja de ahorro del banco

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Error.");
            }
            
        }
    }
}
