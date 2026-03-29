using GisorSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISTEMA_INTEGRAL_GISOR.Models;

namespace GisorSystem.Controllers
{
    [AllowAnonymous] // Acceso libre (la validación se hace dentro)
    public class HomeController : Controller
    {
        private readonly GestorIncidenteService _gestorIncidente;

        public HomeController(GestorIncidenteService gestorIncidente)
        {
            _gestorIncidente = gestorIncidente;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSOS(double lat, double lon)
        {
            // 1. Validamos estado de sesión
            bool esAnonimo = !User.Identity.IsAuthenticated;
            int ciudadanoId = 1; // Valor por defecto: ID 1 (Usuario Anónimo en BD)

            // 2. LÓGICA DE COOKIES / SESIÓN
            if (!esAnonimo)
            {
                // Si está autenticado, extraemos el ID directamente de la Cookie (Claim)
                // Esto evita errores porque usamos el ID exacto que guardaste al loguearte
                var claimCiudadano = User.FindFirst("CiudadanoID");

                if (claimCiudadano != null)
                {
                    int.TryParse(claimCiudadano.Value, out ciudadanoId);
                }
                else
                {
                    // Fallback de seguridad: Si por alguna razón la cookie está corrupta o vieja,
                    // forzamos Logout para que se loguee bien de nuevo, o usamos ID 1.
                    // Para este prototipo, usaremos ID 1 para que no falle.
                    ciudadanoId = 1;
                }
            }

            // 3. Control de Spam para Anónimos (Cookie local de conteo)
            int contadorAnonimo = 0;
            if (esAnonimo)
            {
                var cookieCount = Request.Cookies["SOS_Count"];
                int.TryParse(cookieCount, out contadorAnonimo);
            }

            // 4. Crear el Objeto Evento
            var evento = new EventoSo
            {
                Latitud = (decimal)lat,
                Longitud = (decimal)lon,
                CiudadanoId = ciudadanoId, // <--- Aquí va el ID validado desde la Cookie/Defecto
                // FechaHora se asigna en el Gestor
            };

            // 5. Procesar
            bool permitido = await _gestorIncidente.ProcesarAlertaSOS(evento, contadorAnonimo, esAnonimo);

            if (!permitido)
            {
                return Json(new { success = false, message = "Límite de alertas anónimas excedido (Máx 2). Regístrate para continuar." });
            }

            // 6. Actualizar contador en Cookie del navegador (Solo anónimos)
            if (esAnonimo)
            {
                Response.Cookies.Append("SOS_Count", (contadorAnonimo + 1).ToString(),
                    new CookieOptions { Expires = DateTime.Now.AddHours(24) });
            }

            return Json(new { success = true, message = "¡ALERTA ENVIADA! Tu ubicación ha sido compartida con las autoridades." });
        }

        public IActionResult Anonimo()
        {
            return RedirectToAction("Index");
        }
    }
}