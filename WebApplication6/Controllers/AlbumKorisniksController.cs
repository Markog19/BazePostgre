using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class AlbumKorisniksController : Controller
    {
        private readonly DBContext _context;

        public AlbumKorisniksController(DBContext context)
        {
            _context = context;
           
        }
        
            // GET: AlbumKorisniks
            public async Task<IActionResult> Index()
        {
            var dBContext = _context.AlbumKorisniks.Include(a => a.Album).Include(a => a.Korisnik);
            return View(await dBContext.ToListAsync());
        }

        // GET: AlbumKorisniks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumKorisnik = await _context.AlbumKorisniks
                .Include(a => a.Album)
                .Include(a => a.Korisnik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumKorisnik == null)
            {
                return NotFound();
            }

            return View(albumKorisnik);
        }

        // GET: AlbumKorisniks/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv");
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email");
            return View();
        }

        // POST: AlbumKorisniks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlbumId,KorisnikId,Cijena")] AlbumKorisnik albumKorisnik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(albumKorisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", albumKorisnik.AlbumId);
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", albumKorisnik.KorisnikId);
            return View(albumKorisnik);
        }

        // GET: AlbumKorisniks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumKorisnik = await _context.AlbumKorisniks.FindAsync(id);
            if (albumKorisnik == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", albumKorisnik.AlbumId);
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", albumKorisnik.KorisnikId);
            return View(albumKorisnik);
        }

        // POST: AlbumKorisniks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlbumId,KorisnikId,Cijena")] AlbumKorisnik albumKorisnik)
        {
            if (id != albumKorisnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albumKorisnik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumKorisnikExists(albumKorisnik.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Naziv", albumKorisnik.AlbumId);
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", albumKorisnik.KorisnikId);
            return View(albumKorisnik);
        }

        // GET: AlbumKorisniks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumKorisnik = await _context.AlbumKorisniks
                .Include(a => a.Album)
                .Include(a => a.Korisnik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albumKorisnik == null)
            {
                return NotFound();
            }

            return View(albumKorisnik);
        }

        // POST: AlbumKorisniks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var albumKorisnik = await _context.AlbumKorisniks.FindAsync(id);
            _context.AlbumKorisniks.Remove(albumKorisnik);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumKorisnikExists(int id)
        {
            return _context.AlbumKorisniks.Any(e => e.Id == id);
        }
    }
}
