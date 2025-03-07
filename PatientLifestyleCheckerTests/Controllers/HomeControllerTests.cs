using LifeStyleChecker.Controllers;
using LifeStyleChecker.Models;
using LifeStyleChecker.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LifeStyleCheckerTests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(config => config["Configurations:NhsNumberApiKey"]).Returns("test_key");
            _controller = new HomeController(_mockLogger.Object, _mockConfig.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PatientSearch_InvalidModel_ReturnsIndexView()
        {
            // Arrange
            _controller.ModelState.AddModelError("NhsNumber", "Required");
            var model = new FormPatientModel();

            // Act
            var result = await _controller.PatientSearch(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
        }

        


        [Fact]
        public void GetViewBasedOnParameters_EligiblePatientFound_ReturnsRedirectToSurveyIndex()
        {
            // Arrange
            var model = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };

            // Act
            var result = _controller.GetViewBasedOnParameters(SearchOutcomes.EligiblePatientFound, model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Survey", redirectToActionResult.ControllerName);
            Assert.Equal(model.GetPatientAge(), redirectToActionResult.RouteValues["patientAge"]);
        }

        [Fact]
        public void GetViewBasedOnParameters_UnderAge_ReturnsIndexViewWithError()
        {
            // Arrange
            var model = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(DateTime.Now.Year - 10, 01, 01)
            };

            // Act
            var result = _controller.GetViewBasedOnParameters(SearchOutcomes.UnderAge, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("NhsNumber"));
            var modelStateEntry = _controller.ModelState["NhsNumber"];
            Assert.NotNull(modelStateEntry);
            Assert.Contains(modelStateEntry.Errors, e => e.ErrorMessage == "You are not eligible for this service, thank you for your interest");
        }

        [Fact]
        public void GetViewBasedOnParameters_DetailsNotMatched_ReturnsIndexViewWithError()
        {
            // Arrange
            var model = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };

            // Act
            var result = _controller.GetViewBasedOnParameters(SearchOutcomes.DetailsNotMatched, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("NhsNumber"));
            var modelStateEntry = _controller.ModelState["NhsNumber"];
            Assert.NotNull(modelStateEntry);
            Assert.Contains(modelStateEntry.Errors, e => e.ErrorMessage == "Your details could not be found");
        }

        [Fact]
        public void GetViewBasedOnParameters_NotFound_ReturnsIndexView()
        {
            // Arrange
            var model = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };

            // Act
            var result = _controller.GetViewBasedOnParameters(SearchOutcomes.NotFound, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
            var modelStateEntry = _controller.ModelState["NhsNumber"];
            Assert.NotNull(modelStateEntry);
            Assert.Contains(modelStateEntry.Errors, e => e.ErrorMessage == "Your details could not be found");
        }

       

    }
}
