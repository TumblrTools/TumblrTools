namespace TumblrSharp2.OAuth
{
    using System;
    using System.Threading.Tasks;

    public interface IOAuthClient
    {
        /// <summary>
        /// Asynchronously performs XAuth.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The user password.
        /// </param>
        /// <returns>
        /// The access <see cref="Token"/>.
        /// </returns>
        /// <remarks>
        /// XAuth is mainly used in mobile applications, where the device does not (or can not) have a 
        /// callback url. It uses the user name and password to get the access token from the server.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			<paramref name="userName"/> is <b>null</b>.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			<paramref name="password"/> is <b>null</b>.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			<paramref name="userName"/> is empty.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			<paramref name="password"/> is empty.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        /// <exception cref="OAuthException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			Could not determine oauth_token and oauth_token_secret from server response.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			An exception occurred during the method call.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        Task<Token> PerformXAuthAsync(string userName, string password);

        /// <summary>
        /// Asynchronously gets a request token.
        /// </summary>
        /// <param name="callbackUrl">
        /// The server redirects Users to this URL after they authorize access to their private data.
        /// </param>
        /// <returns>
        /// The request token.
        /// </returns>
        /// <remarks>
        /// The Consumer obtains an unauthorized Request Token by asking the Service Provider to issue a Token. 
        /// The Request Token’s sole purpose is to receive User approval and can only be used to obtain an Access Token.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="callbackUrl"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="callbackUrl"/> is empty.
        /// </exception>
        /// <exception cref="OAuthException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			Could not determine oauth_token and oauth_token_secret from server response.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			An exception occurred during the method call.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        Task<Token> GetRequestTokenAsync(string callbackUrl);

        /// <summary>
        /// Builds the url that is required to connect to the server, where the server will authenticate the user
        /// and ask for authorization.
        /// </summary>
        /// <param name="requestToken">
        /// The request token obtained during the call to <see cref="GetRequestTokenAsync"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/> where to direct the user to obtain authorization.
        /// </returns>
        /// <remarks>
        /// After the User authenticates with the Service Provider and grants permission for Consumer access, the Consumer will be 
        /// notified that the Request Token has been authorized and ready to be exchanged for an Access Token. The Service Provider 
        /// will construct an HTTP GET request URL, and redirects the User’s web browser to that URL with the following parameters: 
        /// <b>oauth_token</b> which is the request token and <b>oauth_verifier</b> which is the verification code tied to the request token.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			<paramref name="requestToken"/> is <b>null</b>.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			<paramref name="requestToken"/>.Key is <b>null</b>.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="requestToken"/>.Key is empty.
        /// </exception>
        Uri GetAuthorizeUrl(Token requestToken);

        /// <summary>
        /// Gets the authorized access token that can be used to make OAuth calls.
        /// </summary>
        /// <param name="requestToken">
        /// The request token sent from the server to the <b>callback url</b>.
        /// </param>
        /// <param name="verifierUrl">
        /// The verifier url returned from the server.
        /// </param>
        /// <returns>
        /// The access token.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			<paramref name="requestToken"/> is <b>null</b>.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			<paramref name="verifierUrl"/> is <b>null</b>.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="verifierUrl"/> is empty.
        /// </exception>
        /// <exception cref="OAuthException">
        /// <list type="bullet">
        /// <item>
        ///		<description>
        ///			Could not determine oauth_token and oauth_token_secret from server response.
        ///		</description>
        ///	</item>
        ///	<item>
        ///		<description>
        ///			An exception occurred during the method call.
        ///		</description>
        ///	</item>
        /// </list>
        /// </exception>
        Task<Token> GetAccessTokenAsync(Token requestToken, string verifier);
    }
}