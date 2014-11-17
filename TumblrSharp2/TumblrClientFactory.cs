namespace TumblrSharp2
{
    using System;
    using TumblrSharp2.Clients;
    using TumblrSharp2.OAuth;

    /// <summary>
    /// Factory for <see cref="TumblrRawClient"/> instances.
    /// </summary>
    public class TumblrClientFactory
    {
        /// <summary>
        /// Creates a new Tumblr client instance.
        /// </summary>
        /// <param name="consumerKey">
        /// The consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer secret.
        /// </param>
        /// <param name="oAuthToken">
        /// An optional access token for the API. If no access token is provided, only the methods
        /// that do not require OAuth can be invoked successfully.
        /// </param>
        /// <returns>
        /// A new Tumblr client instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public ITumblrClient Create(string consumerKey, string consumerSecret, Token oAuthToken = null)
        {
            ITumblrRawClient tumblrRawClient = new TumblrRawClient(
                new HmacSha1HashProvider(), 
                consumerKey, 
                consumerSecret, 
                oAuthToken);

            return new TumblrClient(
                tumblrRawClient,
                consumerKey);
        }
    }
}
