using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;

namespace AgileLabs.ContentManager.Repositories
{
    public class MongoDbContext
    {
        public MongoDbContext(IOptions<MongodbSettings> settings)
        {
            MongoSettings = settings.Value;
            MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(MongoSettings.ConnectionString));
            if (!string.IsNullOrWhiteSpace(MongoSettings.LoginDatabase))
            {
                mongoClientSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                var credential = MongoCredential.CreateCredential(MongoSettings.LoginDatabase, MongoSettings.UserName, MongoSettings.Password);
                mongoClientSettings.Credential = credential;
            }
            Client = new MongoClient(mongoClientSettings);
        }

        public MongoClient Client { get; private set; }

        public MongodbSettings MongoSettings { get; private set; }

        public IMongoDatabase GetDateBase()
        {
            return Client.GetDatabase(MongoSettings.Database, new MongoDatabaseSettings() { });
        }
    }
}
