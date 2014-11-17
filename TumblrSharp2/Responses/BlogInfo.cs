namespace TumblrSharp2.Responses
{
    using System;
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;

    /// <summary>
	/// Contains information about a blog.
	/// </summary>
    public class BlogInfo
    {
        /// <summary>
        /// The display title of the blog.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The short blog name that appears before tumblr.com in a 
        /// standard blog hostname (and before the domain in a custom blog hostname).
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The blog url.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The date and time when the blog was last updated (in local time).
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        [JsonProperty(PropertyName = "updated")]
        public DateTime LastUpdated { get; set; }

		/// <summary>
		/// The total number of posts to this blog.
		/// </summary>
        [JsonProperty(PropertyName = "posts")]
        public long PostsCount { get; set; }

		/// <summary>
		/// The blog's description.
		/// </summary>
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
        
		/// <summary>
		/// Indicates whether the blog allows questions.
		/// </summary>
        [JsonProperty(PropertyName = "ask")]
        public bool IsAskEnabled { get; set; }

		/// <summary>
		/// Indicates whether the blog allows anonymous questions. Returned only if 
		/// <see cref="IsAskEnabled"/> is <b>true</b>.
		/// </summary>
        [JsonProperty(PropertyName = "ask_anon")]
        public bool IsAnonAskEnabled { get; set; }

		/// <summary>
		/// Number of likes for this user. Returned only if this is 
		/// the user's primary blog and sharing of likes is enabled.
		/// </summary>
        [JsonProperty(PropertyName = "likes")]
        public long LikesCount { get; set; }

		/// <summary>
		/// Whether the blog is marked as NSFW or not.
		/// </summary>
		[JsonProperty(PropertyName = "is_nsfw")]
		public bool IsNsfw { get; set; }
	}
}
