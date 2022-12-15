﻿using System;
using System.Collections.Generic;
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
    public class UsuariosController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public UsuariosController(MiContexto context, IHttpContextAccessor httpContextAccessor)
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

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(await _context.usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int dni, string nombre, string apellido, string email, string password, string password2)
        {
            try
            {
                if(_context.usuarios.Where(u => u.dni == dni).FirstOrDefault()!=null)
                {
                    return (usuarioLogueado != null && usuarioLogueado.isAdmin) ? RedirectToAction(nameof(Index), new { mensaje = "Ya existe un usuario con ese DNI." }) : RedirectToAction("Registrarse", "Home", new { mensaje = "Ya existe un usuario con ese DNI." });
                }
                if(_context.usuarios.Where(u => u.mail == email).FirstOrDefault()!=null)
                {
                    return (usuarioLogueado != null && usuarioLogueado.isAdmin) ? RedirectToAction(nameof(Index), new { mensaje = "Ya existe un usuario con ese email." }) : RedirectToAction("Registrarse", "Home", new { mensaje = "Ya existe un usuario con ese email." });
                }
                if(password!=password2)
                {
                    return (usuarioLogueado != null && usuarioLogueado.isAdmin) ? RedirectToAction(nameof(Index), new { mensaje = "Las contraseñas no coinciden." }) : RedirectToAction("Registrarse", "Home", new { mensaje = "Las contraseñas no coinciden." });
                }
                Usuario nuevo = new Usuario(dni, nombre, apellido, email, password, 0, false, false);
                _context.usuarios.Add(nuevo);
                await _context.SaveChangesAsync();
                return (usuarioLogueado != null && usuarioLogueado.isAdmin) ? RedirectToAction(nameof(Index)) : RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction(nameof(Index), new { mensaje = "Error al crear el usuario." });
            }
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("id,dni,nombre,apellido,mail,intentosFallidos,bloqueado,password,isAdmin")] Usuario usuario)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MiContexto.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.usuarios.Any(e => e.id == id);
        }

    }
}
