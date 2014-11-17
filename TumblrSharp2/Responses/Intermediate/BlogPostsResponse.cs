namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;
    using TumblrSharp2.Responses.Posts;

    /// <summary>
	/// Contains information about a blog's posts.
	/// </summary>
	public class BlogPostsResponse
    {
		/// <summary>
		/// A <see cref="BlogInfo"/> instance representing information about
		/// the blog for which the posts are being retrieved.
		/// </summary>
        [JsonProperty(PropertyName = "blog")]
        public BlogInfo Blog { get; set; }

		/// <summary>
		/// An array of <see cref="Post"/> instances, containing the 
		/// blog's posts.
		/// </summary>
		[JsonConverter(typeof(PostArrayConverter))]
		[JsonProperty(PropertyName = "posts")]
		public Post[] Result { get; set; }
    }
}
