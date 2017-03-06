using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectService.Controllers;
using NSubstitute;
using I520.Fremont.Services.Data;
using Microsoft.Extensions.Logging;
using I520.Fremont.Services.Models;
using FluentAssertions;

namespace I520.Fremont.Services.Tests
{
    [TestFixture]
    public class ProjectsControllerTests
    {
        private IProjectRepository _projectRepository;
        private ILogger<ProjectsController> _logger;

        [SetUp]
        public void SetUpMocks()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _logger = Substitute.For<ILogger<ProjectsController>>();
        }

        private ProjectsController CreateControllerWithMocks()
        {
            return new ProjectsController(_projectRepository, _logger);
        }

        [TestFixture]
        public class Get : ProjectsControllerTests
        {
            [Test]
            public void ReturnsAllProjects()
            {
                var controller = CreateControllerWithMocks();

                var projects = new List<Project>
                {
                    new Project
                    {
                        Description = "I love the Mets",
                        Name = "Donde esta",
                        ImageUrls = new List<string>
                        {
                            "http://image"
                        }
                    }
                };

                _projectRepository.GetAllProjects().Returns(projects);

                var result = controller.Get();

                result.Should().NotBeNull();
                result?.Should().Equal(projects);
            }
        }
    }
}
