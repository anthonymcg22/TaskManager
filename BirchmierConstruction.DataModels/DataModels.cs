using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirchmierConstruction.DataModels
{

    [DebuggerDisplay("ProjectId={ProjectId}, UserId={UserId}, Name={Name}, IsBaseLine={IsBaseLine}, StartDate={StartDate}, FinishDate={FinishDate}, DateUpdated = {DateUpdated}, Notes={Notes}")]
    public class Project : DatesAndVariances
    {
        public int ProjectId { get; set; }   // primary key
        public string UserId { get; set; }
        public string Name { get; set; }   //Name of project
        public bool IsBaseLine { get; set; }
        public string Notes { get; set; }     // optional notes for project
        public List<_Task> Tasks { get; set; }    // List of tasks for project
    }

    [DebuggerDisplay("_TaskId={_TaskId}, Name={Name}, Order={Order}, ResourceId = {ResourceId}, ProjectId={ProjectId}, CompletionPercentage={CompletionPercentage}, DurationVariance={DurationVariance}, FinishVariance={FinishVariance}, StartDate={StartDate}, FinishDate={FinishDate}, DateUpdated={DateUpdated}, Predecessors={Predecessors}")]
    public class _Task : DatesAndVariances
    {
        public int _TaskId { get; set; }   //primary key
        public string Name { get; set; }
        public int Order { get; set; }
        public int CompletionPercentage { get; set; }
        public decimal? DurationVariance { get; set; }
        public decimal? FinishVariance { get; set; }
        public string Predecessors { get; set; }

        #region foreign keys
        public int ProjectId { get; set; }   // foreign key to Project
        public virtual Project Project { get; set; }

        public int? ResourceId { get; set; }  //foreign key to Resource
        public virtual Resource Resource { get; set; }
        #endregion
    }

    public class DatesAndVariances
    {
        public DateTime DateUpdated { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FinishDate { get; set; }
    }

    #region Resources and Contacts
    [DebuggerDisplay("ResourceId={ResourceId}, UserId={UserId}, CompanyName={CompanyName}, Notes={Notes}")]
    public class Resource
    {
        public int ResourceId { get; set; }
        public string UserId { get; set; }
        public string CompanyName { get; set; }
        public string Notes { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<_Task> Tasks { get; set; }
    }

    [DebuggerDisplay("ContactId={ContactId}, Name={Name}, Email={Email}, CellNumber={CellNumber}, CellProvider={CellProvider}, ResourceId={ResourceId}")]
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CellNumber { get; set; }
        public CellProvider CellProvider { get; set; }
        public int ResourceId { get; set; }
        public virtual Resource Resource { get; set; }
    }
    public enum CellProvider
    {
        [Display(Name = "AT&T")]
        AT_T,// – cellnumber@txt.att.net
        [Display(Name = "Verizon")]
        Verizon,// – cellnumber@vtext.com
        [Display(Name = "T Mobile")]
        T_Mobile,// – cellnumber@tmomail.net
        [Display(Name = "Sprint PCS")]
        Sprint_PCS,// - cellnumber@messaging.sprintpcs.com
        [Display(Name = "Virgin Mobile")]
        Virgin_Mobile,// – cellnumber@vmobl.com
        [Display(Name = "US Cellular")]
        US_Cellular,// – cellnumber@email.uscc.net
        [Display(Name = "Nextel")]
        Nextel,// - cellnumber@messaging.nextel.com
        [Display(Name = "Boost")]
        Boost,// - cellnumber@myboostmobile.com
        [Display(Name = "Alltel")]
        Alltel// – cellnumber@message.alltel.co
    }
    #endregion
}
