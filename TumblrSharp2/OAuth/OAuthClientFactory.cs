namespace TumblrSharp2.OAuth
{
    /// <summary>
    /// Factory for <see cref="OAuthClient"/> instances.
    /// </summary>
    public class OAuthClientFactory
    {
        /// <summary>
        /// Creates a new <see cref="OAuthClient"/> instance.
        /// </summary>
        /// <param name="consumerKey">
        /// The consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer secret.
        /// </param>
        /// <returns>
        /// A <see cref="OAuthClient"/> instance.
        /// </returns>
        public IOAuthClient Create(string consumerKey, string consumerSecret)
        {
            return new OAuthClient(new HmacSha1HashProvider(), consumerKey, consumerSecret);
        }
    }
}
