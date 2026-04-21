using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Car_Manigment.ViewModels;

namespace Car_Manigment.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        [Route("Error/404")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFound()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var originalPath = HttpContext.Request.Path.Value;

            _logger.LogWarning("404 Not Found: {Path} (RequestId: {RequestId})", originalPath, requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = 404,
                ErrorTitle = "Page Not Found",
                ErrorMessage = "The page you are looking for could not be found.",
                OriginalPath = originalPath
            };

            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        [Route("Error/500")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult InternalServerError()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("500 Internal Server Error (RequestId: {RequestId})", requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = 500,
                ErrorTitle = "Internal Server Error",
                ErrorMessage = "An unexpected error occurred while processing your request. Our team has been notified."
            };

            Response.StatusCode = 500;
            return View("InternalServerError", model);
        }

        [Route("Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HandleError(int statusCode)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var originalPath = HttpContext.Request.Path.Value;

            _logger.LogWarning("HTTP {StatusCode} error: {Path} (RequestId: {RequestId})", statusCode, originalPath, requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = statusCode,
                ErrorTitle = GetErrorTitle(statusCode),
                ErrorMessage = GetErrorMessage(statusCode),
                OriginalPath = originalPath
            };

            Response.StatusCode = statusCode;

            return statusCode switch
            {
                404 => View("NotFound", model),
                500 => View("InternalServerError", model),
                _ => View("Error", model)
            };
        }

        private static string GetErrorTitle(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                503 => "Service Unavailable",
                _ => $"Error {statusCode}"
            };
        }

        private static string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "The request could not be understood by the server.",
                401 => "You are not authorized to access this resource.",
                403 => "You do not have permission to access this resource.",
                404 => "The page you are looking for could not be found.",
                500 => "An unexpected error occurred. Our team has been notified.",
                503 => "The service is temporarily unavailable. Please try again later.",
                _ => "An error occurred while processing your request."
            };
        }
    }
}
