using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aplicacion.Web.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno Alumno { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? Materia { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Presente";
    }
}

