using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_plataformas_de_desarrollo.Data;
using WebApplication_plataformas_de_desarrollo.Models;

namespace WebApplication_plataformas_de_desarrollo.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly MiContexto _context;
        private Usuario usuarioLogueado;
        public UsuariosController(MiContexto context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
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

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (usuarioLogueado.isAdmin)
            {
                return View(await _context.usuarios.ToListAsync());
            }
            else
            {
                return View(await _context.pagos.Where(p => p.usuario == usuarioLogueado).OrderByDescending(u => u.id).ToListAsync());
            }
           }


        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Nombre, string Apellido, int Dni, string Mail, string Password)
        {
            bool esValido = false;
            Usuario us = _context.usuarios.Where(u => u.dni == Dni || u.mail == Mail).FirstOrDefault();
            if (us.isAdmin == true)
            {
                try
                {
                    Usuario nuevo = new Usuario(Dni, Nombre, Apellido, Mail, Password, 0, false, false);
                    esValido = true;
                    _context.usuarios.Add(nuevo);
                    _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return RedirectToAction("Create", "Usuarios", new { mensaje = "El usuario no ha podido crearse" });
                    
                }
            }
            else
            {
                return Problem();
            }
        }




            // GET: Usuarios/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, int dni, string mail, string pass)
        {
            try
            {
                Usuario usuarioAModificar = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
                if (usuarioAModificar != null)
                {
                    usuarioAModificar.mail = mail;
                    usuarioAModificar.password = pass;
                    _context.Update(usuarioAModificar);
                    _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction("Edit", "Usuarios", new { mensaje = "No se encontró a este usuario." });

            }
            catch (Exception ex)
            {
                return Problem("Error.");
            }
        }
       

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id, string mensaje="")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                Usuario usuarioARemover = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
                if (usuarioARemover != null)
                {
                    _context.usuarios.Remove(usuarioARemover);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index)); //si?
                }
                else
                {
                    return RedirectToAction("Delete", "Usuarios", new { mensaje = "No se encontró a este usuario." });

                }
            }
            catch
            {
                return Problem("Error.");
            }
        }

       
    }
}
