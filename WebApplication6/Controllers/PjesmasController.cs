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
    public class PjesmasController : Controller
    {
        private readonly DBContext _context;

        public PjesmasController(DBContext context)
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
                var Pjesme = _context.Pjesmas
                                 .AsNoTracking()
                                 .OrderBy(o => o.Id)
                                 .ToList();
                ispisi(Pjesme, data);
            }
            else
            {
                switch (vrsta)
                {


                    case "Naziv":
                        var Pjesme = _context.Pjesmas
                                .AsNoTracking()
                                .Where(n => n.Naziv.Equals(filter))
                                .OrderBy(o => o.Id)
                                .ToList();
                        ispisi(Pjesme, data);
                        break;
                    case "Album":
                        var album = _context.Albums.AsNoTracking().Where(o => o.Naziv.Equals(filter)).ToList();
                        Pjesme = _context.Pjesmas.AsNoTracking().Where(p => p.AlbumId.Equals(album[0].Id)).ToList();
                        ispisi(Pjesme, data);
                        break;
                    case "Žanr":
                        var zanr = _context.Žanrs.AsNoTracking().Where(o => o.Ime.Equals(filter)).ToList();
                        Pjesme = _context.Pjesmas.AsNoTracking().Where(p => p.AlbumId.Equals(zanr[0].Id)).ToList();

                        ispisi(Pjesme, data);
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
        public void ispisi(List<Pjesma> Pjesme, List<object> data)
        {
            foreach (var Pjesma in Pjesme)
            {
                var album = _context.Albums.AsNoTracking().Where(o => o.Id.Equals(Pjesma.AlbumId)).ToList();
                var zanr = _context.Žanrs.AsNoTracking().Where(o => o.Id.Equals(Pjesma.ZanrId)).ToList();
                var prodane = _context.PjesmaKorisniks.AsNoTracking().Where(e => e.PjesmaId.Equals(Pjesma.Id)).ToList();

                var row = new { Naziv = Pjesma.Naziv, Datum = Pjesma.GodinaIzdavanja, Album = album[0].Naziv, Zanr = zanr[0].Ime, Broj_Prodanih = prodane.Count  };
                data.Add(row);



            }
        }


        // GET: Pjesmas
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Pjesmas.Include(p => p.Album).Include(p => p.Zanr);
            return View(await dBContext.ToListAsync());
        }

        // GET: Pjesmas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesma = await _context.Pjesmas
                .Include(p => p.Album)
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pjesma == null)
            {
                return NotFound();
            }

            return View(pjesma);
        }

        // GET: Pjesmas/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv");
            ViewData["ZanrId"] = new SelectList(_context.Žanrs, "Id", "Ime");
            return View();
        }

        // POST: Pjesmas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,AlbumId,ZanrId,GodinaIzdavanja")] Pjesma pjesma)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(pjesma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", pjesma.AlbumId);
            ViewData["ZanrId"] = new SelectList(_context.Žanrs, "Id", "Ime", pjesma.ZanrId);
            return View(pjesma);
        }

        // GET: Pjesmas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesma = await _context.Pjesmas.FindAsync(id);
            if (pjesma == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", pjesma.AlbumId);
            ViewData["ZanrId"] = new SelectList(_context.Žanrs, "Id", "Ime", pjesma.ZanrId);
            return View(pjesma);
        }

        // POST: Pjesmas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,AlbumId,ZanrId,GodinaIzdavanja")] Pjesma pjesma)
        {
            if (id != pjesma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pjesma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PjesmaExists(pjesma.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", pjesma.AlbumId);
            ViewData["ZanrId"] = new SelectList(_context.Žanrs, "Id", "Ime", pjesma.ZanrId);
            return View(pjesma);
        }

        // GET: Pjesmas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesma = await _context.Pjesmas
                .Include(p => p.Album)
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pjesma == null)
            {
                return NotFound();
            }

            return View(pjesma);
        }

        // POST: Pjesmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pjesma = await _context.Pjesmas.FindAsync(id);
            _context.Pjesmas.Remove(pjesma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PjesmaExists(int id)
        {
            return _context.Pjesmas.Any(e => e.Id == id);
        }
    }
}
