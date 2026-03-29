namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class Usuario
{
    public Usuario()
    {
        // Esto se ejecuta apenas se crea "new Usuario()"
        // Garantiza que SIEMPRE tenga una fecha válida desde el nacimiento del objeto.
        FechaRegistro = DateTime.Now;

        // También podemos inicializar el Rol y Activo por defecto aquí
        Activo = false;

        // Inicializamos las listas de navegación para evitar nulos
        Ciudadanos = new HashSet<Ciudadano>();
        Empleados = new HashSet<Empleado>();
    }
    public int UsuarioId { get; set; }

    public string Correo { get; set; } = null!;

    public int RolId { get; set; }

    public string Password { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Ciudadano> Ciudadanos { get; set; } = new List<Ciudadano>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual Rol Rol { get; set; } = null!;
}
