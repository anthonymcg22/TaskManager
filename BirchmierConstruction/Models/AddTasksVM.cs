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
    }
}