using Microsoft.Extensions.Configuration;

namespace PosMaster.Extensions
{
    public class DbSetting
    {
        public static string ConnectionString(IConfiguration configuration)
        {
            var host = configuration["Database:Host"];
            var port = configuration["Database:Port"];
            var dbUser = configuration["Database:User"];
            var dbName = configuration["Database:Name"];
            var dbPassword = configuration["Database:Password"];
            var realDbUser = Encrypter.Decrypt(dbUser);
            var realDbPassword = Encrypter.Decrypt(dbPassword);
            return $"Host={host};Port={port};Database={dbName};Username={realDbUser};Password={realDbPassword}";
        }
    }
}