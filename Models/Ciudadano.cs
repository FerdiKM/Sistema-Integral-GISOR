namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Ciudadano
{
    public int CiudadanoId { get; set; }

    public int? ListaNegraId { get; set; }

    public int UsuarioId { get; set; }

    public string Dni { get; set; } = null!;

    public int UbigeoId { get; set; }

    public string Telefono { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string? FotoDniUrl { get; set; }

    public virtual ICollection<EventoSo> EventoSos { get; set; } = new List<EventoSo>();

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual ListaNegra? ListaNegra { get; set; }

    public virtual Ubigeo Ubigeo { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
