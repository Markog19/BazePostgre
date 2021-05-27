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
    public class PjesmaKorisniksController : Controller
    {
        private readonly DBContext _context;

        public PjesmaKorisniksController(DBContext context)
        {
            _context = context;
        }

        // GET: PjesmaKorisniks
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.PjesmaKorisniks.Include(p => p.Korisnik).Include(p => p.Pjesma);
            return View(await dBContext.ToListAsync());
        }

        // GET: PjesmaKorisniks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesmaKorisnik = await _context.PjesmaKorisniks
                .Include(p => p.Korisnik)
                .Include(p => p.Pjesma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pjesmaKorisnik == null)
            {
                return NotFound();
            }

            return View(pjesmaKorisnik);
        }

        // GET: PjesmaKorisniks/Create
        public IActionResult Create()
        {
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email");
            ViewData["PjesmaId"] = new SelectList(_context.Pjesmas, "Id", "Naziv");
            return View();
        }

        // POST: PjesmaKorisniks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PjesmaId,KorisnikId,Cijena")] PjesmaKorisnik pjesmaKorisnik)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(pjesmaKorisnik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", pjesmaKorisnik.KorisnikId);
            ViewData["PjesmaId"] = new SelectList(_context.Pjesmas, "Id", "Naziv", pjesmaKorisnik.PjesmaId);
            return View(pjesmaKorisnik);
        }

        // GET: PjesmaKorisniks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesmaKorisnik = await _context.PjesmaKorisniks.FindAsync(id);
            if (pjesmaKorisnik == null)
            {
                return NotFound();
            }
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", pjesmaKorisnik.KorisnikId);
            ViewData["PjesmaId"] = new SelectList(_context.Pjesmas, "Id", "Naziv", pjesmaKorisnik.PjesmaId);
            return View(pjesmaKorisnik);
        }

        // POST: PjesmaKorisniks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PjesmaId,KorisnikId,Cijena")] PjesmaKorisnik pjesmaKorisnik)
        {
            if (id != pjesmaKorisnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pjesmaKorisnik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PjesmaKorisnikExists(pjesmaKorisnik.Id))
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
            ViewData["KorisnikId"] = new SelectList(_context.Korisniks, "Id", "Email", pjesmaKorisnik.KorisnikId);
            ViewData["PjesmaId"] = new SelectList(_context.Pjesmas, "Id", "Naziv", pjesmaKorisnik.PjesmaId);
            return View(pjesmaKorisnik);
        }

        // GET: PjesmaKorisniks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pjesmaKorisnik = await _context.PjesmaKorisniks
                .Include(p => p.Korisnik)
                .Include(p => p.Pjesma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pjesmaKorisnik == null)
            {
                return NotFound();
            }

            return View(pjesmaKorisnik);
        }

        // POST: PjesmaKorisniks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pjesmaKorisnik = await _context.PjesmaKorisniks.FindAsync(id);
            _context.PjesmaKorisniks.Remove(pjesmaKorisnik);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PjesmaKorisnikExists(int id)
        {
            return _context.PjesmaKorisniks.Any(e => e.Id == id);
        }
    }
}
