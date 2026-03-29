namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Rol
{
    public string Nombre { get; set; } = null!;

    public int RolId { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
