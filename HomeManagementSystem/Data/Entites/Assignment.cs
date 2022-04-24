using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;

namespace HomeManagementSystem.Data.Entites
{
    public class Assignment : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public string CreatorId { get; set; }
        public virtual AppUser Creator { get; set; } // Client
        public string AssignedHousekeeperId { get; set; }
        public virtual AppUser AssignedHousekeeper { get; set; } // Housekeeper

        public DateTime DeadLine { get; set; }

        public string CompletedTask { get; set; }

        public DateTime DateOfCompletion { get; set; }

        public Category CategoryOfAssignment { get; set; }

        public Status StatusOfAssignent { get; set; } = Status.Waiting;
        
    }
    public enum Category
    {
        Cleaning,
        PetCare,
        Babysitting,
        SeniorCitizenCare
    }

    public enum Status
    {
        Waiting,
        Assigned,
        ForView,
        Completed,
        Declined
    }
}
