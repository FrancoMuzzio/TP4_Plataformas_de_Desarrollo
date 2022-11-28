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
    public class PlazosFijosController : Controller
    {
        private readonly MiContexto _context;

        public PlazosFijosController(MiContexto context)
        {
            _context = context;
        }

        // GET: PlazosFijos
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.plazosFijos.Include(p => p.titular);
            return View(await miContexto.ToListAsync());
        }

        // GET: PlazosFijos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: PlazosFijos/Create
        public IActionResult Create()
        {
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido");
            return View();
        }

        // POST: PlazosFijos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,monto,fechaIni,fechaFin,tasa,pagado,id_titular,cbu")] PlazoFijo plazoFijo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plazoFijo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // GET: PlazosFijos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // POST: PlazosFijos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,monto,fechaIni,fechaFin,tasa,pagado,id_titular,cbu")] PlazoFijo plazoFijo)
        {
            if (id != plazoFijo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plazoFijo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlazoFijoExists(plazoFijo.id))
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
            ViewData["id_titular"] = new SelectList(_context.usuarios, "id", "apellido", plazoFijo.id_titular);
            return View(plazoFijo);
        }

        // GET: PlazosFijos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        private bool PlazoFijoExists(int id)
        {
          return _context.plazosFijos.Any(e => e.id == id);
        }
    }
}
