namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;
    using TumblrSharp2.JsonConverters;
    using TumblrSharp2.Responses.Posts;

    internal class PostCollectionResponse
    {
        [JsonConverter(typeof(PostArrayConverter))]
        [JsonProperty(PropertyName = "posts")]
        public Post[] Posts { get; set; }
    }
}
