using SISTEMA_INTEGRAL_GISOR.Models;
using GisorSystem.Repositories;

namespace GisorSystem.Services
{
    public class GestorUsuarioService
    {
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Ciudadano> _ciudadanoRepo;
        private readonly IRepository<ListaNegra> _listaNegraRepo; // <--- NUEVO
        private readonly ValidadorSistema _validador;

        public GestorUsuarioService(IRepository<Usuario> usuarioRepo,
                                    IRepository<Ciudadano> ciudadanoRepo,
                                    IRepository<ListaNegra> listaNegraRepo, // <--- INYECCIÓN
                                    ValidadorSistema validador)
        {
            _usuarioRepo = usuarioRepo;
            _ciudadanoRepo = ciudadanoRepo;
            _listaNegraRepo = listaNegraRepo; // <--- ASIGNACIÓN
            _validador = validador;
        }

        public async Task<Usuario> AutenticarUsuario(string correo, string password)
        {
            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correo && u.Password == password)).FirstOrDefault();

            if (usuario == null) return null;

            // VALIDACIÓN DE LISTA NEGRA
            // Si es un ciudadano, verificamos si está bloqueado
            if (usuario.RolId == 2)
            {
                var ciudadano = (await _ciudadanoRepo.FindAsync(c => c.UsuarioId == usuario.UsuarioId)).FirstOrDefault();
                if (ciudadano != null)
                {
                    // Buscamos si existe un registro en Lista Negra para este ciudadano
                    var bloqueo = (await _listaNegraRepo.FindAsync(l => l.CiudadanoId == ciudadano.CiudadanoId)).FirstOrDefault();
                    if (bloqueo != null)
                    {
                        // Si está bloqueado, impedimos el login (retornamos null o podríamos lanzar excepción)
                        // Para el prototipo, retornamos null para que salga "Credenciales incorrectas o usuario bloqueado"
                        return null;
                    }
                }
            }

            // Validar que esté activo (aprobado por moderador)
            if (usuario.Activo != true) return null;

            return usuario;
        }

        // Método para Bloquear
        public async Task BloquearCiudadano(int ciudadanoId, int empleadoId, string motivo)
        {
            var bloqueo = new ListaNegra
            {
                CiudadanoId = ciudadanoId,
                EmpleadoId = empleadoId,
                Motivo = motivo,
                FechaBloqueo = DateTime.Now,
                IpBloqueada = "0.0.0.0" // Simulamos IP ya que no la estamos rastreando en este prototipo
            };

            await _listaNegraRepo.AddAsync(bloqueo);

            // Opcional: Desactivar el usuario también
            var ciudadano = await _ciudadanoRepo.GetByIdAsync(ciudadanoId);
            if (ciudadano != null)
            {
                var usuario = await _usuarioRepo.GetByIdAsync(ciudadano.UsuarioId);
                if (usuario != null)
                {
                    usuario.Activo = false;
                    await _usuarioRepo.UpdateAsync(usuario);
                }
            }
        }

        public async Task<bool> RegistrarCiudadano(Usuario usuario, Ciudadano ciudadano)
        {
            if (!_validador.EsDniValido(ciudadano.Dni)) return false;

            usuario.FechaRegistro = DateTime.Now;
            usuario.Rol = null;
            if (usuario.RolId == 0) usuario.RolId = 2;

            await _usuarioRepo.AddAsync(usuario);

            ciudadano.UsuarioId = usuario.UsuarioId;
            ciudadano.Usuario = null;

            await _ciudadanoRepo.AddAsync(ciudadano);

            return true;
        }

        public async Task<bool> ValidarExistenciaCorreo(string correo)
        {
            var users = await _usuarioRepo.FindAsync(u => u.Correo == correo);
            return users.Any();
        }
    }
}