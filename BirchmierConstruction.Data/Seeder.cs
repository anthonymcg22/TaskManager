using BirchmierConstruction.DataModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;  // forgot this reference
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BirchmierConstruction.Data
{
    public static class Seeder
    {
        static private ApplicationUser user { get; set; }
        static private DateTime now { get { return DateTime.Now; } set { now = value; } }
        //static method "Seed", takes in the following parameters which are set to true by default
        //this method will call the other static methods if the bool values are true
        public static void Seed(ApplicationDbContext context, bool createProjects = true, bool createTasks = true,
            bool createContacts = true, bool createResources = true, bool createUsersAndRoles = true)
        {
            if (createUsersAndRoles)
                CreateUsersAndRoles(context);

            user = context.Users.Where(u => u.UserName == "joe44@gmail.com").FirstOrDefault();

            if (createProjects)
                CreateProjects(context);
            if (createResources)
                CreateResources(context);
            if (createTasks)
                CreateTasks(context);
            if (createContacts)
                CreateContacts(context);
        }

        #region Create Users and Roles
        private static void CreateUsersAndRoles(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            roleManager.Create(new IdentityRole("Creator"));

            if (!context.Users.Any(user => user.UserName == "joe44@gmail.com"))
            {
                var applicationUser = new ApplicationUser() { UserName = "joe44@gmail.com" };
                userManager.Create(applicationUser, "_Joe44");

                userManager.AddToRole(applicationUser.Id, "Creator");
            }
        }
        #endregion

        #region Create Projects
        private static void CreateProjects(ApplicationDbContext db)
        {
            db.Projects.AddOrUpdate(o => o.ProjectId,
                new Project
                {
                    UserId = user.Id,
                    Name = "Make money for Mexico",
                    DateUpdated = now,
                    StartDate = now.AddDays(-1),
                    FinishDate = now.AddDays(2),
                    Notes = "We are excited to go to Mexico!",
                    IsBaseLine = false
                },
                new Project
                {
                    UserId = user.Id,
                    Name = "Clean the house project",
                    DateUpdated = now,
                    StartDate = now,
                    FinishDate = now,
                    Notes = "We are slacking on the chores!",
                    IsBaseLine = false
                });
            db.SaveChanges();
        }
        #endregion

        //create resources
        private static void CreateResources(ApplicationDbContext db)
        {
            db.Resources.AddOrUpdate(x => x.ResourceId,
                new Resource { CompanyName = "Veggie Meat Inc.", Notes = "A Company by SA-Poochie", UserId = user.Id },
                new Resource { CompanyName = "House Cleaning Experts", Notes = "We clean houses faster than you can sneeze", UserId = user.Id },
                new Resource { CompanyName = "Poochie Inc.", Notes = "The CEO is a dog!", UserId = user.Id });
            db.SaveChanges();
        }

        //create tasks
        private static void CreateTasks(ApplicationDbContext db)
        {
            db.Tasks.AddOrUpdate(o => o._TaskId,
                new _Task { 
                    Name = "Complete Wordpress Project", 
                    ProjectId = 1, 
                    ResourceId = 1, 
                    Order = 1, 
                    CompletionPercentage = 100, 
                    DateUpdated = now, 
                    StartDate = now.AddDays(-1), 
                    FinishDate = now, 
                    DurationVariance = 0, 
                    FinishVariance = 0 
                },
                new _Task { 
                    Name = "Clean the kitchen", 
                    ProjectId = 1, 
                    ResourceId = 2, 
                    Order = 2, 
                    CompletionPercentage = 25,  
                    DateUpdated = now,
                    StartDate = now.AddDays(1), 
                    FinishDate = now.AddDays(2),
                    DurationVariance = 0, 
                    FinishVariance = 0 
                });
            db.SaveChanges();
        }

        //create contacts
        private static void CreateContacts(ApplicationDbContext db)
        {
            db.Contacts.AddOrUpdate(x => x.Name,
                new Contact { Name = "Joe Michael", CellProvider = CellProvider.Verizon, Email = "joe44@gmail.com", CellNumber = "4763064322", ResourceId = 1 },
                new Contact { Name = "Taylor Smith", CellProvider = CellProvider.AT_T, Email = "greatshark@aol.com", CellNumber = "4735673498", ResourceId = 2 },
                new Contact { Name = "Tom Bait", CellProvider = CellProvider.Verizon, Email = "tom@email.com", CellNumber = "9358244815", ResourceId = 3 });
            db.SaveChanges();
        }
    }
}
