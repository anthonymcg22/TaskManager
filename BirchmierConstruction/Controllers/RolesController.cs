using BirchmierConstruction.Data;
using BirchmierConstruction.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BirchmierConstruction.Controllers
{

    [Authorize(Roles="Creator")]
    public class RolesController : Controller
    {

        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var roles = db.Roles.ToList();
                return View(roles);
            }
        }

        public ActionResult ManageUserRoles()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList()
                    .Select(rr => new SelectListItem {
                        Value = rr.Name.ToString(),
                        Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var idResult = um.AddToRole(user.Id, RoleName);

                //var account = new AccountController();
                //account.UserManager.AddToRole(user.Id, RoleName);

                ViewBag.ResultMessage = "Role created successfully !";

                // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;

                return View("ManageUserRoles");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                    var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                    //var account = new AccountController();

                    ViewBag.RolesForThisUser = um.GetRoles(user.Id);

                    // prepopulat roles for the view dropdown
                    var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                    ViewBag.Roles = list;
                }
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                if (um.IsInRole(user.Id, RoleName))
                {
                    um.RemoveFromRole(user.Id, RoleName);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;

                return View("ManageUserRoles");
            }
        }

        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Roles.Add(new IdentityRole()
                    {
                        Name = collection["RoleName"]
                    });
                    db.SaveChanges();
                    ViewBag.ResultMessage = "Role created successfully !";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string RoleName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var thisRole = db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                db.Roles.Remove(thisRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var thisRole = db.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                return View(thisRole);
            }
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}