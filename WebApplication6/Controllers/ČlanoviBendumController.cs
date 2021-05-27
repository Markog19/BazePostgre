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
    public class ČlanoviBendumController : Controller
    {
        private readonly DBContext _context;

        public ČlanoviBendumController(DBContext context)
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
                var Članovi = _context.ČlanoviBenda
                                 .AsNoTracking()
                                 .OrderBy(o => o.Id)
                                 .ToList();
                ispisi(Članovi, data);
            }
            else
            {
                switch (vrsta)
                {


                    case "Instrument":
                        var Članovi = _context.ČlanoviBenda
                                .AsNoTracking()
                                .Where(n => n.Instrument.Equals(filter))
                                .OrderBy(o => o.Id)
                                .ToList();
                        ispisi(Članovi, data);
                        break;
                    case "Band":
                         Članovi = _context.ČlanoviBenda
                                .AsNoTracking()
                                .Where(n => n.Band.Equals(filter))
                                .OrderBy(o => o.Id)
                                .ToList();
                        ispisi(Članovi, data);
                        break;
                    case "Glazbenik":
                        Članovi = _context.ČlanoviBenda
                               .AsNoTracking()
                               .Where(n => n.Glazbenik.Equals(filter))
                               .OrderBy(o => o.Id)
                               .ToList();
                        ispisi(Članovi, data);
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
        public void ispisi(List<ČlanoviBendum> clanovi, List<object> data)
        {
            foreach (var clan in clanovi)
            {
                var Glazbenik = _context.Glazbeniks.AsNoTracking().Where(o => o.Id.Equals(clan.GlazbenikId)).ToList();
                var Band = _context.Bands.AsNoTracking().Where(p => p.Id.Equals(clan.BandId)).ToList();
               
                var row = new { Glazbenik = Glazbenik[0].Ime, Instrument = clan.Instrument, Band = Band[0].Ime, Datum = clan.Datum};
                data.Add(row);



            }
        }


        // GET: ČlanoviBendum
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.ČlanoviBenda.Include(č => č.Band).Include(č => č.Glazbenik);
            return View(await dBContext.ToListAsync());
        }

        // GET: ČlanoviBendum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var članoviBendum = await _context.ČlanoviBenda
                .Include(č => č.Band)
                .Include(č => č.Glazbenik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (članoviBendum == null)
            {
                return NotFound();
            }

            return View(članoviBendum);
        }

        // GET: ČlanoviBendum/Create
        public IActionResult Create()
        {
            ViewData["BandId"] = new SelectList(_context.Bands, "Id", "Ime");
            ViewData["GlazbenikId"] = new SelectList(_context.Glazbeniks, "Id", "Ime");
            return View();
        }

        // POST: ČlanoviBendum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GlazbenikId,BandId,Datum,Instrument")] ČlanoviBendum članoviBendum)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(članoviBendum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BandId"] = new SelectList(_context.Bands, "Id", "Ime", članoviBendum.BandId);
            ViewData["GlazbenikId"] = new SelectList(_context.Glazbeniks, "Id", "Ime", članoviBendum.GlazbenikId);
            return View(članoviBendum);
        }

        // GET: ČlanoviBendum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var članoviBendum = await _context.ČlanoviBenda.FindAsync(id);
            if (članoviBendum == null)
            {
                return NotFound();
            }
            ViewData["BandId"] = new SelectList(_context.Bands, "Id", "Ime", članoviBendum.BandId);
            ViewData["GlazbenikId"] = new SelectList(_context.Glazbeniks, "Id", "Ime", članoviBendum.GlazbenikId);
            return View(članoviBendum);
        }

        // POST: ČlanoviBendum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GlazbenikId,BandId,Datum,Instrument")] ČlanoviBendum članoviBendum)
        {
            if (id != članoviBendum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(članoviBendum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ČlanoviBendumExists(članoviBendum.Id))
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
            ViewData["BandId"] = new SelectList(_context.Bands, "Id", "Ime", članoviBendum.BandId);
            ViewData["GlazbenikId"] = new SelectList(_context.Glazbeniks, "Id", "Ime", članoviBendum.GlazbenikId);
            return View(članoviBendum);
        }

        // GET: ČlanoviBendum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var članoviBendum = await _context.ČlanoviBenda
                .Include(č => č.Band)
                .Include(č => č.Glazbenik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (članoviBendum == null)
            {
                return NotFound();
            }

            return View(članoviBendum);
        }

        // POST: ČlanoviBendum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var članoviBendum = await _context.ČlanoviBenda.FindAsync(id);
            _context.ČlanoviBenda.Remove(članoviBendum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ČlanoviBendumExists(int id)
        {
            return _context.ČlanoviBenda.Any(e => e.Id == id);
        }
    }
}
