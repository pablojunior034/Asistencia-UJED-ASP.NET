using System;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Web.Models
{
    public class ClaseActiva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Materia { get; set; }

        [Required]
        public int DocenteId { get; set; }

        public string? CodigoClase { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaExpiracion { get; set; }

        public bool Activa { get; set; }
    }
}

