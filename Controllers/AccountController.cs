using GisorSystem.Repositories;
using GisorSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_INTEGRAL_GISOR.Models;
using System.Security.Claims;

namespace GisorSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly GestorUsuarioService _gestorUsuario;
        private readonly IRepository<Ciudadano> _ciudadanoRepo;
        private readonly IRepository<Usuario> _usuarioRepo; // <--- NUEVO: Para poder eliminar la cuenta
        private readonly IWebHostEnvironment _env;

        public AccountController(GestorUsuarioService gestorUsuario,
                                 IRepository<Ciudadano> ciudadanoRepo,
                                 IRepository<Usuario> usuarioRepo, // <--- INYECCIÓN
                                 IWebHostEnvironment env)
        {
            _gestorUsuario = gestorUsuario;
            _ciudadanoRepo = ciudadanoRepo;
            _usuarioRepo = usuarioRepo; // <--- ASIGNACIÓN
            _env = env;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            // 1. Autenticar
            var usuario = await _gestorUsuario.AutenticarUsuario(correo, password);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas o su cuenta aún está en revisión.";
                return View();
            }

            // 2. Buscar ID Ciudadano
            var ciudadanos = await _ciudadanoRepo.FindAsync(c => c.UsuarioId == usuario.UsuarioId);
            var ciudadano = ciudadanos.FirstOrDefault();
            string ciudadanoIdStr = ciudadano != null ? ciudadano.CiudadanoId.ToString() : "0";

            // 3. Roles
            string nombreRol = "Ciudadano";
            if (usuario.RolId == 1) nombreRol = "Administrador";
            if (usuario.RolId == 2) nombreRol = "Ciudadano";
            if (usuario.RolId == 3) nombreRol = "Moderador";

            // 4. Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim("CiudadanoID", ciudadanoIdStr),
                new Claim(ClaimTypes.Role, nombreRol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario, Ciudadano ciudadano, string passwordConfirm, IFormFile? fotoDni)
        {
            if (usuario.Password != passwordConfirm)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            // 1. Manejo de la Foto DNI
            if (fotoDni != null && fotoDni.Length > 0)
            {
                string nombreArchivo = "DNI_" + Guid.NewGuid().ToString() + Path.GetExtension(fotoDni.FileName);
                string rutaCarpeta = Path.Combine(_env.WebRootPath, "uploads", "dni");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await fotoDni.CopyToAsync(stream);
                }

                ciudadano.FotoDniUrl = "/uploads/dni/" + nombreArchivo;
            }
            else
            {
                ViewBag.Error = "Es obligatorio subir una foto del DNI.";
                return View();
            }

            // 2. Configuración Inicial
            usuario.RolId = 2; // Ciudadano
            usuario.Activo = false; // INACTIVO HASTA QUE EL MODERADOR APRUEBE

            // 3. Registro
            bool registroExitoso = await _gestorUsuario.RegistrarCiudadano(usuario, ciudadano);

            if (!registroExitoso)
            {
                ViewBag.Error = "Error al registrar. Verifique DNI o Correo existente.";
                return View();
            }

            ViewBag.Success = "Registro recibido. Un moderador verificará su identidad pronto.";
            return View("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // --- NUEVOS MÉTODOS PARA CONFIGURAR CUENTA ---

        // GET: Perfil de Usuario
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login");

            int userId = int.Parse(userIdStr);

            var usuario = await _usuarioRepo.GetByIdAsync(userId);
            var ciudadanos = await _ciudadanoRepo.FindAsync(c => c.UsuarioId == userId);
            var ciudadano = ciudadanos.FirstOrDefault();

            ViewBag.Ciudadano = ciudadano;
            return View(usuario);
        }

        // POST: Eliminar Cuenta
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login");
            int userId = int.Parse(userIdStr);

            // 1. Eliminar perfil de Ciudadano (Hijo)
            var ciudadanos = await _ciudadanoRepo.FindAsync(c => c.UsuarioId == userId);
            var ciudadano = ciudadanos.FirstOrDefault();
            if (ciudadano != null)
            {
                await _ciudadanoRepo.DeleteAsync(ciudadano.CiudadanoId);
            }

            // 2. Eliminar Usuario (Padre)
            await _usuarioRepo.DeleteAsync(userId);

            // 3. Cerrar Sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}