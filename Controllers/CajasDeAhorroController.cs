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
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (usuarioLogueado.isAdmin)
            {
                return View(await _context.cajas.ToListAsync());
            }
            else
            {
                return View(await _context.cajas.Where(c => c.titulares.Contains(usuarioLogueado)).ToListAsync());
            }
        }

        // GET: CajasDeAhorro/Create
        public IActionResult Create()
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
 
        // GET: CajasDeAhorro/EliminarTitular/5
        public async Task<IActionResult> EliminarTitular(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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

        // POST: CajasDeAhorro/EliminarTitular/5
        [HttpPost, ActionName("EliminarTitular")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTitular(int id, int dni)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Usuario? titular = _context.usuarios.Where(usuario => usuario.dni == dni).FirstOrDefault();
                CajaDeAhorro? caja = await _context.cajas.FindAsync(id);
                if (titular == null)
                {
                    return RedirectToAction("EliminarTitular", "CajasDeAhorro", new { mensaje = "No se encontró usuario con este DNI en la lista de Usuarios del Banco." });
                }
                if (caja == null)
                {
                    return RedirectToAction("EliminarTitular", "CajasDeAhorro", new { mensaje = "No se encontró la caja de ahorro en la lista de cajas de ahorro." });
                }
                if (!caja.titulares.Contains(titular))
                {
                    return RedirectToAction("EliminarTitular", "CajasDeAhorro", new { mensaje = "El usuario no es titular de esta caja de ahorro." });
                }
                if (caja.titulares.Count < 2)
                {
                    return RedirectToAction("EliminarTitular", "CajasDeAhorro", new { mensaje = "No puedes dejar la caja de ahorros sin titulares." });
                }
                caja.titulares.Remove(titular);
                titular.cajas.Remove(caja);
                _context.Update(caja);
                _context.Update(titular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return Problem(e.ToString());
            }

        }

        // GET: CajasDeAhorro/AgregarTitular/5
        public async Task<IActionResult> AgregarTitular(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {

                CajaDeAhorro? caja = await _context.cajas.FindAsync(id);
                Usuario? userAdd = _context.usuarios.Where(usuario => usuario.dni == dni).FirstOrDefault();
                if (userAdd == null)
                {
                    return RedirectToAction("AgregarTitular", "CajasDeAhorro", new { mensaje = "No se encontró usuario con este DNI en la lista de Usuarios del Banco (" + dni + ")" });                
                }
                if (caja == null)
                {
                    return RedirectToAction("AgregarTitular", "CajasDeAhorro", new { mensaje = "No se encontró la caja de ahorro con ese  (" + id + ")" });              
                }

                if (caja.titulares.Contains(userAdd))
                {
                    return RedirectToAction("AgregarTitular", "CajasDeAhorro", new { mensaje = "El usuario ya es titular de esta caja de ahorro." });
                }
                caja.titulares.Add(userAdd);
                userAdd.cajas.Add(caja);
                _context.Update(caja);
                _context.Update(userAdd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Ocurrio un error.");
            }

        }

        // GET: CajasDeAhorro/Depositar/5
        public async Task<IActionResult> Depositar(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.cajas == null)
            {
                return Problem("ID null or _context.cajas null.");
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return Problem("Caja de ahorro inexistente.");
            }

            return View(cajaDeAhorro);
        }


        // POST: CajasDeAhorro/Depositar/5
        [HttpPost, ActionName("Depositar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Depositar(int id, int monto)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                CajaDeAhorro cajaDestino = await _context.cajas.FindAsync(id);
                if (cajaDestino == null)
                {
                    return RedirectToAction("Depositar", "CajasDeAhorro", new { mensaje = "Caja de ahorro inexistente." });
                }
                cajaDestino.saldo += monto;
                _context.Update(cajaDestino);
                Movimiento movimientoNuevo = new Movimiento(cajaDestino, "Deposito", monto);
                _context.movimientos.Add(movimientoNuevo);
                cajaDestino.movimientos.Add(movimientoNuevo);
                _context.Update(cajaDestino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Error.");
            }
        }

        // GET: CajasDeAhorro/Retirar/5
        public async Task<IActionResult> Retirar(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.cajas == null)
            {
                return Problem("ID null or _context.cajas null");
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return Problem("No existe tal caja de ahorro");
            }

            return View(cajaDeAhorro);
        }


        // POST: CajasDeAhorro/Retirar/5
        [HttpPost, ActionName("Retirar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retirar(int id, int monto)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                CajaDeAhorro cajaDestino = await _context.cajas.FindAsync(id);
                if (cajaDestino == null)
                {
                    return RedirectToAction("Retirar", "CajasDeAhorro", new { mensaje = "No se encontro la caja de ahorro." });
                }
                if (cajaDestino.saldo < monto)
                {
                    return RedirectToAction("Retirar", "CajasDeAhorro", new { mensaje = "El saldo es menor al monto que se desea retirar." });
                }
                cajaDestino.saldo -= monto;
                _context.Update(cajaDestino);
                Movimiento movimientoNuevo = new Movimiento(cajaDestino, "Retiro", monto);
                _context.movimientos.Add(movimientoNuevo);
                cajaDestino.movimientos.Add(movimientoNuevo);
                _context.Update(cajaDestino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Error.");
            }
        }
        // GET: CajasDeAhorro/Transferir/5
        public async Task<IActionResult> Transferir(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.cajas == null)
            {
                return Problem("ID null or _context.cajas null");
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m.id == id);
            if (cajaDeAhorro == null)
            {
                return Problem("No existe tal caja de ahorro");
            }

            return View(cajaDeAhorro);
        }


        // POST: CajasDeAhorro/Transferir/5
        [HttpPost, ActionName("Transferir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferir(int id, int monto, int cbu)
        {
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                CajaDeAhorro cajaOrigen = await _context.cajas.FindAsync(id);
                CajaDeAhorro cajaDestino = _context.cajas.Where(caja => caja.cbu == cbu).FirstOrDefault();
                if (cajaOrigen.saldo < monto)
                {
                    return RedirectToAction("Transferir", "CajasDeAhorro", new { mensaje = "El monto es mayor que el saldo de la caja." });
                }
                if (cajaDestino == null)
                {
                    return RedirectToAction("Transferir", "CajasDeAhorro", new { mensaje = "No se encontro caja destino." });
                }
                cajaOrigen.saldo -= monto;
                Movimiento movimientoNuevo = new Movimiento(cajaOrigen, "Transferencia realizada", monto);
                _context.movimientos.Add(movimientoNuevo);
                cajaOrigen.movimientos.Add(movimientoNuevo);
                _context.Update(cajaOrigen);
                cajaDestino.saldo += monto;
                Movimiento movimientoNuevo2 = new Movimiento(cajaDestino, "Transferencia recibida", monto);
                _context.movimientos.Add(movimientoNuevo2);
                cajaDestino.movimientos.Add(movimientoNuevo2);
                _context.Update(cajaDestino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Problem("Error: "+ex.ToString());
            }
        }

        // GET: CajasDeAhorro/Delete/5
        public async Task<IActionResult> Delete(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                CajaDeAhorro? cajaARemover = await _context.cajas.FindAsync(id);
                if (cajaARemover == null)
                {
                    return RedirectToAction("Delete", "CajasDeAhorro", new { mensaje = "No existe caja." });
                }
                if (cajaARemover.saldo != 0)
                {
                    return RedirectToAction("Delete", "CajasDeAhorro", new { mensaje = "Tiene saldo, no se puede eliminar." });
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

        // GET: CajasDeAhorro/Movimientos/5
        public async Task<IActionResult> Movimientos(int? id, string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            if (usuarioLogueado == null)
            {
                return RedirectToAction("Index", "Home");
            }
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

    }

}
