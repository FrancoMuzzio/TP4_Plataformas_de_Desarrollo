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
    public class TarjetasController : Controller
    {
        private readonly MiContexto _context;

        public TarjetasController(MiContexto context)
        {
            _context = context;
        }

        // GET: Tarjetas
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.tarjetas.Include(t => t.titular);
            return View(await miContexto.ToListAsync());
        }

        // GET: Tarjetas/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Tarjetas/Create
        public IActionResult Create()
        {
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: Tarjetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,id_titular,numero,codigoV,limite,consumo")] Tarjeta tarjeta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // GET: Tarjetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta == null)
            {
                return NotFound();
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // POST: Tarjetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,id_titular,numero,codigoV,limite,consumo")] Tarjeta tarjeta)
        {
            if (id != tarjeta.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjeta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetaExists(tarjeta.id))
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
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", tarjeta.id_titular);
            return View(tarjeta);
        }

        // GET: Tarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Tarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tarjetas == null)
            {
                return Problem("Entity set 'MiContexto.tarjetas'  is null.");
            }
            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                _context.tarjetas.Remove(tarjeta);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetaExists(int id)
        {
          return _context.tarjetas.Any(e => e.id == id);
        }
    }
}
