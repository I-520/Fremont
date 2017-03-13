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
                _logger.LogDebug("Getting all projects...");

                return _projectRepository.GetAllProjects();
            }
            catch (DocumentClientException exception)
            {
                Exception baseException = exception.GetBaseException();
                _logger.LogError("{StatusCode} error occurred: {ErrorMessage}, Message: {BaseMessage}", exception.StatusCode, exception.Message, baseException.Message);
            }
            catch (Exception exception)
            {
                Exception baseException = exception.GetBaseException();
                _logger.LogError("Error: {ErrorMessage}, Message: {BaseMessage}", exception.Message, baseException.Message);
            }

            return null;
        }
    }
}
