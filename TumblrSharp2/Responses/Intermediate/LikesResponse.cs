namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;
    using TumblrSharp2.Responses.Posts;

    /// <summary>
	/// Contains the user's likes.
	/// </summary>
	internal class LikesResponse
    {
		/// <summary>
		/// Total number of liked posts.
		/// </summary>
		[JsonProperty(PropertyName = "liked_count")]
		public long Count { get; set; }

		/// <summary>
		/// An array of <see cref="Post"/> instances, representing
		/// the liked posts.
		/// </summary>
		[JsonConverter(typeof(PostArrayConverter))]
		[JsonProperty(PropertyName = "liked_posts")]
        public Post[] Result { get; set; }
    }
}
