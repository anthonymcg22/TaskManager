using BirchmierConstruction.DataModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirchmierConstruction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AzureConnection", throwIfV1Schema: false) //connecting to AzureConnection connection string -- see web.config file
        {
            this.Configuration.LazyLoadingEnabled = false; //make lazy loading false
        }

        //method that returns a new instance of ApplicationDbContext class
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //making public DbSet objects with classes to store in database
        public DbSet<Project> Projects { get; set; }
        public DbSet<_Task> Tasks { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
