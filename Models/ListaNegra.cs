namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class ListaNegra
{
    public int ListaNegraId { get; set; }

    public int EmpleadoId { get; set; }

    public string Motivo { get; set; } = null!;

    public DateTime FechaBloqueo { get; set; }

    public string IpBloqueada { get; set; } = null!;

    public int CiudadanoId { get; set; }

    public virtual ICollection<Ciudadano> Ciudadanos { get; set; } = new List<Ciudadano>();

    public virtual Empleado Empleado { get; set; } = null!;
}
