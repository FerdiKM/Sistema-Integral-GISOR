using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_INTEGRAL_GISOR.Models;
using GisorSystem.Repositories;
using GisorSystem.Services;

namespace GisorSystem.Controllers
{
    [Authorize(Roles = "Administrador,Moderador")]
    public class ModeradorController : Controller
    {
        private readonly IRepository<Incidente> _incidenteRepo;
        private readonly IRepository<HistorialEstado> _historialRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Ciudadano> _ciudadanoRepo;
        private readonly SimuladorAlertaService _simuladorAlerta;

        public ModeradorController(IRepository<Incidente> incidenteRepo,
                                   IRepository<HistorialEstado> historialRepo,
                                   IRepository<Usuario> usuarioRepo,
                                   IRepository<Ciudadano> ciudadanoRepo,
                                   SimuladorAlertaService simuladorAlerta)
        {
            _incidenteRepo = incidenteRepo;
            _historialRepo = historialRepo;
            _usuarioRepo = usuarioRepo;
            _ciudadanoRepo = ciudadanoRepo;
            _simuladorAlerta = simuladorAlerta;
        }

        // ==========================================
        // 1. DASHBOARD PRINCIPAL (MENÚ)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Obtener TODOS los incidentes para el mapa
            var todosIncidentes = await _incidenteRepo.GetAllAsync();

            // Calcular contadores
            var incidentesPendientes = todosIncidentes.Count(i => i.Descripcion.Contains("[PENDIENTE"));
            var usuariosPendientes = (await _usuarioRepo.FindAsync(u => u.Activo == false && u.RolId == 2)).Count();

            ViewBag.CantIncidentes = incidentesPendientes;
            ViewBag.CantUsuarios = usuariosPendientes;

            // Retornamos todos los incidentes para que el mapa los pinte
            return View(todosIncidentes);
        }

        // ==========================================
        // 2. GESTIÓN DE INCIDENTES (Antes era Index)
        // ==========================================
        public async Task<IActionResult> IncidentesPendientes()
        {
            var incidentes = await _incidenteRepo.FindAsync(i => i.Descripcion.Contains("[PENDIENTE"));
            return View(incidentes);
        }

        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {
            var incidente = await _incidenteRepo.GetByIdAsync(id);
            if (incidente != null)
            {
                incidente.Descripcion = incidente.Descripcion.Replace("[PENDIENTE APROBACION] ", "").Trim();
                await _incidenteRepo.UpdateAsync(incidente);

                var historial = new HistorialEstado
                {
                    IncidenteId = incidente.IncidenteId,
                    EstadoIncidenteId = 2,
                    FechaCambio = DateTime.Now,
                    EmpleadoId = 1,
                    Comentario = "Aprobado manualmente."
                };
                await _historialRepo.AddAsync(historial);
            }
            return RedirectToAction("IncidentesPendientes"); // Redirige a la lista
        }

        [HttpPost]
        public async Task<IActionResult> Rechazar(int id)
        {
            await _incidenteRepo.DeleteAsync(id);
            return RedirectToAction("IncidentesPendientes");
        }

        [HttpPost]
        public async Task<IActionResult> NotificarAutoridad(int id, string autoridad)
        {
            var incidente = await _incidenteRepo.GetByIdAsync(id);
            if (incidente != null)
            {
                string ticket = _simuladorAlerta.NotificarAutoridad(autoridad, incidente.Descripcion, $"{incidente.Latitud},{incidente.Longitud}");
                TempData["MensajeNotificacion"] = $"✅ {autoridad} notificada. Ticket: {ticket}";
            }
            return RedirectToAction("IncidentesPendientes");
        }

        [HttpPost]
        public async Task<IActionResult> BloquearUsuario(int idIncidente, string motivo)
        {
            var incidente = await _incidenteRepo.GetByIdAsync(idIncidente);
            if (incidente != null)
            {
                await _incidenteRepo.DeleteAsync(idIncidente);
            }
            return RedirectToAction("IncidentesPendientes");
        }

        // ==========================================
        // 3. GESTIÓN DE USUARIOS
        // ==========================================
        public async Task<IActionResult> UsuariosPendientes()
        {
            var usuariosPendientes = await _usuarioRepo.FindAsync(u => u.Activo == false && u.RolId == 2);
            ViewBag.Ciudadanos = await _ciudadanoRepo.GetAllAsync();
            return View(usuariosPendientes);
        }

        [HttpPost]
        public async Task<IActionResult> ActivarUsuario(int id)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activo = true;
                await _usuarioRepo.UpdateAsync(usuario);
            }
            return RedirectToAction("UsuariosPendientes");
        }

        [HttpPost]
        public async Task<IActionResult> RechazarUsuario(int id)
        {
            var ciudadanos = await _ciudadanoRepo.FindAsync(c => c.UsuarioId == id);
            var ciudadano = ciudadanos.FirstOrDefault();
            if (ciudadano != null) await _ciudadanoRepo.DeleteAsync(ciudadano.CiudadanoId);
            await _usuarioRepo.DeleteAsync(id);
            return RedirectToAction("UsuariosPendientes");
        }
    }
}