namespace TumblrSharp2.Responses.Posts
{
    using Newtonsoft.Json;

    /// <summary>
	/// Represents a link post.
	/// </summary>
    public class LinkPost : Post
    {
		/// <summary>
		/// The title of the page the link points to.
		/// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

		/// <summary>
		/// The link.
		/// </summary>
        [JsonProperty(PropertyName = "url")]
        public string LinkUrl { get; set; }

		/// <summary>
		/// A user-supplied description.
		/// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
