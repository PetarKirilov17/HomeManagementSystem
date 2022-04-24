using HomeManagementSystem.Data.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeManagementSystem.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedAdmin(userManager);
            SeedClient(userManager);
            SeedHousekeeper(userManager);
        }

        static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.FindByNameAsync("Admin").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Admin"
                }).Wait();
            }

            if (roleManager.FindByNameAsync("Client").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Client"
                }).Wait();
            }

            if (roleManager.FindByNameAsync("Housekeeper").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Housekeeper"
                }).Wait();
            }
        }

        static void SeedAdmin(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@abv.bg",
                    //FirstName = "Admin",
                    //LastName = "Admin",
                    //EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "adminpass").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        static void SeedClient(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("client").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "client",
                    Email = "client@abv.bg",
                    //FirstName = "Client",
                    //LastName = "Client",
                    //EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "clientpass").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Client").Wait();
                }
            }
        }

        static void SeedHousekeeper(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("housekeeper").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "housekeeper",
                    Email = "housekeeper@abv.bg",
                    //FirstName = "Housekeeper",
                    //LastName = "Housekeeper",
                    //EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "housekeeperpass").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Housekeeper").Wait();
                }
            }
        }
    }
}
