using GisorSystem.Repositories;
using SISTEMA_INTEGRAL_GISOR.Models;
using System.Security.Policy;

namespace GisorSystem.Services
{
    public class GestorIncidenteService
    {
        // 1. VARIABLES (Solo una vez)
        private readonly IRepository<Incidente> _incidenteRepo;
        private readonly IRepository<EventoSo> _sosRepo;
        private readonly IRepository<HistorialEstado> _historialRepo;
        private readonly IRepository<Evidencium> _evidenciaRepo;
        private readonly ValidadorSistema _validador;

        // 2. CONSTRUCTOR (Solo una vez)
        public GestorIncidenteService(IRepository<Incidente> incidenteRepo,
                                      IRepository<EventoSo> sosRepo,
                                      IRepository<HistorialEstado> historialRepo,
                                      IRepository<Evidencium> evidenciaRepo,
                                      ValidadorSistema validador)
        {
            _incidenteRepo = incidenteRepo;
            _sosRepo = sosRepo;
            _historialRepo = historialRepo;
            _evidenciaRepo = evidenciaRepo;
            _validador = validador;
        }

        // 3. MÉTODOS

        public async Task RegistrarIncidente(Incidente incidente, bool esAnonimo)
        {
            incidente.FechaHora = DateTime.Now;

            if (_validador.RequiereAprobacionModerador(esAnonimo))
            {
                incidente.Descripcion = "[PENDIENTE APROBACION] " + incidente.Descripcion;
            }

            await _incidenteRepo.AddAsync(incidente);

            int estadoInicial = _validador.RequiereAprobacionModerador(esAnonimo) ? 1 : 2;

            var historial = new HistorialEstado
            {
                IncidenteId = incidente.IncidenteId,
                EstadoIncidenteId = estadoInicial,
                FechaCambio = DateTime.Now,
                EmpleadoId = 1,
                Comentario = "Registro inicial automático."
            };

            await _historialRepo.AddAsync(historial);
        }

        public async Task GuardarEvidencia(int incidenteId, string urlArchivo, string tipo, string formato)
        {
            var evidencia = new Evidencium
            {
                IncidenteId = incidenteId,
                UrlArchivo = urlArchivo,
                TipoArchivo = tipo,
                Formato = formato,
                FechaSubida = DateTime.Now
            };
            await _evidenciaRepo.AddAsync(evidencia);
        }

        public async Task<IEnumerable<Evidencium>> ObtenerEvidencias(int incidenteId)
        {
            return await _evidenciaRepo.FindAsync(e => e.IncidenteId == incidenteId);
        }

        public async Task<bool> ProcesarAlertaSOS(EventoSo evento, int alertasPreviasAnonimo, bool esAnonimo)
        {
            if (esAnonimo && !_validador.PuedeEmitirAlertaAnonima(alertasPreviasAnonimo)) return false;

            evento.FechaHora = DateTime.Now;
            await _sosRepo.AddAsync(evento);

            return true;
        }

        public async Task<IEnumerable<Incidente>> ObtenerIncidentesRecientes()
        {
            return await _incidenteRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Incidente>> BuscarIncidentes(string termino)
        {
            return await _incidenteRepo.FindAsync(i => i.Descripcion.Contains(termino));
        }
    }
}