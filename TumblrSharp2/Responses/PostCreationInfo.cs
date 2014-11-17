namespace TumblrSharp2.Responses
{
    using Newtonsoft.Json;

    /// <summary>
	/// Represents information about a newly created post.
	/// </summary>
    public class PostCreationInfo
    {
		/// <summary>
		/// The identifier of the post.
		/// </summary>
        [JsonProperty("id")]
        public long PostId { get; set; }
    }
}
