namespace TumblrTools.Domain
{
    using System.Collections.Generic;
    using TumblrSharp2.Responses.Posts;

    public interface IPostRepository
    {
        IEnumerable<PhotoPost> GetAll();
        void Remove(PhotoPost post);
        void Add(PhotoPost post);
        IList<PhotoPost> GetAllSortedByBlogId();

        IList<PhotoPost> GetDownloadedToday();

        PhotoPost GetById(long id);
        IEnumerable<PhotoPost> GetNewestFrom(long? id, int size);
        IList<PhotoPost> GetFromBlog(string blogId);
        void Update(PhotoPost post);
    }
}