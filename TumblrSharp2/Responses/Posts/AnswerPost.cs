namespace TumblrSharp2.Responses.Posts
{
    using Newtonsoft.Json;

    /// <summary>
	/// Represents an answer post.
	/// </summary>
    public class AnswerPost : Post
    {
		/// <summary>
		/// Gets or sets the name of the user asking the question.
		/// </summary>
        [JsonProperty("asking_name")]
        public string AskingName { get; set; }

		/// <summary>
		/// Gets or sets the url of the blog of the user asking the question.
		/// </summary>
        [JsonProperty("asking_url")]
        public string AskingUrl { get; set; }

		/// <summary>
		/// Gets or sets tquestion.
		/// </summary>
        [JsonProperty("question")]
        public string Question { get; set; }

		/// <summary>
		/// Gets or sets the answer.
		/// </summary>
        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
