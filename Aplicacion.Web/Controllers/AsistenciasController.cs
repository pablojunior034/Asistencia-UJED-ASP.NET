using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class AsistenciasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AsistenciasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ============================================
    // Ver asistencias del alumno loggeado
    // ============================================
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var alumno = await _context.Alumno
            .FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

        if (alumno == null)
            return Content("No se encontró un Alumno asociado a esta cuenta.");

        var asistencias = await _context.Asistencia
            .Include(a => a.ClaseActiva)
            .Where(a => a.AlumnoId == alumno.Id)
            .ToListAsync();

        return View(asistencias);
    }


    // ============================================
    // Registrar asistencia (QR)
    // ============================================
    public async Task<IActionResult> Registrar(string codigo)
    {
        var clase = await _context.ClasesActivas
            .FirstOrDefaultAsync(c => c.CodigoClase == codigo);

        if (clase == null)
            return Content("Clase no encontrada.");

        if (!clase.Activa || clase.FechaExpiracion < DateTime.Now)
            return Content("La clase ya expiró.");

        var user = await _userManager.GetUserAsync(User);
        var alumno = await _context.Alumno
            .FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

        if (alumno == null)
            return Content("No se encontró un alumno asociado a tu cuenta.");

        // Evitar duplicados
        var existe = await _context.Asistencia.AnyAsync(a =>
            a.ClaseActivaId == clase.Id &&
            a.AlumnoId == alumno.Id
        );

        if (existe)
            return Content("Ya registraste tu asistencia.");

        // Registrar asistencia
        var asistencia = new Asistencia
        {
            ClaseActivaId = clase.Id,
            AlumnoId = alumno.Id,
            FechaRegistro = DateTime.Now
        };

        _context.Asistencia.Add(asistencia);
        await _context.SaveChangesAsync();

        return View("Registrar", clase);
    }

    [Authorize(Roles = "Alumno")]
    public IActionResult RegistrarAsistencia()
    {
        return View();
    }

    
}
