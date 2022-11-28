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
    public class MovimientosController : Controller
    {
        private readonly MiContexto _context;

        public MovimientosController(MiContexto context)
        {
            _context = context;
        }

        // GET: Movimientos
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.movimientos.Include(m => m.caja);
            return View(await miContexto.ToListAsync());
        }

        // GET: Movimientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos
                .Include(m => m.caja)
                .FirstOrDefaultAsync(m => m.id == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // GET: Movimientos/Create
        public IActionResult Create()
        {
            ViewData["id_Caja"] = new SelectList(_context.cajas, "id", "id");
            return View();
        }

        // POST: Movimientos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,detalle,monto,fecha,id_Caja")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_Caja"] = new SelectList(_context.cajas, "id", "id", movimiento.id_Caja);
            return View(movimiento);
        }

        // GET: Movimientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            ViewData["id_Caja"] = new SelectList(_context.cajas, "id", "id", movimiento.id_Caja);
            return View(movimiento);
        }

        // POST: Movimientos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,detalle,monto,fecha,id_Caja")] Movimiento movimiento)
        {
            if (id != movimiento.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimientoExists(movimiento.id))
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
            ViewData["id_Caja"] = new SelectList(_context.cajas, "id", "id", movimiento.id_Caja);
            return View(movimiento);
        }

        // GET: Movimientos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos
                .Include(m => m.caja)
                .FirstOrDefaultAsync(m => m.id == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: Movimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.movimientos == null)
            {
                return Problem("Entity set 'MiContexto.movimientos'  is null.");
            }
            var movimiento = await _context.movimientos.FindAsync(id);
            if (movimiento != null)
            {
                _context.movimientos.Remove(movimiento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimientoExists(int id)
        {
          return _context.movimientos.Any(e => e.id == id);
        }
    }
}
