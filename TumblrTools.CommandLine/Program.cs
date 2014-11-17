namespace TumblrTools.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CLAP;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using TumblrTools.Domain;
    using TumblrTools.Generic;
    using TumblrTools.Infrastructure.Log4Net;
    using TumblrTools.Infrastructure.Mongo;

    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new Infrastructure.AppConfig.AppSettingsConfiguration();
            ILogger logger = SetupLog();
            IRepositoryBuilder repositoryBuilder = new RepositoryBuilder(
                "mongodb://localhost", 
                "tumblr_downloader");
            Application app = new Application(configuration, logger, repositoryBuilder.DownloadRepository, repositoryBuilder.PostRepository);

            Parser.Run(args, app);
            logger.Info("Finished!");
            logger.Dispose();
        }

        private static ILogger SetupLog()
        {
            ILogger logger = new Log4NetLogger("TumblrDownloader");
            log4net.Config.XmlConfigurator.Configure();

            logger.Info("Log set");
            return logger;
        }

        #region MigrateIds

        public static void MainMigrateIds()
        {
            const string connectionString = "mongodb://localhost";
            const string databaseName = "tumblr_downloader";
            const string collectionName = "download_config";

            foreach (
                WriteConcernResult writeConcernResult in 
                MigrateIds(connectionString, databaseName, collectionName, _ => Guid.NewGuid().ToString())
                .Where(writeConcernResult => !writeConcernResult.Ok))
            {
                Console.WriteLine(writeConcernResult);
            }
        }

        public static IEnumerable<WriteConcernResult> MigrateIds(
            string connectionString,
            string databaseName,
            string collectionName,
            Func<BsonDocument, BsonValue> idFunction)
        {
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();

            MongoDatabase database = server.GetDatabase(databaseName);

            MongoCollection<BsonDocument> entryCollection = database.GetCollection(collectionName);

            foreach (BsonDocument entry in entryCollection.FindAll())
            {
                yield return entryCollection.Remove(Query.EQ("_id", entry["_id"]));
                entry.Set("_id", idFunction(entry));
                yield return entryCollection.Save(entry);
            }
        }

        #endregion
    }
}
