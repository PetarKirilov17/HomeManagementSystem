using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagementSystem.Data.Entites
{
    public class Location : BaseEntity
    {
        public Location()
        {
            //Assignments = new List<Assignment>();
        }
        public string Name { get; set; }

        public string Address { get; set; }

        public string CreatorId { get; set; }
        public virtual AppUser Creator { get; set; }

        //public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
