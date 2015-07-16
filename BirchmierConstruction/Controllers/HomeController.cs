using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitMiracle.Docotic.Pdf;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BirchmierConstruction.Data;
using BirchmierConstruction.DataModels;
using BirchmierConstruction.Adapters.Interfaces;
using BirchmierConstruction.Adapters.DataAdapters;
using System.Data.Entity;
using System.Data.Entity.Validation;
using BirchmierConstruction.Models;
using System.Globalization;
using System.Text;

namespace BirchmierConstruction.Controllers
{
    public class HomeController : Controller
    {
        //making private property of type IProjectAdapter : Folders -> Adapters -> Interfaces
        IProjectAdapter _adapter;
        //getting id of current user
        static string UserId { get { return System.Web.HttpContext.Current.User.Identity.GetUserId(); } }

        #region Controller Constructors
        public HomeController() //constructor, initializes _adapter
        {
            _adapter = new ProjectAdapter();
        }

        //constructor, can pass in a new adapter such as mock adapter
        public HomeController(IProjectAdapter adapter)
        {
            _adapter = adapter;
        }
        #endregion

        [HttpGet]
        public ActionResult Index() // ActionResult of Index Page on Home Controller
        {
            return View(_adapter.GetProjects(UserId));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("5DCLC-D241C-NJ5Q1-E88HK-87873");
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    if (ext == ".pdf")
                    {
                        string data = String.Empty;

                        #region Download/Save file to server
                        HttpServerUtility server = System.Web.HttpContext.Current.Server;
                        foreach (string folder in new string[] { "~/App_Data", "~/App_Data/Uploads" })
                        {
                            string map = server.MapPath(folder);
                            if (!Directory.Exists(map))
                                Directory.CreateDirectory(map);
                        }

                        var fileName = Path.GetFileName(file.FileName);

                        // store the file inside ~/App_Data/Uploads folder
                        var path = Path.Combine(server.MapPath("~/App_Data/Uploads"), fileName);
                        file.SaveAs(path);
                        #endregion

                        List<string> tasks = new List<string>();
                        List<int> completes = new List<int>();
                        List<DateTime> starts = new List<DateTime>();
                        List<DateTime> ends = new List<DateTime>();
                        List<string> preds = new List<string>();
                        List<string> resources = new List<string>();
                        string task = String.Empty;
                        string start = String.Empty;
                        string end = String.Empty;
                        string pred = String.Empty;
                        bool predAdded = false;
                        string res = String.Empty;
                        bool resAdded = false;

                        PdfDocument pdf = new PdfDocument(path);
                        for (var i = 0; i < pdf.Pages.Count; i++)
                        {
                            var page = pdf.Pages[i].GetWords();
                            for (var j = 0; j < page.Count; j++)
                            {
                                PdfTextData d = page[j];

                                double l = d.Bounds.Left;
                                double r = d.Bounds.Right;

                                #region Name
                                if (l >= 58 && r < 270)
                                {
                                    if (!resAdded && tasks.Count > 0)
                                    {
                                        resources.Add(res);
                                        res = String.Empty;
                                        resAdded = true;
                                    }

                                    predAdded = false;
                                    if (task.Length > 0) task += " ";
                                    task += d.Text;

                                    if (page[j + 1].Bounds.Left >= 270 && task.Length > 0)
                                    {
                                        if (task.Contains("Task Name"))
                                            task = task.Replace("Task Name", "");
                                        else
                                        {
                                            tasks.Add(task);
                                            task = String.Empty;
                                        }
                                    }
                                }
                                #endregion

                                #region Completion
                                if (l >= 340 && r < 390 && !String.IsNullOrWhiteSpace(d.Text))
                                {
                                    int complete = 0;
                                    try
                                    {
                                        complete = Convert.ToInt32(d.Text.Substring(0, d.Text.IndexOf('%')));
                                        completes.Add(complete);
                                    }
                                    catch { }
                                }
                                #endregion

                                #region Start
                                if (l >= 400 && r < 490 && !String.IsNullOrWhiteSpace(d.Text) && d.Text != "Start")
                                {
                                    if (start.Length == 0)
                                        start += d.Text;
                                    else
                                    {
                                        start += " " + d.Text;
                                        DateTime date = DateTime.Now;

                                        foreach (string f in new string[] { "ddd M/dd/yy", "ddd M/d/yy" })
                                        {
                                            if (DateTime.TryParseExact(start, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                                break;
                                        }
                                        starts.Add(date);
                                        start = String.Empty;
                                    }
                                }
                                #endregion

                                #region Finish
                                if (l >= 490 && r < 546.9 && !String.IsNullOrWhiteSpace(d.Text) && d.Text != "Finish")
                                {
                                    if (end.Length == 0)
                                        end += d.Text;
                                    else
                                    {
                                        end += " " + d.Text;
                                        DateTime date = DateTime.Now;

                                        foreach (string f in new string[] { "ddd M/dd/yy", "ddd M/d/yy" })
                                        {
                                            if (DateTime.TryParseExact(end, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                                break;
                                        }
                                        ends.Add(date);
                                        end = String.Empty;
                                    }
                                }
                                #endregion

                                #region Predecessors
                                if (l >= 545 && d.Text != "Predecessors" && d.Text != "Resource")
                                {
                                    if (r < 630)
                                    {
                                        predAdded = false;
                                        if (pred.Length > 0) pred += " ";
                                        pred += d.Text;

                                        if (page[j + 1].Bounds.Left >= 630)
                                        {
                                            preds.Add(pred);
                                            pred = String.Empty;
                                            predAdded = true;
                                        }
                                    }
                                    else if (l <= 632 && !predAdded)
                                    {
                                        preds.Add("");
                                    }
                                }
                                #endregion

                                #region Resources
                                if (l >= 630 && d.Text != "Resource" && d.Text != "Names")
                                {
                                    if (res.Length > 0) res += " ";
                                    res += d.Text;
                                    resAdded = false;
                                }
                                #endregion
                            }
                        }
                        // adding blank areas
                        preds.Insert(0, "");
                        preds.Insert(61, "");
                        resources.Add(res);
                        resources.Insert(61, "");

                        ViewBag.PDF_Text = "testing"; // data;

                        int projID = 0;
                        using (ApplicationDbContext db = new ApplicationDbContext())
                        {
                            var pro = new Project() { Name = "Magellan Health Expansion", UserId = UserId, DateUpdated = DateTime.Now, StartDate = new DateTime(2015, 06, 15), FinishDate = DateTime.Now };
                            db.Projects.Add(pro);
                            db.SaveChanges();
                            db.Entry(pro).GetDatabaseValues();
                            projID = pro.ProjectId;

                            for (var i = 0; i < 85; i++)
                            {
                                int? resID = null;
                                string resName = resources[i];

                                if (!String.IsNullOrWhiteSpace(resName))
                                {
                                    var resource = db.Resources.FirstOrDefault(r => r.CompanyName == resName && r.UserId == UserId);
                                    if (resource == null)
                                    {
                                        var re = new Resource() { CompanyName = resName, UserId = UserId };
                                        db.Resources.Add(re);
                                        db.SaveChanges();
                                        db.Entry(re).GetDatabaseValues();
                                        resID = re.ResourceId;
                                    }
                                    else
                                        resID = resource.ResourceId;
                                }

                                var t = new _Task() { Order = i > 60 ? i + 2 : i + 1, ProjectId = projID, Name = tasks[i], CompletionPercentage = completes[i], StartDate = starts[i], FinishDate = ends[i], Predecessors = preds[i], ResourceId = resID, DateUpdated = DateTime.Now };
                                db.Tasks.Add(t);
                                if (i % 50 == 0)
                                    db.SaveChanges();
                            }
                            db.SaveChanges();
                        }
                        return RedirectToAction("ProjectDetails", new { id = projID });
                    }
                    else
                    {
                        ViewBag.PDF_Error = "Incorrect file format.  Please choose a '.pdf' file.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.PDF_Error = "You have not chosen a file to upload.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.PDF_Error = "Error: " + ex.Message;
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteProject(int projID) //passing in ID of project to delete
        {
            int numTasks = 0;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Project p = db.Projects.Find(projID);  //find project

                    if (p == null)  //if project is not found say so
                        return Json(new { Success = "False", Result = "The project was not found and could not be deleted" });

                    //remove all tasks referencing the project
                    while (db.Tasks.Where(t => t.ProjectId == projID).Count() != 0)
                    {
                        _Task task = db.Tasks.FirstOrDefault(t => t.ProjectId == projID);
                        db.Tasks.Remove(task);
                        db.SaveChanges();
                        numTasks++;
                    }

                    db.Projects.Remove(p);  //now remove the project
                    db.SaveChanges();       //save changes
                }
            }
            catch (DbEntityValidationException e)
            {
                var message = string.Empty;
                foreach (DbEntityValidationResult validationErrors in e.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        message += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return Json(new { Success = "False", Result = message });
            }
            catch (Exception e)
            {
                return Json(new { Success = "False", Result = e.GetType() + ": " + e.Message });
            }

            return Json(new { Success = "True", Result = string.Format("The project has been deleted and {0} associated tasks have been deleted", numTasks) });
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddProject(int? id)
        {
            ProjectViewModel pro = new ProjectViewModel();
            if (id != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    pro = db.Projects.Where(p => p.ProjectId == id)
                        .Select(x => new ProjectViewModel
                        {
                            ID = x.ProjectId,
                            Name = x.Name,
                            IsBaseLine = x.IsBaseLine,
                            StartDate = x.StartDate,
                            FinishDate = x.FinishDate,
                            Notes = x.Notes
                        }).FirstOrDefault();
                }
            }

            return View(pro);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddProject(ProjectViewModel project)
        {
            int projectId = 0;
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Model Error"] += "<p style='color:lightblue;'>" + modelState.Value + ": " + error.ErrorMessage + "</p>";
                        }
                    }
                    return View();
                }

                projectId = _adapter.AddProject(project, UserId);

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        TempData["Exception"] += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("ProjectDetails", new { id = projectId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProjectDetails(int id)
        {
            ProjectAndResources model = _adapter.GetProject(id, UserId);
            return View(model);
        }

        [Authorize]
        public ActionResult DeleteTask(int id)
        {
            int ProjID = _adapter.DeleteTask(id);
            return RedirectToAction("ProjectDetails", new { id = ProjID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddTask(ProjectAndResources model)
        {
            try
            {
                _adapter.AddTask(model.taskVM.Task);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        TempData["Exception"] += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("ProjectDetails", new { id = model.taskVM.Task.ProjectId });
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveBaseLine(int id, bool save)
        {
            bool success = _adapter.SaveBaseLine(id, save, UserId);
            return Json(new
            {
                Success = success,
                Message = success ? "Project has been updated!" : "Error updating project!"
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateCompletion(int taskid, int percent)
        {
            bool success;
            using (var db = new ApplicationDbContext())
            {
                var task = db.Tasks.Where(t => t._TaskId == taskid).FirstOrDefault();
                task.CompletionPercentage = percent;
                task.DateUpdated = DateTime.Now;
                success = db.SaveChanges() == 1;
            }
            return Json(new
            {
                Success = success,
                Message = success ? "Updated successfully!" : "Failed to update!",
                TaskId = taskid,
                Percent = percent
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateResource(int taskid, int? resourceid)
        {
            if (resourceid == 0)
                resourceid = null;
            bool success;
            string resourceName;
            using (var db = new ApplicationDbContext())
            {
                var task = db.Tasks.Where(t => t._TaskId == taskid).FirstOrDefault();
                task.ResourceId = resourceid;
                task.DateUpdated = DateTime.Now;
                var resource = db.Resources.Where(r => r.ResourceId == resourceid && r.UserId == UserId).FirstOrDefault();
                resourceName = resourceid == null ? "No Resource Assigned" : resource == null ? "No Resource found" : resource.CompanyName;
                success = db.SaveChanges() == 1;
            }
            return Json(new { Success = success, Message = success ? "Updated successfully!" : "Failed to update!", TaskId = taskid, Resource = resourceName, ResourceID = resourceid }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateTaskName(int taskid, string name)
        {
            bool success;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var task = db.Tasks.Where(x => x._TaskId == taskid).FirstOrDefault();
                task.Name = name;
                task.DateUpdated = DateTime.Now;
                success = db.SaveChanges() == 1;
            }
            return Json(new { Success = success, Message = success ? "Updated successfully!" : "Failed to update!", TaskId = taskid, TaskName = name }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateTaskPredecessors(int taskid, string name)
        {
            bool success;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var task = db.Tasks.Where(x => x._TaskId == taskid).FirstOrDefault();
                task.Predecessors = name;
                task.DateUpdated = DateTime.Now;
                success = db.SaveChanges() == 1;
            }
            return Json(new { Success = success, Message = success ? "Updated successfully!" : "Failed to update!", TaskId = taskid, TaskName = name }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateTaskStart(int taskid, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var task = db.Tasks.FirstOrDefault(x => x._TaskId == taskid);
                task.StartDate = dateTime;
                task.DateUpdated = DateTime.Now;
                db.SaveChanges();
            }
            return Json(new { Success = true, TaskId = taskid, Date = dateTime.ToString("ddd MM/dd/yyyy") }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult UpdateTaskFinish(int taskid, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var task = db.Tasks.FirstOrDefault(x => x._TaskId == taskid);
                task.FinishDate = dateTime;
                task.DateUpdated = DateTime.Now;
                db.SaveChanges();
            }
            return Json(new { Success = true, TaskId = taskid, Date = dateTime.ToString("ddd MM/dd/yyyy") }, JsonRequestBehavior.AllowGet);
        }
    }
}