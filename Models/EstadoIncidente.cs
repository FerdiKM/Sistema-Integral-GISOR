namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class EstadoIncidente
{
    public string Descripcion { get; set; } = null!;

    public int EstadoIncidenteId { get; set; }

    public virtual ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();
}
