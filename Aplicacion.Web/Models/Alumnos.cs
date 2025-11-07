using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Web.Models
{
    public class Alumno
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required]
        public string Matricula { get; set; }

        [EmailAddress]
        public string Correo { get; set; }

        // Relación con asistencias
        public ICollection<Asistencia>? Asistencias { get; set; }
    }
}

