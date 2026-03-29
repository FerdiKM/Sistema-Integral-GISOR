namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Incidente
{
    public int IncidenteId { get; set; }

    public int CiudadanoId { get; set; }

    public int? EmpleadoId { get; set; }

    public int TipoIncidenteId { get; set; }

    public string Descripcion { get; set; } = null!;

    public int UbigeoId { get; set; }

    public DateTime FechaHora { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public virtual Ciudadano Ciudadano { get; set; } = null!;

    public virtual Empleado? Empleado { get; set; }

    public virtual ICollection<Evidencium> Evidencia { get; set; } = new List<Evidencium>();

    public virtual ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();

    public virtual TipoIncidente TipoIncidente { get; set; } = null!;

    public virtual Ubigeo Ubigeo { get; set; } = null!;
}
