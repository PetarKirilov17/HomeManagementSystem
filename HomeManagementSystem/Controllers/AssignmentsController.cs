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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace HomeManagementSystem.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AssignmentsController(ApplicationDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Assignments
        public async Task<IActionResult> Index(string searchString, string searchString2, Status status)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilter2"] = searchString2;

            if (!String.IsNullOrEmpty(searchString2))
            {
                return View(await _context.Assignments.Where(a => a.Description == searchString2).ToListAsync());
            }

            if (User.IsInRole("Admin"))
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    return View(await _context.Assignments.Where(a => a.Name == searchString).ToListAsync());
                }
                return View(await _context.Assignments.ToListAsync());
            }
            else if (User.IsInRole("Client"))
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    return View(await _context.Assignments.Where(a => a.Name == searchString && a.CreatorId == user.Id).ToListAsync());
                }
                return View(await _context.Assignments.Where(a => a.CreatorId == user.Id).ToListAsync());
            }
            else // Housekeeper
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    return View(await _context.Assignments.Where(a => a.Name == searchString && a.AssignedHousekeeperId == user.Id).ToListAsync());
                }
                return View(await _context.Assignments.Where(a => a.AssignedHousekeeperId == user.Id).ToListAsync());
            }
            

        }

        // GET: Assignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: Assignments/Create
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Location> possibleLocation = new List<Location>();
            if (User.IsInRole("Admin"))
            {
                possibleLocation = await _context.Locations.ToListAsync();
            }
            else if (User.IsInRole("Client"))
            {
                possibleLocation = await _context.Locations.Where(l => l.CreatorId == user.Id).ToListAsync();
            }

            SelectList options = new SelectList(possibleLocation, nameof(Location.Id), nameof(Location.Name));
            ViewBag.Locations = options;
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,DeadLine,CompletedTask,DateOfCompletion,CategoryOfAssignment,Id,LocationId")] Assignment assignment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                assignment.StatusOfAssignent = Status.Waiting;
                assignment.Creator = user;
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var assignment = _context.Assignments.Find(id);
            if (id != assignment.Id)
            {
                return NotFound();
            }
            if (assignment.StatusOfAssignent == Status.Waiting)
            {
                assignment.StatusOfAssignent = Status.Declined;
                _context.Update(assignment);
                await _context.SaveChangesAsync();

            }
            else
            {
                return BadRequest("This assignment cannot be declined, because it is not with status Waiting!");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var assignment = _context.Assignments.Find(id);
            if (id != assignment.Id)
            {
                return NotFound();
            }
            if (assignment.StatusOfAssignent == Status.ForView)
            {
                assignment.StatusOfAssignent = Status.Completed;
                _context.Update(assignment);
                await _context.SaveChangesAsync();

            }
            else
            {
                return BadRequest("This assignment cannot be completed, because it is not with status ForView!");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MakeForView(int? id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment.DateOfCompletion != default(DateTime))
            {
                return Content("This task is already completed!");
            }
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeForView(int id, [Bind("Id,Name,Description,DeadLine,CompletedTask,DateOfCompletion,CategoryOfAssignment,StatusOfAssignent,LocationId,CreatorId,AssignedHousekeeperId")] Assignment assignment, List<IFormFile> files)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string filePath = "";
                var formFile = files[0];
                if (formFile.Length > 0)
                {
                    // this line is needed for the proper creation of the file int wwwroot/images
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    filePath = Path.Combine(uploadsFolder, formFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                assignment.DateOfCompletion = DateTime.Now;
                assignment.StatusOfAssignent = Status.ForView;
                assignment.CompletedTask = formFile.FileName;
                _context.Update(assignment);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignToHousekeeper(int? id)
        {
            List<AppUser> usersRoles = (await _userManager.GetUsersInRoleAsync("Housekeeper")).ToList();
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            SelectList options = new SelectList(usersRoles, nameof(AppUser.Id), nameof(AppUser.UserName));
            ViewBag.Housekeepers = options;



            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignToHousekeeper(int id, [Bind("Id,Name,Description,DeadLine,CompletedTask,DateOfCompletion,CategoryOfAssignment,StatusOfAssignent,LocationId,CreatorId,AssignedHousekeeperId")] Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(assignment.AssignedHousekeeperId);

            if (ModelState.IsValid)
            {
                try
                {
                    assignment.AssignedHousekeeper = user;
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
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
            return View(assignment);
        }


        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var user = await _userManager.GetUserAsync(User);
            List<Location> possibleLocation = new List<Location>();
            if (User.IsInRole("Admin"))
            {
                possibleLocation = await _context.Locations.ToListAsync();
            }
            else if (User.IsInRole("Client"))
            {
                possibleLocation = await _context.Locations.Where(l => l.CreatorId == user.Id).ToListAsync();
            }
            SelectList options = new SelectList(possibleLocation, nameof(Location.Id), nameof(Location.Name));
            ViewBag.Locations = options;
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") || (User.IsInRole("Client") && assignment.StatusOfAssignent == Status.Waiting))
            {
                return View(assignment);
            }
            else
            {
                return BadRequest("This assignment cannot be edited, because it is not with status Waiting!");
            }
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DeadLine,CompletedTask,DateOfCompletion,CategoryOfAssignment,StatusOfAssignent,Id,LocationId,CreatorId,AssignedHousekeeperId")] Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
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
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
