using Microsoft.EntityFrameworkCore;

namespace SISTEMA_INTEGRAL_GISOR.Models;

public partial class GisorContext : DbContext
{
    public GisorContext()
    {
    }

    public GisorContext(DbContextOptions<GisorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ciudadano> Ciudadanos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EstadoIncidente> EstadoIncidentes { get; set; }

    public virtual DbSet<EventoSo> EventoSos { get; set; }

    public virtual DbSet<Evidencium> Evidencia { get; set; }

    public virtual DbSet<HistorialEstado> HistorialEstados { get; set; }

    public virtual DbSet<Incidente> Incidentes { get; set; }

    public virtual DbSet<ListaNegra> ListaNegras { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoIncidente> TipoIncidentes { get; set; }

    public virtual DbSet<Ubigeo> Ubigeos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-8BSRC2B;Database=GISORDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ciudadano>(entity =>
        {
            entity.ToTable("Ciudadano");

            entity.Property(e => e.CiudadanoId).HasColumnName("CiudadanoID");
            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.FotoDniUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ListaNegraId).HasColumnName("Lista_negraID");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_completo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UbigeoId).HasColumnName("UbigeoID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.ListaNegra).WithMany(p => p.Ciudadanos)
                .HasForeignKey(d => d.ListaNegraId)
                .HasConstraintName("FK_Ciudadano_Lista_negra");

            entity.HasOne(d => d.Ubigeo).WithMany(p => p.Ciudadanos)
                .HasForeignKey(d => d.UbigeoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ciudadano_Ubigeo");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Ciudadanos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ciudadano_Usuario");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("Empleado");

            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_completo");
            entity.Property(e => e.UbigeoId).HasColumnName("UbigeoID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Ubigeo).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.UbigeoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_Ubigeo");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_Usuario");
        });

        modelBuilder.Entity<EstadoIncidente>(entity =>
        {
            entity.ToTable("Estado_incidente");

            entity.Property(e => e.EstadoIncidenteId).HasColumnName("Estado_incidenteID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventoSo>(entity =>
        {
            entity.HasKey(e => e.EventoSosId);

            entity.ToTable("Evento_sos");

            entity.Property(e => e.EventoSosId).HasColumnName("Evento_sosID");
            entity.Property(e => e.CiudadanoId).HasColumnName("CiudadanoID");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_hora");
            entity.Property(e => e.Latitud).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitud).HasColumnType("decimal(10, 8)");

            entity.HasOne(d => d.Ciudadano).WithMany(p => p.EventoSos)
                .HasForeignKey(d => d.CiudadanoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evento_sos_Ciudadano");
        });

        modelBuilder.Entity<Evidencium>(entity =>
        {
            entity.HasKey(e => e.EvidenciaId);

            entity.Property(e => e.EvidenciaId).HasColumnName("EvidenciaID");
            entity.Property(e => e.FechaSubida)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_subida");
            entity.Property(e => e.Formato)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IncidenteId).HasColumnName("IncidenteID");
            entity.Property(e => e.TipoArchivo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Tipo_archivo");
            entity.Property(e => e.UrlArchivo)
                .HasColumnType("text")
                .HasColumnName("Url_archivo");

            entity.HasOne(d => d.Incidente).WithMany(p => p.Evidencia)
                .HasForeignKey(d => d.IncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evidencia_Incidente");
        });

        modelBuilder.Entity<HistorialEstado>(entity =>
        {
            entity.HasKey(e => e.HistorialEstadosId);

            entity.ToTable("Historial_estados");

            entity.Property(e => e.HistorialEstadosId).HasColumnName("Historial_estadosID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.EstadoIncidenteId).HasColumnName("Estado_incidenteID");
            entity.Property(e => e.FechaCambio)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_cambio");
            entity.Property(e => e.IncidenteId).HasColumnName("IncidenteID");

            entity.HasOne(d => d.Empleado).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_estados_Empleado");

            entity.HasOne(d => d.EstadoIncidente).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.EstadoIncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_estados_Estado_incidente");

            entity.HasOne(d => d.Incidente).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.IncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_estados_Incidente");
        });

        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.ToTable("Incidente");

            entity.Property(e => e.IncidenteId).HasColumnName("IncidenteID");
            entity.Property(e => e.CiudadanoId).HasColumnName("CiudadanoID");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_hora");
            entity.Property(e => e.Latitud).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitud).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.TipoIncidenteId).HasColumnName("Tipo_incidenteID");
            entity.Property(e => e.UbigeoId).HasColumnName("UbigeoID");

            entity.HasOne(d => d.Ciudadano).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.CiudadanoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Ciudadano");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_Incidente_Empleado");

            entity.HasOne(d => d.TipoIncidente).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.TipoIncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Tipo_incidente");

            entity.HasOne(d => d.Ubigeo).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.UbigeoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Ubigeo");
        });

        modelBuilder.Entity<ListaNegra>(entity =>
        {
            entity.ToTable("Lista_negra");

            entity.Property(e => e.ListaNegraId).HasColumnName("Lista_negraID");
            entity.Property(e => e.CiudadanoId).HasColumnName("CiudadanoID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.FechaBloqueo)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_bloqueo");
            entity.Property(e => e.IpBloqueada)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("Ip_bloqueada");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Empleado).WithMany(p => p.ListaNegras)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lista_negra_Empleado");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol");

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoIncidente>(entity =>
        {
            entity.ToTable("Tipo_incidente");

            entity.Property(e => e.TipoIncidenteId).HasColumnName("Tipo_incidenteID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NivelGravedad)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Nivel_Gravedad");
        });

        modelBuilder.Entity<Ubigeo>(entity =>
        {
            entity.ToTable("Ubigeo");

            entity.Property(e => e.UbigeoId).HasColumnName("UbigeoID");
            entity.Property(e => e.CodigoPostal).HasColumnName("Codigo_Postal");
            entity.Property(e => e.Distrito)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Provincia)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Activo).HasDefaultValue(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_registro");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RolId).HasColumnName("RolID");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
