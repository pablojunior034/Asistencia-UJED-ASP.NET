using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Web.Controllers
{
    [Authorize(Roles = "Docente")]
    public class ClaseActivasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClaseActivasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: ClaseActivas
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            var docente = await _context.Docente
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            if (docente == null)
                return Content("No se encontró el docente asociado a tu cuenta.");

            var clases = await _context.ClasesActivas
                .Where(c => c.DocenteId == docente.Id)
                .ToListAsync();

            return View(clases);
        }

        // GET: ClaseActivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseActiva = await _context.ClasesActivas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claseActiva == null)
            {
                return NotFound();
            }

            return View(claseActiva);
        }

        // GET: ClaseActivas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClaseActivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Materia")] ClaseActiva claseActiva)
        {
            // Obtener usuario actual y su DocenteId
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var docente = await _context.Docente
            .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            // Generar valores antes de validar
            claseActiva.CodigoClase = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            claseActiva.FechaInicio = DateTime.Now;
            claseActiva.FechaExpiracion = DateTime.Now.AddMinutes(15);
            claseActiva.Activa = true;

            // Asignar el docente real (no confiar en el DocenteId enviado por el form)
            claseActiva.DocenteId = docente.Id;

            if (ModelState.IsValid)
            {
                _context.Add(claseActiva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MostrarQR), new { id = claseActiva.Id });
            }

            return View(claseActiva);
        }

        public async Task<IActionResult> MostrarQR(int id)
        {
            var clase = await _context.ClasesActivas.FindAsync(id);
            if (clase == null)
            {
                return NotFound();
            }

            // Generar el QR con el código de clase
            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(clase.CodigoClase, QRCoder.QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCoder.QRCode(qrData);
                using (var qrBitmap = qrCode.GetGraphic(20))
                {
                    using (var ms = new MemoryStream())
                    {
                        qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ViewBag.QRBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            return View(clase);
        }


        // GET: ClaseActivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseActiva = await _context.ClasesActivas.FindAsync(id);
            if (claseActiva == null)
            {
                return NotFound();
            }
            return View(claseActiva);
        }

        // POST: ClaseActivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Materia,DocenteId,CodigoClase,FechaInicio,FechaExpiracion,Activa")] ClaseActiva claseActiva)
        {
            if (id != claseActiva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claseActiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaseActivaExists(claseActiva.Id))
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
            return View(claseActiva);
        }

        // GET: ClaseActivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseActiva = await _context.ClasesActivas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claseActiva == null)
            {
                return NotFound();
            }

            return View(claseActiva);
        }

        // POST: ClaseActivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claseActiva = await _context.ClasesActivas.FindAsync(id);
            if (claseActiva != null)
            {
                _context.ClasesActivas.Remove(claseActiva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaseActivaExists(int id)
        {
            return _context.ClasesActivas.Any(e => e.Id == id);
        }
    }
}
