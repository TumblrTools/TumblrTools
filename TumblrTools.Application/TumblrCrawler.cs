namespace TumblrTools.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Polly;
    using TumblrSharp2;
    using TumblrSharp2.Clients;
    using TumblrSharp2.Requests;
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Generic;

    public class TumblrCrawler
    {
        private readonly ILogger logger;
        private readonly ITumblrClient client;

        public TumblrCrawler(ILogger logger, ITumblrClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public IEnumerable<TPost> AllPosts<TPost>(string blogId, IList<string> tags, PostType postType, bool includeReblogInfo) where TPost : Post
        {
            int offset = 0;
            BlogAndPosts blogPosts = null;

            do
            {
                Pagination pagination = new Pagination(offset);

                BlogAndPosts posts = Policy.Handle<Exception>()
                    .Retry(3, (exception, retryCount) => this.logger.Warn("{0}. Retrying...", exception.Message))
                    .Execute(
                        () => this.client.GetPostsAsync(blogId, pagination, PostType.Photo, includeReblogInfo).Result);

                foreach (TPost post in posts.Cast<TPost>())
                {
                    yield return post;
                }

                offset += 20;
            } while (blogPosts.Count() == 20);
        }
    }
}