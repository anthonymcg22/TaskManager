using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirchmierConstruction.DataModels;
using System.Web.Mvc;
using BirchmierConstruction.Models;

namespace BirchmierConstruction.Adapters.Interfaces
{
    public interface IProjectAdapter
    {
        List<Project> GetProjects(string userId);
        ProjectAndResources GetProject(int id, string userid);
        int AddProject(ProjectViewModel project, string userId);
        _Task PrepNewTask(int projectId);
        void AddTask(_Task task);
        int DeleteTask(int id);
        bool SaveBaseLine(int id, bool save, string userID);
    }
}
