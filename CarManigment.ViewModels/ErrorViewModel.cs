namespace Car_Manigment.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int StatusCode { get; set; }

        public string ErrorTitle { get; set; } = "Error";

        public string ErrorMessage { get; set; } = "An error occurred while processing your request.";

        public string? OriginalPath { get; set; }
    }
}
