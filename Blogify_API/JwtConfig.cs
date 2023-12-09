namespace Blogify_API
{
    public class JwtConfig
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SigningKey { get; set; }

        public int Expires { get; set; }

        public bool ValidateLifetime { get; set; }
    }
}
