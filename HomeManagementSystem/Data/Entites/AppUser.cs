using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagementSystem.Data.Entites
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            //AssignedTasks = new List<Assignment>();
            //CreatedTasks = new List<Assignment>();
            //Locations = new List<Location>();
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        //public virtual ICollection<Assignment> AssignedTasks { get; set; } // for a housekeeper
        //
        //public virtual ICollection<Assignment> CreatedTasks { get; set; } // for a client
        //
        //public virtual ICollection<Location> Locations { get; set; }
    }
}
