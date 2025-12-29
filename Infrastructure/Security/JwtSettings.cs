namespace Infrastructure.Security
{
    namespace Infrastructure.Security
    {
        public class JwtSettings
        {
            public string Secret { get; set; } = null!;
            public string Issuer { get; set; } = null!;
            public string Audience { get; set; } = null!;
            public int TokenExpirationInMinutes { get; set; }
        }
    }

}
