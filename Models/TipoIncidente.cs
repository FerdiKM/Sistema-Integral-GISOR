namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class TipoIncidente
{
    public string Descripcion { get; set; } = null!;

    public int TipoIncidenteId { get; set; }

    public string NivelGravedad { get; set; } = null!;

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
