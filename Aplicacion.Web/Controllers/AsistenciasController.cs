using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Web.Data;
using Aplicacion.Web.Models;

namespace Aplicacion.Web.Controllers
{
    public class AsistenciasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Asistencias
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Asistencia.Include(a => a.Alumno).Include(a => a.ClaseActiva).Include(a => a.Docente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Asistencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Alumno)
                .Include(a => a.ClaseActiva)
                .Include(a => a.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // GET: Asistencias/Create
        public IActionResult Create()
        {
            ViewData["AlumnoId"] = new SelectList(_context.Alumno, "Id", "Matricula");
            ViewData["ClaseActivaId"] = new SelectList(_context.ClasesActivas, "Id", "DocenteId");
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Correo");
            return View();
        }

        // POST: Asistencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlumnoId,DocenteId,ClaseActivaId,FechaRegistro,Estado")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asistencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlumnoId"] = new SelectList(_context.Alumno, "Id", "Matricula", asistencia.AlumnoId);
            ViewData["ClaseActivaId"] = new SelectList(_context.ClasesActivas, "Id", "DocenteId", asistencia.ClaseActivaId);
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Correo", asistencia.DocenteId);
            return View(asistencia);
        }

        // GET: Asistencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            ViewData["AlumnoId"] = new SelectList(_context.Alumno, "Id", "Matricula", asistencia.AlumnoId);
            ViewData["ClaseActivaId"] = new SelectList(_context.ClasesActivas, "Id", "DocenteId", asistencia.ClaseActivaId);
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Correo", asistencia.DocenteId);
            return View(asistencia);
        }

        // POST: Asistencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlumnoId,DocenteId,ClaseActivaId,FechaRegistro,Estado")] Asistencia asistencia)
        {
            if (id != asistencia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciaExists(asistencia.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlumnoId"] = new SelectList(_context.Alumno, "Id", "Matricula", asistencia.AlumnoId);
            ViewData["ClaseActivaId"] = new SelectList(_context.ClasesActivas, "Id", "DocenteId", asistencia.ClaseActivaId);
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Correo", asistencia.DocenteId);
            return View(asistencia);
        }

        // GET: Asistencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Alumno)
                .Include(a => a.ClaseActiva)
                .Include(a => a.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciaExists(int id)
        {
            return _context.Asistencia.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Registrar(string codigo)
        {
            var clase = await _context.ClasesActivas.FirstOrDefaultAsync(c => c.CodigoClase == codigo && c.Activa);
            if (clase == null)
            {
                return View("Mensaje", $"❌ Clase no encontrada o expirada ({codigo})");
            }

            if (DateTime.Now > clase.FechaExpiracion)
            {
                clase.Activa = false;
                _context.ClasesActivas.Update(clase);
                await _context.SaveChangesAsync();
                return View("Mensaje", $"⚠️ Esta clase ha expirado. El código QR ya no es válido.");
            }

            int alumnoDePruebaId = 1; // temporal hasta implementar Identity

            var asistenciaExistente = await _context.Asistencia
                .FirstOrDefaultAsync(a => a.AlumnoId == alumnoDePruebaId && a.ClaseActivaId == clase.Id);

            if (asistenciaExistente != null)
            {
                ViewBag.Materia = clase.Materia;
                return View("Registrar", asistenciaExistente);
            }

            var asistencia = new Asistencia
            {
                AlumnoId = alumnoDePruebaId,
                ClaseActivaId = clase.Id,
                FechaRegistro = DateTime.Now
            };

            _context.Asistencia.Add(asistencia);
            await _context.SaveChangesAsync();

            ViewBag.Materia = clase.Materia;
            return View("Registrar", asistencia);
        }

    }
}
