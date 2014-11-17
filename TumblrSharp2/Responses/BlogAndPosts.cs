namespace TumblrSharp2.Responses
{
    using System.Collections;
    using System.Collections.Generic;
    using TumblrSharp2.Responses.Posts;

    public class BlogAndPosts : IEnumerable<Post>
    {
        public BlogAndPosts(BlogInfo blog, Paginated<Post> posts)
        {
            this.Blog = blog;
            this.Posts = posts;
        }            

        public BlogInfo Blog { get; private set; }

        public Paginated<Post> Posts { get; private set; }

        public IEnumerator<Post> GetEnumerator()
        {
            return this.Posts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Posts).GetEnumerator();
        }
    }
}
