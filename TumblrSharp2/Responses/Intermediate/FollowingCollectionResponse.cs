﻿namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;

    /// <summary>
	/// Contains the blogs that a user is following.
	/// </summary>
    public class FollowingCollectionResponse
    {
		/// <summary>
		/// The number of blogs the user is following.
		/// </summary>
        [JsonProperty(PropertyName = "total_blogs")]
        public int Count { get; set; }

		/// <summary>
        /// An array of <see cref="Following"/> instances, representing information
		/// about each followed blog.
		/// </summary>
        [JsonProperty(PropertyName = "blogs")]
        public Following[] Result { get; set; }
    }
}
