using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aplicacion.Web.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        // FK al Alumno (suponiendo que ya existe)
        [Required]
        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; }

        // FK al Docente (opcional, si lo guardas)
        public int? DocenteId { get; set; }
        public Docente? Docente { get; set; }

        // FK a la clase activa (nuevo)
        [Required]
        public int ClaseActivaId { get; set; }
        public ClaseActiva ClaseActiva { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Estado/Tipo de asistencia (Presente, Retardo, etc.)
        public string Estado { get; set; } = "Presente";
    }
}
