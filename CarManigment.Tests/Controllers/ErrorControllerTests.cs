using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Car_Manigment.Controllers;
using Car_Manigment.ViewModels;
using System.Diagnostics;

namespace CarManigment.Tests.Controllers
{
    public class ErrorControllerTests
    {
        private readonly Mock<ILogger<ErrorController>> _mockLogger;
        private readonly ErrorController _controller;

        public ErrorControllerTests()
        {
            _mockLogger = new Mock<ILogger<ErrorController>>();
            _controller = new ErrorController(_mockLogger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region NotFound (404) Tests

        [Fact]
        public void NotFound_ReturnsViewResult()
        {

            var result = _controller.NotFound();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void NotFound_ReturnsCorrectViewModel()
        {

            var result = _controller.NotFound() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.Equal(404, model.StatusCode);
            Assert.Equal("Page Not Found", model.ErrorTitle);
            Assert.Equal("The page you are looking for could not be found.", model.ErrorMessage);
        }

        [Fact]
        public void NotFound_SetsStatusCode404()
        {

            _controller.NotFound();

            Assert.Equal(404, _controller.Response.StatusCode);
        }

        [Fact]
        public void NotFound_ReturnsCorrectViewName()
        {

            var result = _controller.NotFound() as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("NotFound", result.ViewName);
        }

        [Fact]
        public void NotFound_CapturesOriginalPath()
        {

            _controller.HttpContext.Request.Path = "/invalid/path";

            var result = _controller.NotFound() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.Equal("/invalid/path", model.OriginalPath);
        }

        [Fact]
        public void NotFound_LogsWarning()
        {

            _controller.HttpContext.Request.Path = "/test/path";

            _controller.NotFound();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("404 Not Found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void NotFound_IncludesRequestId()
        {

            var result = _controller.NotFound() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.NotNull(model.RequestId);
            Assert.NotEmpty(model.RequestId);
        }

        #endregion

        #region InternalServerError (500) Tests

        [Fact]
        public void InternalServerError_ReturnsViewResult()
        {

            var result = _controller.InternalServerError();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void InternalServerError_ReturnsCorrectViewModel()
        {

            var result = _controller.InternalServerError() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.Equal(500, model.StatusCode);
            Assert.Equal("Internal Server Error", model.ErrorTitle);
            Assert.Contains("unexpected error", model.ErrorMessage);
        }

        [Fact]
        public void InternalServerError_SetsStatusCode500()
        {

            _controller.InternalServerError();

            Assert.Equal(500, _controller.Response.StatusCode);
        }

        [Fact]
        public void InternalServerError_ReturnsCorrectViewName()
        {

            var result = _controller.InternalServerError() as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("InternalServerError", result.ViewName);
        }

        [Fact]
        public void InternalServerError_LogsError()
        {

            _controller.InternalServerError();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("500 Internal Server Error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void InternalServerError_IncludesRequestId()
        {

            var result = _controller.InternalServerError() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.NotNull(model.RequestId);
            Assert.NotEmpty(model.RequestId);
        }

        #endregion

        #region HandleError Tests

        [Theory]
        [InlineData(400, "Bad Request")]
        [InlineData(401, "Unauthorized")]
        [InlineData(403, "Forbidden")]
        [InlineData(503, "Service Unavailable")]
        public void HandleError_WithKnownStatusCode_ReturnsCorrectErrorTitle(int statusCode, string expectedTitle)
        {

            var result = _controller.HandleError(statusCode) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.Equal(expectedTitle, model.ErrorTitle);
        }

        [Fact]
        public void HandleError_With404_ReturnsNotFoundView()
        {

            var result = _controller.HandleError(404) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("NotFound", result.ViewName);
        }

        [Fact]
        public void HandleError_With500_ReturnsInternalServerErrorView()
        {

            var result = _controller.HandleError(500) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("InternalServerError", result.ViewName);
        }

        [Fact]
        public void HandleError_WithUnknownStatusCode_ReturnsErrorView()
        {

            var result = _controller.HandleError(418) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void HandleError_SetsCorrectStatusCode()
        {

            var statusCode = 403;

            _controller.HandleError(statusCode);

            Assert.Equal(403, _controller.Response.StatusCode);
        }

        [Fact]
        public void HandleError_LogsWarning()
        {

            _controller.HandleError(403);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("HTTP 403 error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void HandleError_CapturesOriginalPath()
        {

            _controller.HttpContext.Request.Path = "/some/path";

            var result = _controller.HandleError(403) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model);
            Assert.Equal("/some/path", model.OriginalPath);
        }

        #endregion

        #region Response Cache Tests

        [Fact]
        public void NotFound_HasResponseCacheAttribute()
        {

            var methodInfo = typeof(ErrorController).GetMethod("NotFound", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                null, Type.EmptyTypes, null);

            var attributes = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

            Assert.NotNull(attributes);
            Assert.Single(attributes);
            var cacheAttribute = attributes[0] as ResponseCacheAttribute;
            Assert.NotNull(cacheAttribute);
            Assert.Equal(0, cacheAttribute.Duration);
            Assert.Equal(ResponseCacheLocation.None, cacheAttribute.Location);
            Assert.True(cacheAttribute.NoStore);
        }

        [Fact]
        public void InternalServerError_HasResponseCacheAttribute()
        {

            var methodInfo = typeof(ErrorController).GetMethod("InternalServerError");

            var attributes = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

            Assert.NotNull(attributes);
            Assert.Single(attributes);
        }

        #endregion
    }
}

