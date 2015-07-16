using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using BirchmierConstruction.Data;
using BirchmierConstruction.DataModels;
using BirchmierConstruction.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BirchmierConstruction.Controllers
{
    //controlling the emailing and texting functionality
    public class NotifyController : Controller
    {
        static string UserId { get { return System.Web.HttpContext.Current.User.Identity.GetUserId(); } }
        static public ApplicationDbContext db { get { return new ApplicationDbContext(); } }
        // GET: Notify
        [HttpGet]
        public ActionResult Index(int id)
        {
            Resource resource = db.Resources.Where(x => x.ResourceId == id).FirstOrDefault();
            List<Project> projects = db.Projects.Where(p => p.UserId == UserId).ToList();
            List<Contact> contacts = db.Contacts.Where(x => x.ResourceId == id).ToList();
            List<_Task> Tasks = db.Tasks.Where(x => x.ResourceId == id).ToList();

            var appSettings = System.Web.Configuration.WebConfigurationManager.AppSettings;
            var Interface = new EmailandText();
            try
            {
                Interface.ProvideCredentials("Anthony", appSettings["mailAccount"], appSettings["mailPassword"]);
                MailAddress from = Interface.FromAddress;

                foreach (var contact in contacts)
                {
                    var toAddress = new MailAddress(contact.Email, contact.Name);
                    string postfix = Interface.CellEmailPostfix[contact.CellProvider];
                    var toCellAddress = new MailAddress(contact.CellNumber.Replace("-", "").Replace(".", "") + postfix, contact.Name);
                    string subject = "Tasks for " + resource.CompanyName;
                    string emailBody = "";
                    string textBody = "";
                    foreach (var task in Tasks)
                    {
                        var project = projects.Where(x => x.ProjectId == task.ProjectId).FirstOrDefault();
                        textBody = "<h3 style='text-align: center;'>Project: " + project.Name +
            "</h3><h3>Task: " + task.Name +
            "</h3><ul><li>Start: " + String.Format("{0:MM-dd-yyyy}", task.StartDate) + "</li><li>Finish: " + String.Format("{0:MM-dd-yyyy}", task.FinishDate) + "</li><li>Completion: " + task.CompletionPercentage + "% </li></ul><br/>";
                        emailBody += textBody;

                        using (var message = new MailMessage(from, toCellAddress) { Subject = subject, Body = textBody, IsBodyHtml = true })
                        {
                            Interface.smtp.Send(message);
                        }
                    }
                    using (var message = new MailMessage(from, toAddress) { Subject = subject, Body = emailBody, IsBodyHtml = true })
                    {
                        Interface.smtp.Send(message);
                    }
                }
            }
            catch (Exception e)
            {
                db.Dispose();
                ViewBag.ResultMessage = e.GetType() + ": " + e.Message;
                return View();
            }

            db.Dispose();
            TempData["ResultMessage"] = "Email/Text Updates sent succesfully.";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateOptions()
        {
            ProjectAndResources model = new ProjectAndResources();
            model.Contacts = db.Contacts.OrderBy(x => x.Name).ToList();
            model.Projects = db.Projects.Include("Tasks").Where(p => p.UserId == UserId).ToList();
            List<int> pIDS = new List<int>();
            model.Projects.ForEach(p => pIDS.Add(p.ProjectId));
            model.Resources = db.Resources.Include("Tasks").Include("Contacts").OrderBy(x => x.CompanyName).ToList();
            model.Tasks = db.Tasks.Where(t => pIDS.Contains(t.ProjectId)).OrderBy(x => x.StartDate).ToList();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateOptions(DateTime start, DateTime finish, int[] chosenResources, int[] chosenTasks)
        {
            int[] Tasks = chosenTasks;
            return RedirectToAction("Index", "Home");
        }
    }
}