using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using Syncfusion.Pdf.Grid;
using PdfSharp.Pdf.Advanced;
using PdfFont = Syncfusion.Pdf.Graphics.PdfFont;

namespace WebApplication6.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DBContext _context;

        public AlbumsController(DBContext context)
        {
            _context = context;
        }
        public ActionResult CreateDocument(string? filter,string? vrsta)

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
            PdfCompositeField compField = new PdfCompositeField(bigFont, brush,"Izvjestaj");

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
                var albumi = _context.Albums
                                 .AsNoTracking()
                                 .OrderBy(o => o.Id)
                                 .ToList();
                ispisi(albumi, data);
            }
            else
            {
                switch (vrsta)
                {

                    
                    case "Naziv":

                        var albumi = _context.Albums
                                .AsNoTracking()
                                .Where(n => n.Naziv.Equals(filter))
                                .OrderBy(o => o.Id)
                                .ToList();
                        ispisi(albumi, data);
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
        public void ispisi(List<Album> albumi, List<object> data)
        {
            foreach (var album in albumi)
            {
                var baza = _context.IzdavačkaKućas.AsNoTracking()
                                .Where(n => n.Id.Equals(album.IzdavačkaKućaId))
                                .OrderBy(o => o.Id)
                                .ToList();
                var baza1 = _context.Bands.AsNoTracking()
                                .Where(n => n.Id.Equals(album.IzvođačId))
                                .OrderBy(o => o.Id)
                                .ToList();
                var albumK = _context.AlbumKorisniks.AsNoTracking()
                                .Where(n => n.AlbumId.Equals(album.Id))
                                .OrderBy(o => o.Id)
                                .ToList();
                var row = new { Naziv = album.Naziv, Izvodac = baza1[0].Ime, Godina = album.GodinaIzdavanja, Izdavacka_Kuca = baza[0].Ime, Prodaja = albumK.Count };
                data.Add(row);



            }
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Albums.Include(a => a.IzdavačkaKuća).Include(a => a.Izvođač);
            return View(await dBContext.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.IzdavačkaKuća)
                .Include(a => a.Izvođač)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            ViewData["IzdavačkaKućaId"] = new SelectList(_context.IzdavačkaKućas, "Id", "Ime");
            ViewData["IzvođačId"] = new SelectList(_context.Bands, "Id", "Ime");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IzvođačId,IzdavačkaKućaId,GodinaIzdavanja,Id,Naziv")] Album album)
        {

            if (!ModelState.IsValid)
            {

                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IzdavačkaKućaId"] = new SelectList(_context.IzdavačkaKućas, "Id", "Ime", album.IzdavačkaKućaId);
            ViewData["IzvođačId"] = new SelectList(_context.Bands, "Id", "Ime", album.IzvođačId);
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["IzdavačkaKućaId"] = new SelectList(_context.IzdavačkaKućas, "Id", "Ime", album.IzdavačkaKućaId);
            ViewData["IzvođačId"] = new SelectList(_context.Bands, "Id", "Ime", album.IzvođačId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IzvođačId,IzdavačkaKućaId,GodinaIzdavanja,Id,Naziv")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            ViewData["IzdavačkaKućaId"] = new SelectList(_context.IzdavačkaKućas, "Id", "Ime", album.IzdavačkaKućaId);
            ViewData["IzvođačId"] = new SelectList(_context.Bands, "Id", "Ime", album.IzvođačId);
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.IzdavačkaKuća)
                .Include(a => a.Izvođač)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
