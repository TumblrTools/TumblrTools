namespace TumblrSharp2.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using TumblrSharp2.ApiMethods;
    using TumblrSharp2.OAuth;

    public interface ITumblrRawClient : IDisposable
    {
        event EventHandler<CallingApiMethodEventArgs> CallingApiMethod;

        /// <summary>
        /// Gets the OAuth <see cref="Token"/> used when the object was created.
        /// </summary>
        Token OAuthToken { get; }

        bool Disposed { get; }

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
        Task<TResult> CallApiMethodAsync<TResponse, TResult>(ApiMethod method, Func<TResponse, TResult> projection, CancellationToken cancellationToken, IEnumerable<JsonConverter> converters = null)
            where TResponse : class;

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
        Task CallApiMethodNoResultAsync(ApiMethod method, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously invokes a method on the Tumblr API.
        /// </summary>
        /// <typeparam name="TResult">
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
        /// carry a <typeparamref name="TResult"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        /// representing the error occurred during the call.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The object has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <b>null</b>.
        /// </exception>
        Task<TResult> CallApiMethodAsync<TResult>(ApiMethod method, CancellationToken cancellationToken, IEnumerable<JsonConverter> converters = null);
    }
}