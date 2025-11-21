using Aplicacion.Web.Data;
using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using Aplicacion.Web.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> MisAsistencias(int? claseId)
    {
        var user = await _userManager.GetUserAsync(User);

        // Buscar el docente asociado
        var docente = await _context.Docente
            .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

        if (docente == null)
            return Content("No se encontró un Docente asociado a esta cuenta.");

        // Obtener clases del docente
        var clases = await _context.ClasesActivas
            .Where(c => c.DocenteId == docente.Id)
            .ToListAsync();

        ViewBag.Clases = clases;
        ViewBag.ClaseSeleccionada = claseId;

        // Base: asistencias de sus clases
        var query = _context.Asistencia
            .Include(a => a.Alumno)
            .Include(a => a.ClaseActiva)
            .Where(a => clases.Select(c => c.Id).Contains(a.ClaseActivaId))
            .AsQueryable();

        // FILTRO SI SE ELIGE UNA CLASE
        if (claseId.HasValue)
        {
            query = query.Where(a => a.ClaseActivaId == claseId.Value);
        }

        var asistencias = await query
            .OrderByDescending(a => a.FechaRegistro)
            .ToListAsync();

        return View(asistencias);
    }


    public async Task<IActionResult> ExportarExcel(int? claseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

        if (docente == null)
            return Content("No se encontró un Docente asociado.");

        var clases = await _context.ClasesActivas
            .Where(c => c.DocenteId == docente.Id)
            .ToListAsync();

        var query = _context.Asistencia
            .Include(a => a.Alumno)
            .Include(a => a.ClaseActiva)
            .Where(a => clases.Select(x => x.Id).Contains(a.ClaseActivaId));

        if (claseId.HasValue)
            query = query.Where(a => a.ClaseActivaId == claseId.Value);

        var asistencias = await query.ToListAsync();

        // Crear archivo Excel
        using (var workbook = new ClosedXML.Excel.XLWorkbook())
        {
            var hoja = workbook.Worksheets.Add("Asistencias");

            hoja.Cell(1, 1).Value = "Clase";
            hoja.Cell(1, 2).Value = "Alumno";
            hoja.Cell(1, 3).Value = "Fecha Registro";

            int row = 2;
            foreach (var a in asistencias)
            {
                hoja.Cell(row, 1).Value = a.ClaseActiva.Materia;
                hoja.Cell(row, 2).Value = a.Alumno.Nombre;
                hoja.Cell(row, 3).Value = a.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Asistencias.xlsx");
            }
        }
    }

    public async Task<IActionResult> ExportarPDF(int? claseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

        if (docente == null)
            return Content("No se encontró un Docente asociado.");

        var clases = await _context.ClasesActivas
            .Where(c => c.DocenteId == docente.Id)
            .ToListAsync();

        var query = _context.Asistencia
            .Include(a => a.Alumno)
            .Include(a => a.ClaseActiva)
            .Where(a => clases.Select(x => x.Id).Contains(a.ClaseActivaId));

        if (claseId.HasValue)
            query = query.Where(a => a.ClaseActivaId == claseId.Value);

        var asistencias = await query.ToListAsync();

        using (var stream = new MemoryStream())
        {
            var doc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            doc.Add(new Paragraph("Reporte de Asistencias"));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("-----------------------------------------------------"));
            doc.Add(new Paragraph(" "));

            foreach (var a in asistencias)
            {
                doc.Add(new Paragraph(
                    $"{a.ClaseActiva.Materia} - {a.Alumno.Nombre} - {a.FechaRegistro:dd/MM/yyyy HH:mm}"
                ));
            }

            doc.Close();

            return File(
                stream.ToArray(),
                "application/pdf",
                "Asistencias.pdf");
        }
    }


}
