using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class KorisniksController : Controller
    {
        private readonly DBContext _context;

        public KorisniksController(DBContext context)
        {
            _context = context;
        }

        public ActionResult CreateDocument(string? filter, string? vrsta)

        {
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            PdfFont bigFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

            PdfBrush brush = new PdfSolidBrush(Syncfusion.Drawing.Color.Black);
            PdfDocument doc = new PdfDocument();
            //Add a page to the document.
            PdfPage page = doc.Pages.Add();
            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;
            PdfGrid pdfGrid = new PdfGrid();

            //Load the image as stream.

            System.IO.FileStream imageStream = new FileStream("logo.png", FileMode.Open, FileAccess.Read);
            PdfBitmap image = new PdfBitmap(imageStream);
            graphics.DrawImage(image, 200, 200);

            //Draw the image
            Syncfusion.Drawing.RectangleF bounds = new Syncfusion.Drawing.RectangleF(0, 0, doc.Pages[0].GetClientSize().Width, 50);
            PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
            PdfCompositeField compField = new PdfCompositeField(bigFont, brush, "Izvjestaj");

            compField.Draw(header.Graphics, new Syncfusion.Drawing.PointF(120, 0));

            //Add the header at the top.

            doc.Template.Top = header;
            //Save the PDF document to stream
            //Create a Page template that can be used as footer.


            PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);



            //Create page number field.

            PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);

            //Create page count field.

            PdfPageCountField count = new PdfPageCountField(font, brush);

            //Add the fields in composite fields.

            PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Stranica {0} od {1}", pageNumber, count);

            compositeField.Bounds = footer.Bounds;

            //Draw the composite field in footer.

            compositeField.Draw(footer.Graphics, new Syncfusion.Drawing.PointF(450, 40));
            //Add the footer template at the bottom.

            doc.Template.Bottom = footer;

            MemoryStream stream = new MemoryStream();
            //Add values to list
            List<object> data = new List<object>();

            if (String.IsNullOrEmpty(filter))
            {
                var Korisnici = _context.Korisniks
                                 .AsNoTracking()
                                 .OrderBy(o => o.Id)
                                 .ToList();
                ispisi(Korisnici, data);
            }
            else
            {
                switch (vrsta)
                {


                    case "Ime":
                        var Korisnici = _context.Korisniks
                                .AsNoTracking()
                                .Where(n => n.Ime.Equals(filter))
                                .OrderBy(o => o.Id)
                                .ToList();
                        ispisi(Korisnici, data);
                        break;
                    case "Prezime":
                        Korisnici = _context.Korisniks
                               .AsNoTracking()
                               .Where(n => n.Prezime.Equals(filter))
                               .OrderBy(o => o.Id)
                               .ToList();
                        ispisi(Korisnici, data);
                        break;
                    case "Mjesto":
                        var Mjesto = _context.MjestoRođenjas.AsNoTracking().Where(o => o.Ime.Equals(filter)).ToList();
                        Korisnici = _context.Korisniks
                               .AsNoTracking()
                               .Where(n => n.MjestoId.Equals(Mjesto[0].Id))
                               .OrderBy(o => o.Id)
                               .ToList();
                        ispisi(Korisnici, data);
                        break;


                }
            }
            //Add list to IEnumerable
            IEnumerable<object> dataTable = data;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(50, 50));
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.
            string fileName = "Output.pdf";

            return File(stream, contentType, fileName);
        }
        public void ispisi(List<Korisnik> Korisnici, List<object> data)
        {
            foreach (var Korisnik in Korisnici)
            {
                var Mjesto = _context.MjestoRođenjas.AsNoTracking().Where(o => o.Id.Equals(Korisnik.MjestoId)).ToList();
                var row = new { Ime = Korisnik.Ime, Prezime = Korisnik.Prezime, Mjesto = Mjesto[0].Ime, Email = Korisnik.Email };
                data.Add(row);



            }
        }

        // GET: Korisniks
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Korisniks.Include(k => k.Mjesto);
            return View(await dBContext.ToListAsync());
        }

        // GET: Korisniks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _context.Korisniks
                .Include(k => k.Mjesto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        // GET: Korisniks/Create
        public IActionResult Create()
        {
            ViewData["MjestoId"] = new SelectList(_context.MjestoRođenjas, "Id", "Ime");
            return View();
        }

        // POST: Korisniks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Email,MjestoId")] Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(korisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MjestoId"] = new SelectList(_context.MjestoRođenjas, "Id", "Ime", korisnik.MjestoId);
            return View(korisnik);
        }

        // GET: Korisniks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _context.Korisniks.FindAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }
            ViewData["MjestoId"] = new SelectList(_context.MjestoRođenjas, "Id", "Ime", korisnik.MjestoId);
            return View(korisnik);
        }

        // POST: Korisniks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Prezime,Email,MjestoId")] Korisnik korisnik)
        {
            if (id != korisnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(korisnik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorisnikExists(korisnik.Id))
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
            ViewData["MjestoId"] = new SelectList(_context.MjestoRođenjas, "Id", "Ime", korisnik.MjestoId);
            return View(korisnik);
        }

        // GET: Korisniks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _context.Korisniks
                .Include(k => k.Mjesto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        // POST: Korisniks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnik = await _context.Korisniks.FindAsync(id);
            _context.Korisniks.Remove(korisnik);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KorisnikExists(int id)
        {
            return _context.Korisniks.Any(e => e.Id == id);
        }
    }
}
