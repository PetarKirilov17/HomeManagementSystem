using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeManagementSystem.Data;
using HomeManagementSystem.Data.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HomeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Client")]
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LocationsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Locations.ToListAsync());
            }
            else
            {
                return View(await _context.Locations
                    .Where(l => l.Creator.Id == user.Id)
                    .ToListAsync());
            }
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if (location == null || (location.CreatorId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Id")] Location location)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                location.CreatorId = user.Id;
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (location == null || (location.CreatorId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Address,CreatorId,Id")] Location location)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != location.Id || (location.CreatorId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
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
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if (location == null || (location.CreatorId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound();
            }
            var locationInAssignemnt = await _context.Assignments.Where(a => a.LocationId == id).FirstOrDefaultAsync();
            if (User.IsInRole("Client") && locationInAssignemnt != null)
            {
                return BadRequest("You cannot delete this location because there are assignments on it!");
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin, Client")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
