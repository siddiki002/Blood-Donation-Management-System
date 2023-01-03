using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BDMS.Data;
using BDMS.Models;

namespace BDMS.Controllers
{
    public class BloodCampsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodCampsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BloodCamps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BloodCamps.Include(b => b.Area).Include(b => b.Organization);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BloodCamps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BloodCamps == null)
            {
                return NotFound();
            }

            var bloodCamp = await _context.BloodCamps
                .Include(b => b.Area)
                .Include(b => b.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodCamp == null)
            {
                return NotFound();
            }

            return View(bloodCamp);
        }

        // GET: BloodCamps/Create
        public IActionResult Create()
        {
            ViewData["AreaCode"] = new SelectList(_context.Areas, "Id", "City");
            ViewData["OrgCode"] = new SelectList(_context.Organizations, "Id", "Address");
            return View();
        }

        // POST: BloodCamps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrgCode,AreaCode,StartTime,EndTime,beds")] BloodCamp bloodCamp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bloodCamp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaCode"] = new SelectList(_context.Areas, "Id", "City", bloodCamp.AreaCode);
            ViewData["OrgCode"] = new SelectList(_context.Organizations, "Id", "Address", bloodCamp.OrgCode);
            return View(bloodCamp);
        }

        // GET: BloodCamps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BloodCamps == null)
            {
                return NotFound();
            }

            var bloodCamp = await _context.BloodCamps.FindAsync(id);
            if (bloodCamp == null)
            {
                return NotFound();
            }
            ViewData["AreaCode"] = new SelectList(_context.Areas, "Id", "City", bloodCamp.AreaCode);
            ViewData["OrgCode"] = new SelectList(_context.Organizations, "Id", "Address", bloodCamp.OrgCode);
            return View(bloodCamp);
        }

        // POST: BloodCamps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrgCode,AreaCode,StartTime,EndTime,beds")] BloodCamp bloodCamp)
        {
            if (id != bloodCamp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bloodCamp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BloodCampExists(bloodCamp.Id))
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
            ViewData["AreaCode"] = new SelectList(_context.Areas, "Id", "City", bloodCamp.AreaCode);
            ViewData["OrgCode"] = new SelectList(_context.Organizations, "Id", "Address", bloodCamp.OrgCode);
            return View(bloodCamp);
        }

        // GET: BloodCamps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BloodCamps == null)
            {
                return NotFound();
            }

            var bloodCamp = await _context.BloodCamps
                .Include(b => b.Area)
                .Include(b => b.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodCamp == null)
            {
                return NotFound();
            }

            return View(bloodCamp);
        }

        // POST: BloodCamps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BloodCamps == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BloodCamps'  is null.");
            }
            var bloodCamp = await _context.BloodCamps.FindAsync(id);
            if (bloodCamp != null)
            {
                _context.BloodCamps.Remove(bloodCamp);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BloodCampExists(int id)
        {
          return _context.BloodCamps.Any(e => e.Id == id);
        }
    }
}
