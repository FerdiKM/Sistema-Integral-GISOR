namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class HistorialEstado
{
    public int HistorialEstadosId { get; set; }

    public int EmpleadoId { get; set; }

    public int EstadoIncidenteId { get; set; }

    public DateTime FechaCambio { get; set; }

    public int IncidenteId { get; set; }

    public string? Comentario { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual EstadoIncidente EstadoIncidente { get; set; } = null!;

    public virtual Incidente Incidente { get; set; } = null!;
}
