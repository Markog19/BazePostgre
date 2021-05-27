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
    public class ŽanrController : Controller
    {
        private readonly DBContext _context;

        public ŽanrController(DBContext context)
        {
            _context = context;
        }

        public ActionResult CreateDocument()

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

            FileStream imageStream = new FileStream("logo.png", FileMode.Open, FileAccess.Read);
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


            var zanrovi = _context.Žanrs
                             .AsNoTracking()
                             .OrderBy(o => o.Id)
                             .ToList();
            ispisi(zanrovi, data);


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
        public void ispisi(List<Žanr> zanrovi, List<object> data)
        {
            foreach (var zanr in zanrovi)
            {
                var pjesme = _context.Pjesmas.AsNoTracking().Where(e => e.ZanrId.Equals(zanr.Id)).ToList();
                var row = new { Naziv = zanr.Ime, Broj_Pjesama = pjesme.Count };
                data.Add(row);



            }
        }


        // GET: Žanr
        public async Task<IActionResult> Index()
        {
            return View(await _context.Žanrs.ToListAsync());
        }

        // GET: Žanr/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var žanr = await _context.Žanrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (žanr == null)
            {
                return NotFound();
            }

            return View(žanr);
        }

        // GET: Žanr/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Žanr/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime")] Žanr žanr)
        {
            if (ModelState.IsValid)
            {
                _context.Add(žanr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(žanr);
        }

        // GET: Žanr/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var žanr = await _context.Žanrs.FindAsync(id);
            if (žanr == null)
            {
                return NotFound();
            }
            return View(žanr);
        }

        // POST: Žanr/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime")] Žanr žanr)
        {
            if (id != žanr.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(žanr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ŽanrExists(žanr.Id))
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
            return View(žanr);
        }

        // GET: Žanr/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var žanr = await _context.Žanrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (žanr == null)
            {
                return NotFound();
            }

            return View(žanr);
        }

        // POST: Žanr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var žanr = await _context.Žanrs.FindAsync(id);
            _context.Žanrs.Remove(žanr);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ŽanrExists(int id)
        {
            return _context.Žanrs.Any(e => e.Id == id);
        }
    }
}
