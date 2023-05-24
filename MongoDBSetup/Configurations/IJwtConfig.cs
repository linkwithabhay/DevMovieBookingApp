namespace MongoDBSetup.Configurations
{
    public interface IJwtConfig
    {
        string Audience { get; set; }
        string Issuer { get; set; }
        string Secret { get; set; }
    }
}
