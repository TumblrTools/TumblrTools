﻿namespace TumblrSharp2.OAuth
{
    /// <summary>
    /// Provides HMAC-SHA1 hash for signing OAuth requests.
    /// </summary>
    public interface IHashProvider
    {
        /// <summary>
        /// Gets a HMAC-SHA1 hash for an OAuth request.
        /// </summary>
        /// <param name="consumerSecret">
        /// The consumer secret.
        /// </param>
        /// <param name="oauthSecret">
        /// The OAuth secret.
        /// </param>
        /// <param name="signatureBaseString">
        /// The signature base string for which to compute the hash.
        /// </param>
        /// <returns>
        /// A HMAC-SHA1 hash of <paramref name="signatureBaseString"/>.
        /// </returns>
        string ComputeHash(string consumerSecret, string oauthSecret, string signatureBaseString);
    }
}
