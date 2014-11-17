namespace TumblrSharp2.Clients
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using TumblrSharp2.ApiMethods;
    using TumblrSharp2.ApiMethods.Parameters;
    using TumblrSharp2.OAuth;

    /// <summary>
	/// Encapsulates the Tumblr API.
	/// </summary>
	internal class TumblrRawClient : ITumblrRawClient
    {
		private readonly object disposeLock = new object();
        private bool disposed;

		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="TumblrRawClient"/> class.
		/// </summary>
        /// <param name="hashProvider">
        /// A <see cref="IHashProvider"/> implementation used to generate a
        /// HMAC-SHA1 hash for OAuth purposes.
        /// </param>
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
		/// <remarks>
		///  You can get a consumer key and a consumer secret by registering an application with Tumblr:<br/>
		/// <br/>
		/// http://www.tumblr.com/oauth/apps
		/// </remarks>
		public TumblrRawClient(IHashProvider hashProvider, string consumerKey, string consumerSecret, Token oAuthToken = null)
		{
			if (consumerKey == null)
				throw new ArgumentNullException("consumerKey");

			if (consumerKey.Length == 0)
				throw new ArgumentException("Consumer key cannot be empty.", "consumerKey");

			if (consumerSecret == null)
				throw new ArgumentNullException("consumerSecret");

			if (consumerSecret.Length == 0)
				throw new ArgumentException("Consumer secret cannot be empty.", "consumerSecret");

			this.oAuthToken = oAuthToken;
			this.client = new HttpClient(new OAuthMessageHandler(hashProvider, consumerKey, consumerSecret, oAuthToken));
		}

		#region Public Properties

		private readonly Token oAuthToken;
        public event EventHandler<CallingApiMethodEventArgs> CallingApiMethod;

        /// <summary>
		/// Gets the OAuth <see cref="Token"/> used when the object was created.
		/// </summary>
		public Token OAuthToken { get { return this.oAuthToken; } }

        public bool Disposed
        {
            get { return this.disposed; }
        }

        #endregion

		#region Public Methods

		/// <summary>
		/// Asynchronously invokes a method on the Tumblr API, and performs a projection on the
		/// response before returning the result.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the response received from the API. This must be a type that can be deserialized
		/// from the response JSON.
		/// </typeparam>
		/// <typeparam name="TResult">
		/// The actual type that is the result of the method.
		/// </typeparam>
		/// <param name="method">
		/// The <see cref="ApiMethod"/> to invoke.
		/// </param>
		/// <param name="projection">
		/// The projection function that transforms <typeparamref name="TResponse"/> into <typeparamref name="TResult"/>.
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the operation.
		/// </param>
		/// <param name="converters">
		/// An optional list of JSON converters that will be used while deserializing the response from the Tumblr API.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <typeparamref name="TResult"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="method"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="projection"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		public async Task<TResult> CallApiMethodAsync<TResponse, TResult>(ApiMethod method, Func<TResponse, TResult> projection, CancellationToken cancellationToken, IEnumerable<JsonConverter> converters = null)
            where TResponse : class
        {
			if (this.Disposed)
				throw new ObjectDisposedException("TumblrClient");

            if (method == null)
                throw new ArgumentNullException("method");

            if (projection == null)
                throw new ArgumentNullException("projection");

            var response = await this.CallApiMethodAsync<TResponse>(method, cancellationToken, converters).ConfigureAwait(false);
            return projection(response);
        }

		/// <summary>
		/// Asynchronously invokes a method on the Tumblr API without expecting a response.
		/// </summary>
		/// <param name="method">
		/// The <see cref="ApiMethod"/> to invoke.
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the operation.
		/// </param>
		/// <returns>
		///  A <see cref="Task"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		///  will carry a <see cref="TumblrException"/> representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="method"/> is <b>null</b>.
		/// </exception>
		public Task CallApiMethodNoResultAsync(ApiMethod method, CancellationToken cancellationToken)
        {
			if (this.Disposed)
				throw new ObjectDisposedException("TumblrClient");

            if (method == null)
                throw new ArgumentNullException("method");

            return this.CallApiMethodAsync<object>(method, cancellationToken);
        }

		/// <summary>
		/// Asynchronously invokes a method on the Tumblr API.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the response received from the API. This must be a type that can be deserialized
		/// from the response JSON.
		/// </typeparam>
		/// <param name="method">
		/// The <see cref="ApiMethod"/> to invoke.
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the operation.
		/// </param>
		/// <param name="converters">
		/// An optional list of JSON converters that will be used while deserializing the response from the Tumblr API.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <typeparamref name="TResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="method"/> is <b>null</b>.
		/// </exception>
        public async Task<TResponse> CallApiMethodAsync<TResponse>(ApiMethod method, CancellationToken cancellationToken, IEnumerable<JsonConverter> converters = null)
        {
			if (this.Disposed)
				throw new ObjectDisposedException("TumblrClient");

            if (method == null)
                throw new ArgumentNullException("method");

            //build the api call URL
		    string apiRequestUrl = method.ApiRequestUrl();

            using (HttpRequestMessage request = new HttpRequestMessage(method.HttpMethod, apiRequestUrl))
            {
                if (method.OAuthToken != null)
                    method.Parameters.Add("oauth_token", method.OAuthToken.Key);

                if (method.Parameters.Any(c => c is BinaryMethodParameter))
                {
                    //if there is binary content we submit a multipart form request
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    foreach (var p in method.Parameters)
                        content.Add(p.AsHttpContent());

                    request.Content = content;
                }
                else
                {
                    //otherwise just a form url encoded request
                    FormUrlEncodedContent content = new FormUrlEncodedContent(method.Parameters.Select(c => new KeyValuePair<string, string>(c.Name, ((StringMethodParameter)c).Value)));
                    request.Content = content;
                }

                if (CallingApiMethod != null)
                {
                    CallingApiMethod(this, new CallingApiMethodEventArgs(apiRequestUrl));
                }

                using (HttpResponseMessage response = await this.client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using (var reader = new JsonTextReader(new StreamReader(stream)))
                    {
                        JsonSerializer serializer = this.CreateSerializer(converters);
                        if (response.IsSuccessStatusCode)
                        {
                            return serializer.Deserialize<TumblrRawResponse<TResponse>>(reader).Response;
                        }

                        string[] errorResponse = 
                            (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
                            ? new string[0] 
                            : serializer.Deserialize<TumblrRawResponse<string[]>>(reader).Response;
                        throw new TumblrException(response.StatusCode, response.ReasonPhrase, errorResponse);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private JsonSerializer CreateSerializer(IEnumerable<JsonConverter> converters)
        {
            JsonSerializer serializer = new JsonSerializer();
            if (converters != null)
            {
                foreach (JsonConverter converter in converters)
                    serializer.Converters.Add(converter);
            }

            return serializer;
        }

        #endregion

        #region IDisposable Implementation

		/// <summary>
		/// Disposes of the object and the internal HttpClient instance.
		/// </summary>
        public void Dispose()
		{
			if (!this.Disposed)
			{
				lock (this.disposeLock)
				{
					if (!this.Disposed)
					{
						this.disposed = true;
						this.client.Dispose();
						this.Dispose(true);
						GC.SuppressFinalize(this);
					}
				}
			}
        }

		/// <summary>
		/// Subclasses can override this method to provide custom
		/// disposing logic.
		/// </summary>
		/// <param name="disposing">
		/// <b>true</b> if managed resources have to be disposed; otherwise <b>false</b>.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{ }

        #endregion

        #region Nested classes

        private class TumblrRawResponse<TResponse>
        {
            [JsonProperty(PropertyName = "meta")]
            public TumblrResponseStatus Status { get; set; }

            [JsonProperty(PropertyName = "response")]
            public TResponse Response { get; set; }
        }

        private class TumblrResponseStatus
        {
            public TumblrResponseStatus()
                : this(0, null)
            { }

            public TumblrResponseStatus(int code, string message)
            {
                this.Code = code;
                this.Message = message;
            }

            [JsonProperty(PropertyName = "status")]
            public int Code { get; set; }

            [JsonProperty(PropertyName = "msg")]
            public string Message { get; set; }
        }

        #endregion
    }
}
