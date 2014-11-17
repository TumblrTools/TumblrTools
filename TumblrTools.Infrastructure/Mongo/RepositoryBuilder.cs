namespace TumblrTools.Infrastructure.Mongo
{
    using MongoDB.Driver;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Domain;
    using TumblrTools.Infrastructure.Mongo.Repositories;

    public class RepositoryBuilder : IRepositoryBuilder
    {
        public RepositoryBuilder(string connectionString, string databaseName)
        {
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();

            var database = server.GetDatabase(databaseName);

            MongoCollection<DownloadEntry> entryCollection = database.GetCollection<DownloadEntry>("download_config");
            MongoCollection<PhotoPost> postCollection = database.GetCollection<PhotoPost>("posts");

            this.DownloadRepository = new DownloadRepository(entryCollection);
            this.PostRepository = new PostRepository(postCollection);
        }

        public IDownloadRepository DownloadRepository { get; private set; }
        public IPostRepository PostRepository { get; private set; }

        public void Save()
        {
        }
    }
}