namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Empleado
{
    public int EmpleadoId { get; set; }

    public int UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public int UbigeoId { get; set; }

    public string Area { get; set; } = null!;

    public virtual ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual ICollection<ListaNegra> ListaNegras { get; set; } = new List<ListaNegra>();

    public virtual Ubigeo Ubigeo { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
