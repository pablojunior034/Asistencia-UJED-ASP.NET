using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Web.Models
{
    public class Docente
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        public string? Materia { get; set; }

        public ICollection<Asistencia>? Asistencias { get; set; }
    }
}

