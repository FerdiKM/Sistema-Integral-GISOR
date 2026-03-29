namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Evidencium
{
    public int EvidenciaId { get; set; }

    public int IncidenteId { get; set; }

    public string UrlArchivo { get; set; } = null!;

    public string TipoArchivo { get; set; } = null!;

    public string Formato { get; set; } = null!;

    public DateTime FechaSubida { get; set; }

    public virtual Incidente Incidente { get; set; } = null!;
}
