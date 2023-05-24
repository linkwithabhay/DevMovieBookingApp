namespace MongoDBSetup.Configurations
{
    public class JwtConfig: IJwtConfig
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
