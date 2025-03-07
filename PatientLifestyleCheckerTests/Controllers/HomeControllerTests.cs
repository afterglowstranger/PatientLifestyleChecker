using LifeStyleChecker.Controllers;
using LifeStyleChecker.Models;
using LifeStyleChecker.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
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

        //[Fact]
        //public async Task PatientSearch_ValidModel_EligiblePatientFound_RedirectsToSurveyIndex()
        //{
        //    // Arrange
        //    var model = new FormPatientModel
        //    {
        //        NhsNumber = 123456789,
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Born = new DateOnly(1990, 01, 01)
        //    };

        //    var patient = new PatientModel
        //    {
        //        NhsNumber = 123456789,
        //        Name = "DOE, John",
        //        Born = new DateTime(1990, 01, 01)
        //    };

        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    mockHttpMessageHandler
        //        .Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
        //        .Returns(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(JsonConvert.SerializeObject(patient))
        //        });

        //    var client = new HttpClient(mockHttpMessageHandler.Object);
        //    _controller.Client = client;

        //    // Act
        //    var result = await _controller.PatientSearch(model);

        //    // Assert
        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectToActionResult.ActionName);
        //    Assert.Equal("Survey", redirectToActionResult.ControllerName);
        //    Assert.Equal(model.GetPatientAge(), redirectToActionResult.RouteValues["patientAge"]);
        //}

        //[Fact]
        //public async Task PatientSearch_ValidModel_UnderAge_ReturnsIndexViewWithError()
        //{
        //    // Arrange
        //    var model = new FormPatientModel
        //    {
        //        NhsNumber = 123456789,
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Born = new DateOnly(DateTime.Now.Year - 10, 01, 01)
        //    };

        //    var patient = new PatientModel
        //    {
        //        NhsNumber = 123456789,
        //        Name = "DOE, John",
        //        Born = new DateTime(DateTime.Now.Year - 10, 01, 01)
        //    };

        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    mockHttpMessageHandler
        //        .Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
        //        .Returns(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(JsonConvert.SerializeObject(patient))
        //        });

        //    var client = new HttpClient(mockHttpMessageHandler.Object);
        //    _controller.Client = client;

        //    // Act
        //    var result = await _controller.PatientSearch(model);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Index", viewResult.ViewName);
        //    Assert.Equal(model, viewResult.Model);
        //    Assert.True(_controller.ModelState.ContainsKey("NhsNumber"));
        //}

        //[Fact]
        //public async Task PatientSearch_ValidModel_DetailsNotMatched_ReturnsIndexViewWithError()
        //{
        //    // Arrange
        //    var model = new FormPatientModel
        //    {
        //        NhsNumber = 123456789,
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Born = new DateOnly(1990, 01, 01)
        //    };

        //    var patient = new PatientModel
        //    {
        //        NhsNumber = 123456789,
        //        Name = "DOE, Jane",
        //        Born = new DateTime(1990, 01, 01)
        //    };

        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    mockHttpMessageHandler
        //        .Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
        //        .Returns(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(JsonConvert.SerializeObject(patient))
        //        });

        //    var client = new HttpClient(mockHttpMessageHandler.Object);
        //    _controller.Client = client;

        //    // Act
        //    var result = await _controller.PatientSearch(model);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Index", viewResult.ViewName);
        //    Assert.Equal(model, viewResult.Model);
        //    Assert.True(_controller.ModelState.ContainsKey("NhsNumber"));
        //}

        //[Fact]
        //public async Task PatientSearch_ValidModel_NotFound_ReturnsIndexViewWithError()
        //{
        //    // Arrange
        //    var model = new FormPatientModel
        //    {
        //        NhsNumber = 123456789,
        //        FirstName = "John",
        //        LastName = "Doe",
        //        Born = new DateOnly(1990, 01, 01)
        //    };

        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    mockHttpMessageHandler
        //        .Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
        //        .Returns(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.NotFound
        //        });

        //    var client = new HttpClient(mockHttpMessageHandler.Object);
        //    _controller.Client = client;

        //    // Act
        //    var result = await _controller.PatientSearch(model);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Index", viewResult.ViewName);
        //    Assert.Equal(model, viewResult.Model);
        //    Assert.True(_controller.ModelState.ContainsKey("NhsNumber"));
        //}

      
    }
}
