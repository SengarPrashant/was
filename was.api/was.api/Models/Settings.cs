namespace was.api.Models
{
    public class Settings
    {
        public JwtSettings Jwt { get; set; }
    }
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
