using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Web.Models;

namespace Aplicacion.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Aplicacion.Web.Models.Alumno> Alumno { get; set; } = default!;
        public DbSet<Aplicacion.Web.Models.Docente> Docente { get; set; } = default!;
        public DbSet<Aplicacion.Web.Models.Asistencia> Asistencia { get; set; } = default!;
    }
}
