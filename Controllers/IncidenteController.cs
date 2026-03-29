using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SISTEMA_INTEGRAL_GISOR.Models;
using GisorSystem.Services;
using GisorSystem.Repositories;
using System.Security.Claims;

namespace GisorSystem.Controllers
{
    public class IncidenteController : Controller
    {
        private readonly GestorIncidenteService _gestor;
        private readonly IRepository<TipoIncidente> _repoTipos;
        private readonly IWebHostEnvironment _env;

        public IncidenteController(GestorIncidenteService gestor,
                                   IRepository<TipoIncidente> repoTipos,
                                   IWebHostEnvironment env)
        {
            _gestor = gestor;
            _repoTipos = repoTipos;
            _env = env;
        }

        // GET: Listado y Búsqueda (Mapa Público)
        public async Task<IActionResult> Index(string busqueda)
        {
            // --- BLOQUEO DE SEGURIDAD ---
            // Admin y Mod NO deben ver el mapa público, tienen sus propios paneles.
            if (User.IsInRole("Administrador") || User.IsInRole("Moderador"))
            {
                return RedirectToAction("Index", "Home");
            }

            IEnumerable<Incidente> lista;

            if (!string.IsNullOrEmpty(busqueda))
            {
                lista = await _gestor.BuscarIncidentes(busqueda);
            }
            else
            {
                lista = await _gestor.ObtenerIncidentesRecientes();
            }

            // Filtro: Solo aprobados para el público
            lista = lista.Where(i => !i.Descripcion.Contains("[PENDIENTE"));

            return View(lista);
        }

        // GET: Crear
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            // --- BLOQUEO DE SEGURIDAD ---
            if (User.IsInRole("Administrador") || User.IsInRole("Moderador"))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Tipos = await _repoTipos.GetAllAsync();
            return View();
        }

        // POST: Crear
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Incidente incidente, IFormFile? fotoEvidencia)
        {
            // --- BLOQUEO DE SEGURIDAD ---
            if (User.IsInRole("Administrador") || User.IsInRole("Moderador"))
            {
                return RedirectToAction("Index", "Home");
            }

            bool esAnonimo = !User.Identity.IsAuthenticated;

            // Asignación de IDs
            if (esAnonimo)
            {
                incidente.CiudadanoId = 1;
            }
            else
            {
                var claimCiudadano = User.FindFirst("CiudadanoID");
                if (claimCiudadano != null && int.TryParse(claimCiudadano.Value, out int cid))
                {
                    incidente.CiudadanoId = cid;
                }
                else
                {
                    incidente.CiudadanoId = 1;
                }
            }

            incidente.FechaHora = DateTime.Now;
            if (incidente.UbigeoId == 0) incidente.UbigeoId = 1;

            await _gestor.RegistrarIncidente(incidente, esAnonimo);

            if (fotoEvidencia != null && fotoEvidencia.Length > 0)
            {
                string rutaCarpeta = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoEvidencia.FileName);
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await fotoEvidencia.CopyToAsync(stream);
                }

                await _gestor.GuardarEvidencia(incidente.IncidenteId, "/uploads/" + nombreArchivo, "Imagen", Path.GetExtension(fotoEvidencia.FileName));
            }

            return RedirectToAction("Index");
        }

        // GET: Detalles
        public async Task<IActionResult> Details(int id)
        {
            // Nota: Aquí permitimos entrar a Admin/Mod por si necesitan ver detalle desde sus paneles,
            // pero si quieres bloquearlo también, agrega el if aquí.
            // Por lógica, el moderador SÍ necesita ver detalles para aprobar.

            var lista = await _gestor.ObtenerIncidentesRecientes();
            var incidente = lista.FirstOrDefault(i => i.IncidenteId == id);

            if (incidente == null) return NotFound();

            ViewBag.Evidencias = await _gestor.ObtenerEvidencias(id);

            return View(incidente);
        }

        // GET: Mis Reportes
        [Authorize]
        public async Task<IActionResult> MisReportes()
        {
            // --- BLOQUEO DE SEGURIDAD ---
            // Admin y Mod no reportan, por ende no tienen "Mis Reportes"
            if (User.IsInRole("Administrador") || User.IsInRole("Moderador"))
            {
                return RedirectToAction("Index", "Home");
            }

            var claimCiudadano = User.FindFirst("CiudadanoID");

            if (claimCiudadano == null || !int.TryParse(claimCiudadano.Value, out int ciudadanoId))
            {
                return RedirectToAction("Index", "Home");
            }

            var todosIncidentes = await _gestor.ObtenerIncidentesRecientes();

            var misIncidentes = todosIncidentes
                                .Where(i => i.CiudadanoId == ciudadanoId)
                                .OrderByDescending(i => i.FechaHora)
                                .ToList();

            return View(misIncidentes);
        }
    }
}