using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using WebApplication_plataformas_de_desarrollo.Data;
using WebApplication_plataformas_de_desarrollo.Models;

namespace WebApplication_plataformas_de_desarrollo.Controllers
{
    public class HomeController : Controller
    {
        private readonly MiContexto _context;
        private Usuario? usuarioLogueado;

        public HomeController(MiContexto context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            usuarioLogueado = _context.usuarios.Where(u => u.id == httpContextAccessor.HttpContext.Session.GetInt32("IdUsuario")).FirstOrDefault();
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
        }
        public IActionResult Index()
        {
            var sessionUserId = HttpContext.Session.GetInt32("IdUsuario");
            ViewData["mensaje"] = (sessionUserId != null && usuarioLogueado != null) ? "Bienvenido " + usuarioLogueado.nombre + " " + usuarioLogueado.apellido + ", "+ usuarioLogueado.mail : "Inicia sesión para continuar";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Login(int dni, string password)
        {
            try
            {
                var usuario = _context.usuarios.Where(u => u.dni == dni).FirstOrDefault();
                if (usuario == null)
                {
                    return RedirectToAction("Login", "Home", new { mensaje = "No existe usuario con ese DNI." });
                }
                if (usuario.bloqueado)
                {
                    return RedirectToAction("Login", "Home", new { mensaje = "Ese usuario esta bloqueado." });
                }
                if (usuario.password != password)
                {
                    usuario.intentosFallidos++;
                    _context.Update(usuario);
                    _context.SaveChanges();
                        if (usuario.intentosFallidos >= 3)           
                        {
                            usuario.bloqueado = true;
                            _context.Update(usuario);
                            _context.SaveChanges();
                            return RedirectToAction("Login", "Home", new { mensaje = "Numero de intentos excedidos. Se ha bloqueado el usuario." });
                        }
                        else
                        {
                            return RedirectToAction("Login", "Home", new { mensaje = "Contraseña incorrecta, "+ (3-usuario.intentosFallidos) +" intentos restantes." });
                        }
                }
                usuario.intentosFallidos = 0;
                _context.Update(usuario);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("IdUsuario",usuario.id);
                HttpContext.Session.SetInt32("IsAdmin", usuario.isAdmin ? 1 : 0);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View(); //Error
            }
        }

        [HttpGet]
        public IActionResult Login(string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); //Usuario logueado
        }

        [HttpGet]
        public IActionResult Registrarse(string mensaje = "")
        {
            ViewData["mensaje"] = mensaje;
            return View();
        }

    }
}