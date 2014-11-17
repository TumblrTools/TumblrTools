namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;

    internal class BlogInfoResponse
    {
        [JsonProperty(PropertyName = "blog")]
        public BlogInfo Blog { get; set; }
    }
}
