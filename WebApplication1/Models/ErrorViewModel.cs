namespace Web.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
            RequestId = string.Empty;
        }
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorSource { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public int StatusCode { get; set; }
    }
}
