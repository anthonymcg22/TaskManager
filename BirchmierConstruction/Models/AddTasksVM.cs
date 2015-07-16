using BirchmierConstruction.Data;
using BirchmierConstruction.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BirchmierConstruction.Models
{
    public class AddTaskVM
    {
        public _Task Task { get; set; }
        public List<SelectListItem> Resources { get; set; }
        public List<SelectListItem> Percentages
        {
            get
            {
                var list = new List<SelectListItem>();
                foreach (int i in new int[] { 0, 25, 50, 75, 100 })
                    list.Add(new SelectListItem() { Selected = i == 0, Value = i.ToString(), Text = i + " %" });
                return list;
            }
        }
        public void GetResourcesDropDownList(string userid)
        {
            List<SelectListItem> ResourcesDropDownList;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var resources = db.Resources.Include("Tasks").Where(x => x.UserId == userid).OrderBy(x => x.CompanyName).ToList();

                ResourcesDropDownList = new List<SelectListItem> { new SelectListItem { Selected = true, Value = "0", Text = resources.Count() != 0 ? "No Resource Assigned" : "Go Add Resources First" } };

                for (var i = 0; i < resources.Count(); i++)
                {
                    ResourcesDropDownList.Add(new SelectListItem { Selected = false, Value = resources[i].ResourceId.ToString(), Text = resources[i].CompanyName });
                }
            }
            Resources = ResourcesDropDownList;
        }
    }
}