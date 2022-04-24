using HomeManagementSystem.Data.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // configures one-to-many relationship
            modelBuilder.Entity<Location>()
            .HasMany<Assignment>(l => l.Assignments)
            .WithOne(a => a.Location);

            modelBuilder.Entity<AppUser>()
                .HasMany<Assignment>(u => u.AssignedTasks)
                .WithOne(a => a.AssignedHousekeeper);

            modelBuilder.Entity<AppUser>()
                .HasMany<Assignment>(u => u.CreatedTasks)
                .WithOne(a => a.Creator);

            modelBuilder.Entity<AppUser>()
                .HasMany<Location>(u => u.Locations)
                .WithOne(l => l.Creator);
        }*/

        public DbSet<HomeManagementSystem.Data.Entites.Location> Locations { get; set; }

        public DbSet<HomeManagementSystem.Data.Entites.Assignment> Assignments { get; set; }
    }
}

