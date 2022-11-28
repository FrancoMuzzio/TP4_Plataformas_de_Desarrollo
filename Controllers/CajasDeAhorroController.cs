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
    public class CajasDeAhorroController : Controller
    {
        private readonly MiContexto _context;

        public CajasDeAhorroController(MiContexto context)
        {
            _context = context;
        }

        // GET: CajasDeAhorro
        public async Task<IActionResult> Index()
        {
              return View(await _context.cajas.ToListAsync());
        }

        // GET: CajasDeAhorro/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: CajasDeAhorro/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CajasDeAhorro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,cbu,saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cajaDeAhorro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cajaDeAhorro);
        }

        // GET: CajasDeAhorro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            return View(cajaDeAhorro);
        }

        // POST: CajasDeAhorro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,cbu,saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (id != cajaDeAhorro.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cajaDeAhorro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaDeAhorroExists(cajaDeAhorro.id))
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
            return View(cajaDeAhorro);
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
            if (_context.cajas == null)
            {
                return Problem("Entity set 'MiContexto.cajas'  is null.");
            }
            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro != null)
            {
                _context.cajas.Remove(cajaDeAhorro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajaDeAhorroExists(int id)
        {
          return _context.cajas.Any(e => e.id == id);
        }
    }
}
