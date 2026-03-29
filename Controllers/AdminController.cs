using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_INTEGRAL_GISOR.Models;
using GisorSystem.Repositories;

namespace GisorSystem.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IRepository<Incidente> _incidenteRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<TipoIncidente> _tipoRepo;
        private readonly IRepository<Ubigeo> _ubigeoRepo;

        public AdminController(IRepository<Incidente> incidenteRepo,
                               IRepository<Usuario> usuarioRepo,
                               IRepository<TipoIncidente> tipoRepo,
                               IRepository<Ubigeo> ubigeoRepo)
        {
            _incidenteRepo = incidenteRepo;
            _usuarioRepo = usuarioRepo;
            _tipoRepo = tipoRepo;
            _ubigeoRepo = ubigeoRepo;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Obtener datos
            var incidentes = await _incidenteRepo.GetAllAsync(); // <-- ESTO ES LO QUE USAREMOS EN EL MAPA
            var usuarios = await _usuarioRepo.GetAllAsync();
            var tipos = await _tipoRepo.GetAllAsync();
            var ubigeos = await _ubigeoRepo.GetAllAsync();

            // 2. KPIs
            ViewBag.TotalIncidentes = incidentes.Count();
            ViewBag.IncidentesPendientes = incidentes.Count(i => i.Descripcion.Contains("[PENDIENTE"));
            ViewBag.TotalUsuarios = usuarios.Count();
            ViewBag.UsuariosInactivos = usuarios.Count(u => u.Activo == false);

            // 3. Gráficos (Pastel y Barras)
            var datosPorTipo = incidentes.GroupBy(i => i.TipoIncidenteId)
                .Select(g => new { Tipo = tipos.FirstOrDefault(t => t.TipoIncidenteId == g.Key)?.Descripcion ?? "Otros", Cantidad = g.Count() }).ToList();
            ViewBag.LabelsTipo = datosPorTipo.Select(x => x.Tipo).ToArray();
            ViewBag.DataTipo = datosPorTipo.Select(x => x.Cantidad).ToArray();

            var datosPorUbigeo = incidentes.GroupBy(i => i.UbigeoId)
                .Select(g => new { Distrito = ubigeos.FirstOrDefault(u => u.UbigeoId == g.Key)?.Distrito ?? "Desconocido", Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad).Take(5).ToList();
            ViewBag.LabelsDistrito = datosPorUbigeo.Select(x => x.Distrito).ToArray();
            ViewBag.DataDistrito = datosPorUbigeo.Select(x => x.Cantidad).ToArray();

            // RETORNAMOS LA LISTA COMPLETA DE INCIDENTES A LA VISTA
            return View(incidentes);
        }
    }
}