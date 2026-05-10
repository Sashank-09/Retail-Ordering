namespace UrbanBites.Application.DTOs.Auth
{
    public class EmailSettings
    {
        public string SendGridApiKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}