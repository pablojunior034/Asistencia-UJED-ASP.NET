using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Aplicacion.Web.Controllers
{
    public class HomeController : Controller
    {
        /*private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var totalAlumnos = _context.Alumno.Count();
            var totalDocentes = _context.Docente.Count();
            var totalAsistencias = _context.Asistencia.Count();

            ViewBag.TotalAlumnos = totalAlumnos;
            ViewBag.TotalDocentes = totalDocentes;
            ViewBag.TotalAsistencias = totalAsistencias;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
