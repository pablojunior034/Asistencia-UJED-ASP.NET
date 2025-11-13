using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Web.Data;
using Aplicacion.Web.Models;
using QRCoder;

namespace Aplicacion.Web.Controllers
{
    public class ClaseActivasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaseActivasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClaseActivas
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClasesActivas.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Materia,DocenteId")] ClaseActiva claseActiva)
        {
            Console.WriteLine("Entro al metodo Create POST");
            if (ModelState.IsValid)
            {
                // Generar código y fechas
                claseActiva.CodigoClase = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                claseActiva.FechaInicio = DateTime.Now;
                claseActiva.FechaExpiracion = DateTime.Now.AddMinutes(15);
                claseActiva.Activa = true;

                _context.Add(claseActiva);
                await _context.SaveChangesAsync();

                Console.WriteLine("Clase guardada correctamente, ID: " + claseActiva.Id);
                // Redirigir a la acción que muestra el QR
                return RedirectToAction(nameof(MostrarQR), new { id = claseActiva.Id });
            }
            else
            {
                Console.WriteLine(">>> ModelState inválido <<<");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
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
