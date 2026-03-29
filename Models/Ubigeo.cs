namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Ubigeo
{
    public string Provincia { get; set; } = null!;

    public int UbigeoId { get; set; }

    public string Distrito { get; set; } = null!;

    public int CodigoPostal { get; set; }

    public virtual ICollection<Ciudadano> Ciudadanos { get; set; } = new List<Ciudadano>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
