namespace GisorSystem.Services
{
    // Patrón GRASP: Pure Fabrication / Information Expert
    // Centraliza reglas de negocio que no pertenecen estrictamente a una sola entidad.
    public class ValidadorSistema
    {
        private const int LIMITE_ALERTAS_ANONIMAS = 2;

        // Validar si un usuario anónimo puede emitir alerta
        public bool PuedeEmitirAlertaAnonima(int alertasEmitidasPreviamente)
        {
            return alertasEmitidasPreviamente < LIMITE_ALERTAS_ANONIMAS;
        }

        // Validar si requiere aprobación de moderador (Regla: Anónimos siempre requieren, Registrados no)
        public bool RequiereAprobacionModerador(bool esUsuarioAnonimo)
        {
            return esUsuarioAnonimo;
        }

        // Validar permisos de datos sensibles
        public bool ValidarConsentimientoDatos(bool aceptoTerminos, bool aceptoUbicacion)
        {
            if (!aceptoTerminos) return false;
            if (!aceptoUbicacion) return false;
            return true;
        }

        // Validar formato básico de DNI (Regla simple para Perú: 8 dígitos numéricos)
        public bool EsDniValido(string dni)
        {
            return !string.IsNullOrEmpty(dni) && dni.Length == 8 && dni.All(char.IsDigit);
        }
    }
}