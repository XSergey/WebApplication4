using MongoDB.Driver;

namespace WebApplication4.Api.V1.Providers.Database
{
    public class MongoDbProvider
    {
        /// <summary>
        /// MongoClient. 
        /// </summary>
        public MongoClient Client;

        /// <summary>
        /// IMongoDatabase interface of database in MongoDb.
        /// </summary>
        public IMongoDatabase Db;

        public MongoDbProvider(string url, string database)
        {
            this.Client = new MongoClient(url);
            //if the database is not exist, creates the database
            this.Db = this.Client.GetDatabase(database);
        }
    }
}
