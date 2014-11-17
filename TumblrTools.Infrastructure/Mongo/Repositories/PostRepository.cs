namespace TumblrTools.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using TumblrSharp2;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Domain;

    public class PostRepository : IPostRepository
    {
        private readonly MongoCollection<PhotoPost> collection;

        public PostRepository(MongoCollection<PhotoPost> collection)
        {
            this.collection = collection;
        }

        public IEnumerable<PhotoPost> GetAll()
        {
            return this.collection.FindAll();
        }

        public void Remove(PhotoPost post)
        {
            
            IMongoQuery query = Query<PhotoPost>.EQ(e => e.Id, post.Id);
            this.collection.Remove(query);
        }

        public IList<PhotoPost> GetAllSortedByBlogId()
        {
            throw new NotImplementedException();
            // return this.collection.DownloadFiles.OrderBy(file => file.DownloadFileId.BlogId).ToList();
        }

        public PhotoPost GetById(long id)
        {
            IMongoQuery query = Query<PhotoPost>.EQ(e => e.Id, id);
            return this.collection.FindOne(query);
        }

        public IEnumerable<PhotoPost> GetNewestFrom(long? id, int size)
        {
            IMongoQuery query;
            if (id.HasValue)
            {
                query = Query.And(
                    Query<PhotoPost>.LT(p => p.Id, id),
                    Query<PhotoPost>.NE(p => p.State, PostCreationState.Draft),
                    Query<PhotoPost>.EQ(p => p.RebloggedFromId, null));
            }
            else
            {
                query = Query.And(
                    Query<PhotoPost>.NE(p => p.State, PostCreationState.Draft),
                    Query<PhotoPost>.EQ(p => p.RebloggedFromId, null));
            }

            IMongoSortBy sortBy = SortBy<PhotoPost>.Descending(e => e.Id);
            return this.collection.Find(query).SetSortOrder(sortBy).Take(size).ToList();
        }

        public IList<PhotoPost> GetFromBlog(string blogId)
        {
            IMongoQuery query = Query<PhotoPost>.EQ(p => p.BlogName, blogId);

            return this.collection.Find(query).ToList();
        }

        public void Add(PhotoPost post)
        {
            this.collection.Insert(post);
        }

        public void Update(PhotoPost post)
        {
            this.collection.Save(post);
        }

        public void Remove(long postId)
        {
            IMongoQuery query = Query<PhotoPost>.EQ(x => x.Id, postId);
            this.collection.Remove(query);
        }

        public Post GetOneByPostId(long postId)
        {
            IMongoQuery query = Query<PhotoPost>.EQ(x => x.Id, postId);
            return this.collection.FindOne(query);
        }

        public IList<PhotoPost> GetDownloadedToday()
        {
            // DateTime limit = DateTime.Now.Subtract(TimeSpan.FromDays(2));
            throw new NotImplementedException();
            // return this.collection.DownloadFiles.Where(df => df.DownloadDate >= limit).ToList();
        }
    }
}