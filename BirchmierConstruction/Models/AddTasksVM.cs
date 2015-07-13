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
        public List<SelectListItem> Percentages { get; set; }

        public void GetPercentages()
        {
            Percentages = new List<SelectListItem>
               {
                   new SelectListItem {Selected = true, Value = "0", Text = "0 %"},
                   new SelectListItem {Selected = false, Value = "25", Text = "25 %"},
                   new SelectListItem {Selected = false, Value = "50", Text = "50 %"},
                   new SelectListItem {Selected = false, Value = "75", Text = "75 %"},
                   new SelectListItem {Selected = false, Value = "100", Text = "100 %"}
               };
        }
    }
}