using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; // "Docente" o "Alumno"
    }
}