namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class EventoSo
{
    public int EventoSosId { get; set; }

    public int CiudadanoId { get; set; }

    public DateTime FechaHora { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public virtual Ciudadano Ciudadano { get; set; } = null!;
}
