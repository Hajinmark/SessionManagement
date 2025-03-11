namespace SessionManagement.Models.DTO
{
    public class AuthenticationDTO
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool IsLogin { get; set; }
        public DateTime Expiration { get; set; }

    }
}
