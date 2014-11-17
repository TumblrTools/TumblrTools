namespace TumblrTools.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using TumblrTools.Domain;

    public class DownloadRepository : IDownloadRepository
    {
        private readonly MongoCollection<DownloadEntry> collection;

        public DownloadRepository(MongoCollection<DownloadEntry> context)
        {
            this.collection = context;
        }

        public string NewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public DownloadEntry Get(string blogId)
        {
            IMongoQuery query = Query.And(
                Query<DownloadEntry>.Where(e => !e.Removed),
                Query<DownloadEntry>.EQ(e => e.BlogId, blogId));
            DownloadEntry existing = this.collection.FindOne(query);
            return existing;
        }

        public IList<DownloadEntry> GetAll()
        {
            return this.GetCursorForNotRemoved().ToList();
        }

        public IList<DownloadEntry> GetAllSortedByBlogId()
        {
            return this.GetCursorForNotRemoved().OrderBy(e => e.BlogId).ToList();
        }

        public void Create(DownloadEntry entry)
        {
            WriteConcernResult result = this.collection.Insert(entry);

            if (!result.Ok)
            {
                throw new Exception("Create failed");
            }
        }

        public void Update(DownloadEntry entry)
        {
            IMongoQuery query = Query<DownloadEntry>.EQ(e => e.BlogId, entry.BlogId);
            IMongoUpdate update = Update<DownloadEntry>.Replace(entry);
            WriteConcernResult result = this.collection.Update(query, update);
            if (!result.Ok)
            {
                throw new Exception("Update failed");
            }
        }

        public bool Remove(string blogId)
        {
            IMongoQuery query = Query<DownloadEntry>.EQ(e => e.BlogId, blogId);
            WriteConcernResult result = this.collection.Remove(query);
            return result.Ok;
        }

        public void UpdateLastDownloadTime(DownloadEntry entry, DateTime lastDownload)
        {
            IMongoQuery query = Query<DownloadEntry>.EQ(e => e.Id, entry.Id);
            UpdateBuilder<DownloadEntry> updateBuilder = new UpdateBuilder<DownloadEntry>();
            IMongoUpdate update = updateBuilder.Set(e => e.LastUpdate, lastDownload);
            this.collection.Update(query, update);
        }

        private IEnumerable<DownloadEntry> GetCursorForNotRemoved()
        {
            IMongoQuery query = Query<DownloadEntry>.Where(e => !e.Removed);
            MongoCursor<DownloadEntry> downloadEntries = this.collection.Find(query);
            return downloadEntries;
        }
    }
}