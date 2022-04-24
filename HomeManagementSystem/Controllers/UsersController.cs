using HomeManagementSystem.Data.Entites;
using HomeManagementSystem.Models.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;

namespace HomeManagementSystem.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: UsersController
        public ActionResult Index()
        {
            List<AppUserViewModel> users = _userManager.Users
               .Select(item => new AppUserViewModel()
               {
                   Id = item.Id,
                   UserName = item.UserName,
                   Email = item.Email,
                   Role = string.Join(
                       ", ", _userManager.GetRolesAsync(item).Result
                   )
               })
               .ToList();

            return View(users);
        }

        // GET: UsersController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new AppUserViewModel()
            {
                Id = userData.Id,
                UserName = userData.UserName,
                Email = userData.Email,
                Role = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)
            };
            return View(user);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            SelectList options = new SelectList(roles, nameof(IdentityRole.Id), nameof(IdentityRole.Name));
            ViewBag.Roles = options;
            return View(new CreateAppUserViewModel());
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] CreateAppUserViewModel model)
        {
            try
            {
                AppUser user = new AppUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true
                };

                var role = await _roleManager.FindByIdAsync(model.Role);
                IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, role.Name).Wait();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new CreateAppUserViewModel()
            {
                UserName = userData.UserName,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Password = userData.PasswordHash,
                Email = userData.Email,
                Role = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)

            };
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            SelectList options = new SelectList(roles, nameof(IdentityRole.Id), nameof(IdentityRole.Name));
            ViewBag.Roles = options;
            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, CreateAppUserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var role = await _roleManager.FindByIdAsync(model.Role);
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                if (model.Password != "")
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, model.Password);
                }

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //    IdentityResult res2 = await _userManager.RemoveFromRolesAsync(user, new List<string>()
                    //{
                    //    "Admin", "Housekeeper", "Client"
                    //});
                    var roles = new List<string>() { "Admin", "Client", "Housekeeper" };
                    bool succeded = false;
                    foreach (var r in roles)
                    {
                        IdentityResult res2 = await _userManager.RemoveFromRoleAsync(user, r);
                        if (res2.Succeeded)
                        {
                            succeded = true;
                        }
                    }
                    if (succeded)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new AppUserViewModel()
            {
                Id = userData.Id,
                UserName = userData.UserName,
                Email = userData.Email,
                Role = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)
            };
            return View(user);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, AppUserViewModel model)
        {
            try
            {
                IdentityResult identityResult = await _userManager.DeleteAsync(
                    await _userManager.FindByIdAsync(model.Id));
                if (identityResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
