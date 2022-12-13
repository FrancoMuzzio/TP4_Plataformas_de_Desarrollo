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
    public class CajasDeAhorroController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public CajasDeAhorroController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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
 
        // GET: CajasDeAhorro/AgregarTitular/5
        public async Task<IActionResult> AgregarTitular(int? id)
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


        // POST: CajasDeAhorro/AgregarTitular/5
        [HttpPost, ActionName("AgregarTitular")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarTitular(int id, int dni)
        {
            try
            {
                CajaDeAhorro? caja = await _context.cajas.FindAsync(id);
                Usuario? userAdd = _context.usuarios.Where(usuario => usuario.dni == dni).FirstOrDefault();
                if (userAdd == null)
                {
                    return Problem("No se encontró usuario con este DNI en la lista de Usuarios del Banco");                //No se encontró usuario con este DNI en la lista de Usuarios del Banco
                }
                if (caja == null)
                {
                    return Problem("No se encontró la caja de ahorro con ese ID");                   //No se encontró la caja de ahorro con ese ID
                }

                if (caja.titulares.Contains(userAdd))
                {
                    return Problem("El usuario ya posee esta caja de ahorro en el sistema.");                      //El usuario ya posee esta caja de ahorro en el sistema.
                }
                caja.titulares.Add(userAdd);
                userAdd.cajas.Add(caja);
                _context.Update(caja);
                _context.Update(userAdd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); ;
            }
            catch
            {
                return Problem("Ocurrio un error.");
            }

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
