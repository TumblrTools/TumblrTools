namespace TumblrSharp2.Responses
{
    using System;
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;

    /// <summary>
    /// Contains information about a user's blog.
    /// </summary>
    public class Following
    {
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
        /// The display title of the blog.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The blog's description.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}