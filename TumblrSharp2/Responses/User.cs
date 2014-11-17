namespace TumblrSharp2.Responses
{
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;
    using TumblrSharp2.Requests;

    /// <summary>
	/// Contains information about a user's account.
	/// </summary>
    public class User
    {
		/// <summary>
		/// The number of blogs the user is following
		/// </summary>
        [JsonProperty(PropertyName = "following")]
        public long FollowingCount { get; set; }

		/// <summary>
		/// The user's default <see cref="PostFormat"/>.
		/// </summary>
        [JsonConverter(typeof(EnumConverter))]
		[JsonProperty(PropertyName = "default_post_format")]
		public PostFormat DefaultPostFormat { get; set; }

		/// <summary>
		/// The user's tumblr short name.
		/// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

		/// <summary>
		/// The total count of the user's likes
		/// </summary>
        [JsonProperty(PropertyName = "likes")]
        public long LikesCount { get; set; }

		/// <summary>
		/// An array of <see cref="UserBlog"/> instances, containing information
		/// about the user's blogs.
		/// </summary>
        [JsonProperty(PropertyName = "blogs")]
        public UserBlog[] Blogs { get; set; }
    }
}
