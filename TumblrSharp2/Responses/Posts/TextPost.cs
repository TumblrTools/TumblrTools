namespace TumblrSharp2.Responses.Posts
{
    using Newtonsoft.Json;

    /// <summary>
	/// Represents a text post.
	/// </summary>
    public class TextPost : Post
    {
		/// <summary>
		/// The optional title of the post.
		/// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

		/// <summary>
		/// The full post body.
		/// </summary>
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
    }
}
