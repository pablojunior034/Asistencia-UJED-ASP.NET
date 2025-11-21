using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Web.Models
{
    public class Docente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        public string? Materia { get; set; }

        public ICollection<Asistencia>? Asistencias { get; set; }

        public string? ApplicationUserId { get; set; }

        public ApplicationUser? Usuario { get; set; }
    }
}

