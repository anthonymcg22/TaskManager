using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BirchmierConstruction.DataModels;
using BirchmierConstruction.Data;
using System.Data.Entity;
using BirchmierConstruction.Models;

namespace BirchmierConstruction.Controllers
{
    public class ResourceController : Controller
    {
        //getting id of current user
        static string UserId { get { return System.Web.HttpContext.Current.User.Identity.GetUserId(); } }

        // GET: Resource
        [Authorize]
        public ActionResult Index()
        {
            ProjectAndResources model = new ProjectAndResources();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                model.Resources = db.Resources.Include("Contacts").Include("Tasks").Where(p => p.UserId == UserId).OrderBy(x => x.CompanyName).ToList();
                model.Projects = db.Projects.Include("Tasks").Where(p => p.UserId == UserId).OrderByDescending(x => x.StartDate).ToList();
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddResource()
        {
            return View(new Resource());
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddResource(Resource resource)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                resource.UserId = UserId;
                db.Resources.Add(resource);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Resource");
        }

        [Authorize]
        [HttpGet]
        public ActionResult ResourceDetails(int id)
        {
            Resource resource;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                resource = db.Resources.Include("Contacts").Include("Tasks").Where(x => x.ResourceId == id).FirstOrDefault();
                resource.Contacts.OrderBy(x => x.Name);
            }

            return View(resource);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditResource(int id)
        {
            Resource resource;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                resource = db.Resources.Where(x => x.ResourceId == id).FirstOrDefault();
            }

            return View(resource);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditResource(Resource resource)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    db.Entry(resource).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Resource");
        }

        [HttpGet]
        public ActionResult AddContact(int id)
        {
            Contact contact = new Contact();
            contact = new Contact() { ResourceId = id };
            return View(contact);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Contacts.Add(contact);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ResourceDetails", new { id = contact.ResourceId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditContact(int id)
        {
            Contact contact = new Contact();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                contact = db.Contacts.Where(x => x.ContactId == id).FirstOrDefault();
            }
            return View(contact);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Entry(contact).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ResourceDetails", new { id = contact.ResourceId });
        }

        [Authorize]
        public ActionResult DeleteContact(int id)
        {
            int ResourceID;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Contact contact = db.Contacts.Where(x => x.ContactId == id).FirstOrDefault();
                ResourceID = contact.ResourceId;
                db.Contacts.Remove(contact);
                db.SaveChanges();
            }

            return RedirectToAction("ResourceDetails", new {id = ResourceID });
        }

        [Authorize]
        public ActionResult DeleteResource(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var resource = (from a in db.Resources where a.ResourceId == id select a).FirstOrDefault();
                while (db.Contacts.Where(x => x.ResourceId == id).ToList().Count() != 0)
                {
                    var contact = db.Contacts.Where(x => x.ResourceId == id).FirstOrDefault();
                    db.Contacts.Remove(contact);
                    db.SaveChanges();
                }
                var tasks = db.Tasks.Where(x => x.ResourceId == id).ToList();
                for (var i = 0; i < tasks.Count(); i++)
                {
                    tasks[i].ResourceId = null;
                    db.SaveChanges();
                }
                db.Resources.Remove(resource);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Resource");
        }
    }
}