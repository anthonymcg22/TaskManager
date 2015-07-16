using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BirchmierConstruction.Adapters.Interfaces;
using BirchmierConstruction.DataModels;
using BirchmierConstruction.Data;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Data.Entity;
using BirchmierConstruction.Models;

namespace BirchmierConstruction.Adapters.DataAdapters
{
    public class ProjectAdapter : IProjectAdapter
    {
        public List<Project> GetProjects(string UserId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
                return db.Projects.Where(x => x.UserId == UserId).Include("Tasks").OrderBy(x => x.Name).ToList();
        }

        public ProjectAndResources GetProject(int id, string userid)
        {
            ProjectAndResources model = new ProjectAndResources();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                model.Project = db.Projects.Include("Tasks").Where(x => x.ProjectId == id && x.UserId == userid).FirstOrDefault();
                model.Resources = db.Resources.Include("Tasks").Include("Contacts").Where(x => x.UserId == userid).ToList();
            }
            model.taskVM = new AddTaskVM()
            {
                Resources = GetResourcesDropDownList(userid),
                Task = PrepNewTask(id),
            };

            return model;
        }

        public void AddTask(_Task task)
        {
            task.DateUpdated = DateTime.Now;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var Tasks = db.Tasks.Where(t => t.ProjectId == task.ProjectId).ToList();
                    if (task.ResourceId == 0)
                        task.ResourceId = (int?)null;
                    db.Tasks.Add(task);
                    db.SaveChanges();
                }
            }
            catch (Exception e) { }
        }

        public int DeleteTask(int id)
        {
            int ProjID = 0;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                _Task task = db.Tasks.Where(x => x._TaskId == id).FirstOrDefault();
                //saving projectid of task
                ProjID = task.ProjectId;
                //remove task and save changes
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
            return ProjID;
        }
        public List<SelectListItem> GetResourcesDropDownList(string userid)
        {
            List<SelectListItem> ResourcesDropDownList;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Resources = db.Resources.Include("Tasks").Where(x => x.UserId == userid).OrderBy(x => x.CompanyName).ToList();

                ResourcesDropDownList = new List<SelectListItem> { new SelectListItem { Selected = true, Value = "0", Text = Resources.Count() != 0 ? "No Resource Assigned" : "Go Add Resources First" } };

                for (var i = 0; i < Resources.Count(); i++)
                {
                    ResourcesDropDownList.Add(new SelectListItem { Selected = false, Value = Resources[i].ResourceId.ToString(), Text = Resources[i].CompanyName });
                }
            }
            return ResourcesDropDownList;
        }


        public _Task PrepNewTask(int id)
        {
            _Task task = new _Task { ProjectId = id };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var tasks = db.Tasks.Where(x => x.ProjectId == id).ToList();
                var project = db.Projects.Where(x => x.ProjectId == id).FirstOrDefault();

                task.StartDate = tasks.Count() == 0 ? project.StartDate : tasks.OrderByDescending(x => x.Order).FirstOrDefault().FinishDate.AddDays(1);
                task.Order = tasks.Count() == 0 ? 1 : tasks.OrderByDescending(x => x.Order).Select(x => x.Order).FirstOrDefault() + 1;
            }
            return task;
        }

        public bool SaveBaseLine(int id, bool save, string userID)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var project = db.Projects.Where(x => x.ProjectId == id && x.UserId == userID).FirstOrDefault();
                    if (project != null)
                    {
                        project.IsBaseLine = save;
                        project.DateUpdated = DateTime.Now;
                        db.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public int AddProject(ProjectViewModel project, string userId)
        {
            Project p = new Project();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (project.ID == null)
                {
                    p = new Project()
                    {
                        Name = project.Name,
                        IsBaseLine = project.IsBaseLine,
                        Notes = project.Notes,
                        StartDate = project.StartDate,
                        FinishDate = project.FinishDate,
                        DateUpdated = DateTime.Now,
                        UserId = userId
                    };
                    db.Projects.Add(p);
                }
                else
                {
                    p = db.Projects.Where(x => x.ProjectId == project.ID && x.UserId == userId).FirstOrDefault();
                    if (p != null)
                    {
                        p.Name = project.Name;
                        p.IsBaseLine = project.IsBaseLine;
                        p.Notes = project.Notes;
                        p.StartDate = project.StartDate;
                        p.FinishDate = project.FinishDate;
                        p.DateUpdated = DateTime.Now;
                    }
                }
                db.SaveChanges();
                db.Entry(p).GetDatabaseValues();
            }
            return p.ProjectId;
        }
    }
}