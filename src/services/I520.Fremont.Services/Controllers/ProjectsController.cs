using I520.Fremont.Services.Data;
using I520.Fremont.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ProjectService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository, ILogger<ProjectsController> logger)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Project> Get()
        {
            try
            {
                return _projectRepository.GetAllProjects();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

            return null;
        }
    }
}
