namespace TumblrSharp2.Responses.Intermediate
{
    using Newtonsoft.Json;

    internal class UserInfoResponse
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }
    }
}
