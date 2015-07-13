using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BirchmierConstruction.DataModels;

namespace BirchmierConstruction.Models
{
    public class ProjectViewModel
    {
        public int? ID { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Display(Name="Save as Base Line")]
        public bool IsBaseLine { get; set; }
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Finish Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FinishDate { get; set; }
    }

    public class ProjectAndResources
    {
        public List<Contact> Contacts { get; set; }
        public List<Resource> Resources { get; set; }
        public List<Project> Projects { get; set; }
        public Project Project { get; set; }
        public List<_Task> Tasks { get; set; }

        public AddTaskVM taskVM { get; set; }
    }
}