using I520.Fremont.Services.Models;
using System.Collections.Generic;

namespace I520.Fremont.Services.Data
{
    public interface IProjectRepository
    {
        List<Project> GetAllProjects();
    }
}