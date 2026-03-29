namespace GisorSystem.Services
{
    public class SimuladorAlertaService
    {
        // Simula el envío de una alerta y retorna un código de ticket ficticio
        public string NotificarAutoridad(string tipoAutoridad, string descripcionIncidente, string ubicacion)
        {
            // Aquí iría la lógica real de API (HTTP Client)
            // Para el prototipo, generamos un código de seguimiento simulado
            string codigoSeguimiento = $"{tipoAutoridad.ToUpper().Substring(0, 3)}-{DateTime.Now.Ticks.ToString().Substring(10)}";

            return codigoSeguimiento;
        }
    }
}