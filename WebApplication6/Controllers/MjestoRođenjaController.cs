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
    public class MjestoRođenjaController : Controller
    {
        private readonly DBContext _context;

        public MjestoRođenjaController(DBContext context)
        {
            _context = context;
        }

        // GET: MjestoRođenja
        public async Task<IActionResult> Index()
        {
            return View(await _context.MjestoRođenjas.ToListAsync());
        }

        // GET: MjestoRođenja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjestoRođenja = await _context.MjestoRođenjas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mjestoRođenja == null)
            {
                return NotFound();
            }

            return View(mjestoRođenja);
        }

        // GET: MjestoRođenja/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MjestoRođenja/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime")] MjestoRođenja mjestoRođenja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mjestoRođenja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mjestoRođenja);
        }

        // GET: MjestoRođenja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjestoRođenja = await _context.MjestoRođenjas.FindAsync(id);
            if (mjestoRođenja == null)
            {
                return NotFound();
            }
            return View(mjestoRođenja);
        }

        // POST: MjestoRođenja/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime")] MjestoRođenja mjestoRođenja)
        {
            if (id != mjestoRođenja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mjestoRođenja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MjestoRođenjaExists(mjestoRođenja.Id))
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
            return View(mjestoRođenja);
        }

        // GET: MjestoRođenja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjestoRođenja = await _context.MjestoRođenjas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mjestoRođenja == null)
            {
                return NotFound();
            }

            return View(mjestoRođenja);
        }

        // POST: MjestoRođenja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mjestoRođenja = await _context.MjestoRođenjas.FindAsync(id);
            _context.MjestoRođenjas.Remove(mjestoRođenja);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MjestoRođenjaExists(int id)
        {
            return _context.MjestoRođenjas.Any(e => e.Id == id);
        }
    }
}
