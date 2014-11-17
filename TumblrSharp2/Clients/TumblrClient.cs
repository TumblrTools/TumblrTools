namespace TumblrSharp2.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using TumblrSharp2.ApiMethods;
    using TumblrSharp2.ApiMethods.Parameters;
    using TumblrSharp2.JsonConverters;
    using TumblrSharp2.Requests;
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Intermediate;
    using TumblrSharp2.Responses.Posts;
    using TumblrSharp2.Utils;

    /// <summary>
	/// Encapsulates the Tumblr API.
	/// </summary>
	internal class TumblrClient : ITumblrClient
    {
        private readonly string apiKey;
        private readonly ITumblrRawClient rawClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TumblrClient"/> class.
        /// </summary>
        /// <param name="rawClient">
        /// </param>
        /// <param name="apiKey">
        /// The API consumer key
        /// </param>
        /// <remarks>
        ///  You can get a consumer key and a consumer secret by registering an application with Tumblr:<br/>
        /// <br/>
        /// http://www.tumblr.com/oauth/apps
        /// </remarks>
        public TumblrClient(ITumblrRawClient rawClient, string apiKey)
		{
            this.rawClient = rawClient;
		    this.apiKey = apiKey;
		}

		#region Public Methods

		#region Blog Methods

		#region GetBlogInfoAsync

        public ITumblrRawClient RawClient { get { return this.rawClient; } }

        /// <summary>
		/// Asynchronously retrieves general information about the blog, such as the title, number of posts, and other high-level data.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#blog-info.
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="BlogInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogName"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		/// </exception>
		public Task<BlogInfo> GetBlogInfoAsync(string blogName)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			MethodParameterSet parameters = new MethodParameterSet {{"api_key", this.apiKey}};

            return this.rawClient.CallApiMethodAsync<BlogInfoResponse, BlogInfo>(
                new BlogMethod(blogName, "info", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
				r => r.Blog,
				CancellationToken.None);
		}

		#endregion

		#region GetPostAsync

        public Task<TPost> GetPostAsync<TPost>(
            string blogName,
            long postId) where TPost : Post
        {
            const string methodName = "posts";

            MethodParameterSet parameters = new MethodParameterSet();
            parameters.Add("api_key", this.apiKey);
            parameters.Add("id", postId);

            return this.rawClient.CallApiMethodAsync<PostCollectionResponse, TPost>(
                new BlogMethod(blogName, methodName, null, HttpMethod.Get, parameters),
                response => (TPost)response.Posts.SingleOrDefault(),
                CancellationToken.None);            
        }

		/// <summary>
		/// Asynchronously retrieves published posts from a blog.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#posts
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog.
		/// </param>
		/// <param name="pagination">
		/// Pagination configuration.
		/// </param>
		/// <param name="type">
		/// The <see cref="PostType"/> to retrieve.
		/// </param>
		/// <param name="includeReblogInfo">
		/// Whether or not to include reblog info with the posts.
		/// </param>
		/// <param name="includeNotesInfo">
		/// Whether or not to include notes info with the posts.
		/// </param>
		/// <param name="filter">
		/// A <see cref="PostFilter"/> to apply.
		/// </param>
        /// <param name="tag">
        /// A tag to filter by.
        /// </param>
        /// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry a <see cref="BlogAndPosts"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogName"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		/// </exception>
		public Task<BlogAndPosts> GetPostsAsync(string blogName, Pagination pagination, PostType type = PostType.All, bool includeReblogInfo = false, bool includeNotesInfo = false, PostFilter filter = PostFilter.Html, string tag = null)
		{
            if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (pagination == null)
                throw new ArgumentNullException("pagination");

			string methodName = PostTypeMethodName(type);

		    MethodParameterSet parameters = new MethodParameterSet
		    {
		        {"api_key",     this.apiKey},
		        {"offset",      pagination.StartIndex,                0},
		        {"limit",       pagination.Count,                     0},
		        {"reblog_info", includeReblogInfo,                    false},
		        {"notes_info",  includeNotesInfo,                     false},
		        {"filter",      filter.ToString().ToLowerInvariant(), "html"},
		        {"tag",         tag}
		    };

		    return this.rawClient.CallApiMethodAsync<BlogPostsResponse, BlogAndPosts>(
				new BlogMethod(blogName, methodName, null, HttpMethod.Get, parameters),
                response => new BlogAndPosts(response.Blog, new Paginated<Post>(response.Result, response.Blog.PostsCount, pagination.StartIndex)),
				CancellationToken.None);
		}

        private static string PostTypeMethodName(PostType type)
        {
            string methodName = null;
            switch (type)
            {
                case PostType.Text:
                    methodName = "posts/text";
                    break;
                case PostType.Quote:
                    methodName = "posts/quote";
                    break;
                case PostType.Link:
                    methodName = "posts/link";
                    break;
                case PostType.Answer:
                    methodName = "posts/answer";
                    break;
                case PostType.Video:
                    methodName = "posts/video";
                    break;
                case PostType.Audio:
                    methodName = "posts/audio";
                    break;
                case PostType.Photo:
                    methodName = "posts/photo";
                    break;
                case PostType.Chat:
                    methodName = "posts/chat";
                    break;
                case PostType.All:
                default:
                    methodName = "posts";
                    break;
            }
            return methodName;
        }

        #endregion

		#region GetBlogLikesAsync

        ///  <summary>
        ///  Asynchronously retrieves the publicly exposed likes from a blog.
        ///  </summary>
        ///  <remarks>
        ///  See: http://www.tumblr.com/docs/en/api/v2#blog-likes
        ///  </remarks>
        ///  <param name="blogName">
        ///  The name of the blog.
        ///  </param>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///  A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry a <see cref="Paginated{Post}"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        ///  representing the error occurred during the call.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="ArgumentNullException">
        ///  <paramref name="blogName"/> is <b>null</b>.
        ///  </exception>
        ///  <exception cref="ArgumentException">
        ///  <paramref name="blogName"/> is empty.
        ///  </exception>
        public Task<Paginated<Post>> GetBlogLikesAsync(string blogName, Pagination pagination)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("api_key", this.apiKey);
            AddPaginationToParameters(pagination, parameters);

			return this.rawClient.CallApiMethodAsync<LikesResponse, Paginated<Post>>(
				new BlogMethod(blogName, "likes", null, HttpMethod.Get, parameters),
                response => new Paginated<Post>(response.Result, response.Count, pagination.StartIndex), 
				CancellationToken.None);
		}

		#endregion

		#region GetFollowersAsync

        ///  <summary>
        ///  Asynchronously retrieves a blog's followers.
        ///  </summary>
        ///  <remarks>
        ///  See: http://www.tumblr.com/docs/en/api/v2#blog-followers
        ///  </remarks>
        ///  <param name="blogName">
        ///  The name of the blog.
        ///  </param>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///   A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry a <see cref="Paginated{Follower}"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        ///  A <see cref="Paginated{Follower}"/> instance.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="ArgumentNullException">
        ///  <paramref name="blogName"/> is <b>null</b>.
        ///  </exception>
        ///  <exception cref="ArgumentException">
        ///  <paramref name="blogName"/> is empty.
        ///  </exception>
        public Task<Paginated<Follower>> GetFollowersAsync(string blogName, Pagination pagination)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetFollowersAsync method requires an OAuth token to be specified.");

            MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);

            return this.rawClient.CallApiMethodAsync<FollowersCollectionResponse, Paginated<Follower>>(
				new BlogMethod(blogName, "followers", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => new Paginated<Follower>(response.Result, response.Count, pagination.StartIndex),
				CancellationToken.None);
		}

		#endregion

		#region CreatePostAsync

		/// <summary>
		/// Asynchronously creates a new post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#posting
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog where to post to (must be one of the current user's blogs).
		/// </param>
		/// <param name="postData">
		/// The data that represents the type of post to create. See <see cref="PostData"/> for how
		/// to create various post types.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="PostCreationInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postData"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task<long> CreatePostAsync(string blogName, PostData postData)
		{
			return this.CreatePostAsync(blogName, postData, CancellationToken.None);
		}

		/// <summary>
		/// Asynchronously creates a new post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#posting
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog where to post to (must be one of the current user's blogs).
		/// </param>
		/// <param name="postData">
		/// The data that represents the type of post to create. See <see cref="PostData"/> for how
		/// to create various post types.
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="PostCreationInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postData"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
        public Task<long> CreatePostAsync(string blogName, PostData postData, CancellationToken cancellationToken)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (postData == null)
				throw new ArgumentNullException("postData");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("CreatePostAsync method requires an OAuth token to be specified.");

		    return this.rawClient.CallApiMethodAsync<PostCreationInfo, long>(
		        new BlogMethod(blogName, "post", this.rawClient.OAuthToken, HttpMethod.Post, postData.ToMethodParameterSet()),
		        response => response.PostId,
				cancellationToken);
		}

		#endregion

		#region EditPostAsync

		/// <summary>
		/// Asynchronously edits an existing post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#editing
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog where the post to edit is (must be one of the current user's blogs).
		/// </param>
		/// <param name="postId">
		/// The identifier of the post to edit.
		/// </param>
		/// <param name="postData">
		/// The data that represents the updated information for the post. See <see cref="PostData"/> for how
		/// to create various post types.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="PostCreationInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postData"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is empty.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postId"/> is less than 0.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task<long> EditPostAsync(string blogName, long postId, PostData postData)
		{
			return this.EditPostAsync(blogName, postId, postData, CancellationToken.None);
		}

		/// <summary>
		/// Asynchronously edits an existing post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#editing
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog where the post to edit is (must be one of the current user's blogs).
		/// </param>
		/// <param name="postId">
		/// The identifier of the post to edit.
		/// </param>
		/// <param name="postData">
		/// The data that represents the updated information for the post. See <see cref="PostData"/> for how
		/// to create various post types.
		/// </param>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="PostCreationInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postData"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is empty.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="postId"/> is less than 0.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
        public Task<long> EditPostAsync(string blogName, long postId, PostData postData, CancellationToken cancellationToken)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (postId < 0)
				throw new ArgumentOutOfRangeException("postId", "Post ID must be greater or equal to zero.");

			if (postData == null)
				throw new ArgumentNullException("postData");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("EditPostAsync method requires an OAuth token to be specified.");

			var parameters = postData.ToMethodParameterSet();
			parameters.Add("id", postId);

			return this.rawClient.CallApiMethodAsync<PostCreationInfo, long>(
				new BlogMethod(blogName, "post/edit", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
                response => response.PostId,
				CancellationToken.None);
		}

		#endregion

		#region ReblogAsync

		/// <summary>
		/// Asynchronously reblogs a post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#reblogging
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog where to reblog the psot (must be one of the current user's blogs).
		/// </param>
		/// <param name="postId">
		/// The identifier of the post to reblog.
		/// </param>
		/// <param name="reblogKey">
		/// The post reblog key.
		/// </param>
		/// <param name="comment">
		/// An optional comment to add to the reblog.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="PostCreationInfo"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is <b>null</b>.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="reblogKey"/> is <b>null</b>.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// /// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="blogName"/> is empty.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="reblogKey"/> is empty.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
        public Task<long> ReblogAsync(string blogName, long postId, string reblogKey, string comment = null)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (postId <= 0)
				throw new ArgumentException("Post ID must be greater than 0.", "postId");

			if (reblogKey == null)
				throw new ArgumentNullException("reblogKey");

			if (reblogKey.Length == 0)
				throw new ArgumentException("reblogKey cannot be empty.", "reblogKey");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("ReblogAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("id", postId);
			parameters.Add("reblog_key", reblogKey);
			parameters.Add("comment", comment, null);

			return this.rawClient.CallApiMethodAsync<PostCreationInfo, long>(
				new UserMethod("post/reblog", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
                response => response.PostId,
				CancellationToken.None);
		}

		#endregion

		#region GetQueuedPostsAsync

        ///  <summary>
        ///  Asynchronously returns posts in the current user's queue.
        ///  </summary>
        ///  <remarks>
        ///  See: http://www.tumblr.com/docs/en/api/v2#blog-queue
        ///  </remarks>
        ///  <param name="blogName">
        ///  The name of the blog for which to retrieve queued posts.
        ///  </param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="filter">
        ///  A <see cref="PostFilter"/> to apply.
        ///  </param>
        ///  <returns>
        ///  A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        ///  representing the error occurred during the call.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="ArgumentNullException">
        ///  <paramref name="blogName"/> is <b>null</b>.
        /// 	</exception>
        ///  <exception cref="ArgumentException">
        ///  <paramref name="blogName"/> is empty.
        /// 	</exception>
        ///  <exception cref="InvalidOperationException">
        ///  This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
        ///  </exception>
        public Task<IEnumerable<Post>> GetQueuedPostsAsync(string blogName, Pagination pagination, PostFilter filter = PostFilter.Html)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetQueuedPostsAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);
			parameters.Add("filter", filter.ToString().ToLowerInvariant(), "html");

			return this.rawClient.CallApiMethodAsync<PostCollectionResponse, IEnumerable<Post>>(
				new BlogMethod(blogName, "posts/queue",this.rawClient.OAuthToken, HttpMethod.Get, parameters),
				response => response.Posts, 
				CancellationToken.None);
		}

		#endregion

		#region GetDraftPostsAsync

		/// <summary>
		/// Asynchronously returns draft posts.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#blog-drafts
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog for which to retrieve drafted posts. 
		/// </param>
		/// <param name="sinceId">
		/// Return posts that have appeared after the specified ID. Use this parameter to page through 
		/// the results: first get a set of posts, and then get posts since the last ID of the previous set. 
		/// </param>
		/// <param name="filter">
		/// A <see cref="PostFilter"/> to apply.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogName"/> is <b>null</b>.
		///	</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		///	</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="sinceId"/> is less than 0.
		///	</exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
        public Task<IEnumerable<Post>> GetDraftPostsAsync(string blogName, long sinceId = 0, PostFilter filter = PostFilter.Html)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (sinceId < 0)
				throw new ArgumentOutOfRangeException("sinceId", "sinceId must be greater or equal to zero.");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetDraftPostsAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("since_id", sinceId, 0);
			parameters.Add("filter", filter.ToString().ToLowerInvariant(), "html");

            return this.rawClient.CallApiMethodAsync<PostCollectionResponse, IEnumerable<Post>>(
				new BlogMethod(blogName, "posts/draft", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => response.Posts,
				CancellationToken.None);
		}

		#endregion

		#region GetSubmissionPostsAsync

		/// <summary>
		/// Asynchronously retrieves submission posts.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#blog-submissions
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog for which to retrieve submission posts. 
		/// </param>
		/// <param name="startIndex">
		/// The post number to start at. Pass 0 to start from the first post.
		/// </param>
		/// <param name="filter">
		/// A <see cref="PostFilter"/> to apply.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogName"/> is <b>null</b>.
		///	</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		///	</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than 0.
		///	</exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
        public Task<IEnumerable<Post>> GetSubmissionPostsAsync(string blogName, long startIndex = 0, PostFilter filter = PostFilter.Html)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException("startIndex", "startIndex must be greater or equal to zero.");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetSubmissionPostsAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("offset", startIndex);
			parameters.Add("filter", filter.ToString().ToLowerInvariant(), "html");

			return this.rawClient.CallApiMethodAsync<PostCollectionResponse, IEnumerable<Post>>(
				new BlogMethod(blogName, "posts/submission", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => response.Posts,
				CancellationToken.None);
		}

		#endregion

		#region DeletePostAsync

		/// <summary>
		/// Asynchronously deletes a post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#deleting-posts
		/// </remarks>
		/// <param name="blogName">
		/// The name of the blog to which the post to delete belongs.
		/// </param>
		/// <param name="postId">
		/// The identifier of the post to delete.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		/// will carry a <see cref="TumblrException"/> representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogName"/> is <b>null</b>.
		///	</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogName"/> is empty.
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	<paramref name="postId"/> is less than 0.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task DeletePostAsync(string blogName, long postId)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogName == null)
				throw new ArgumentNullException("blogName");

			if (blogName.Length == 0)
				throw new ArgumentException("Blog name cannot be empty.", "blogName");

			if (postId < 0)
				throw new ArgumentOutOfRangeException("postId", "Post ID must be greater or equal to zero.");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("DeletePostAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("id", postId);

			return this.rawClient.CallApiMethodNoResultAsync(
				new BlogMethod(blogName, "post/delete", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
				CancellationToken.None);
		}

		#endregion

		#endregion

		#region User Methods

		#region GetUserInfoAsync

		/// <summary>
		/// Asynchronously retrieves the user's account information that matches the OAuth credentials submitted with the request.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#user-methods
		/// </remarks>
		/// <returns>
		/// A <see cref="Task"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="User"/> instance. Otherwise <see cref="Task.Exception"/> will carry the <see cref="TumblrException"/>
		/// generated during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task<User> GetUserInfoAsync()
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetUserInfoAsync method requires an OAuth token to be specified.");

			return this.rawClient.CallApiMethodAsync<UserInfoResponse, User>(
				new UserMethod("info", this.rawClient.OAuthToken, HttpMethod.Get),
				r => r.User,
				CancellationToken.None);
		}

		#endregion

		#region GetFollowingAsync

		/// <summary>
		/// Asynchronously retrieves the blog that the current user is following.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-ug-following
		/// </remarks>
		/// <param name="startIndex">
		/// The offset at which to start retrieving the followed blogs. Use 0 to start retrieving from the latest followed blog.
		/// </param>
		/// <param name="count">
		/// The number of following blogs to retrieve. Must be between 1 and 20.
		/// </param>
		/// <returns>
		/// A <see cref="Task"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry a <see cref="FollowingCollection"/> instance. Otherwise <see cref="Task.Exception"/> will carry the <see cref="TumblrException"/>
		/// generated during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <list type="bullet">
		/// <item>
		///		<description>
		///			<paramref name="startIndex"/> is less than 0.
		///		</description>
		///	</item>
		///	<item>
		///		<description>
		///			<paramref name="count"/> is less than 1 or greater than 20.
		///		</description>
		///	</item>
		/// </list>
		/// </exception>
        public Task<Paginated<Following>> GetFollowingAsync(Pagination pagination)
		{
		    if (pagination == null)
		    {
		        throw new ArgumentNullException("pagination");
		    }

		    if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetFollowingAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);

		    return this.rawClient.CallApiMethodAsync<FollowingCollectionResponse, Paginated<Following>>(
				new UserMethod("following", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => new Paginated<Following>(response.Result, response.Count, pagination.StartIndex),
				CancellationToken.None);
		}

        #endregion

		#region GetLikesAsync

        ///  <summary>
        ///  Asynchronously retrieves the current user's likes.
        ///  </summary>
        ///  <remarks>
        ///  See: http://www.tumblr.com/docs/en/api/v2#m-ug-likes
        ///  </remarks>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///  A <see cref="Task"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry a <see cref="Likes"/> instance. Otherwise <see cref="Task.Exception"/> will carry the <see cref="TumblrException"/>
        ///  generated during the call.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="InvalidOperationException">
        ///  This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
        ///  </exception>
        public Task<Paginated<Post>> GetLikesAsync(Pagination pagination)
		{
            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetLikesAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);

			return this.rawClient.CallApiMethodAsync<LikesResponse, Paginated<Post>>(
				new UserMethod("likes", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => new Paginated<Post>(response.Result, response.Count, pagination.StartIndex), 
				CancellationToken.None);
		}

		#endregion

		#region LikeAsync

		/// <summary>
		/// Asynchronously likes a post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-up-like
		/// </remarks>
		/// <param name="postId">
		/// The identifier of the post to like.
		/// </param>
		/// <param name="reblogKey">
		/// The reblog key for the post.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		/// will carry a <see cref="TumblrException"/>
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="reblogKey"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="reblogKey"/> is empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="postId"/> is less than 0.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task LikeAsync(long postId, string reblogKey)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (postId <= 0)
				throw new ArgumentOutOfRangeException("Post ID must be greater than 0.", "postId");

			if (reblogKey == null)
				throw new ArgumentNullException("reblogKey");

			if (reblogKey.Length == 0)
				throw new ArgumentException("reblogKey cannot be empty.", "reblogKey");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("LikeAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("id", postId);
			parameters.Add("reblog_key", reblogKey);

			return this.rawClient.CallApiMethodNoResultAsync(
				new UserMethod("like", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
				CancellationToken.None);
		}

		#endregion

		#region UnlikeAsync

		/// <summary>
		/// Asynchronously unlikes a post.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-up-unlike
		/// </remarks>
		/// <param name="postId">
		/// The identifier of the post to like.
		/// </param>
		/// <param name="reblogKey">
		/// The reblog key for the post.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		/// will carry a <see cref="TumblrException"/>
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="reblogKey"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="reblogKey"/> is empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="postId"/> is less than 0.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task UnlikeAsync(long postId, string reblogKey)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (postId <= 0)
				throw new ArgumentException("Post ID must be greater than 0.", "postId");

			if (reblogKey == null)
				throw new ArgumentNullException("reblogKey");

			if (reblogKey.Length == 0)
				throw new ArgumentException("reblogKey cannot be empty.", "reblogKey");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("UnlikeAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("id", postId);
			parameters.Add("reblog_key", reblogKey);

			return this.rawClient.CallApiMethodNoResultAsync(
				new UserMethod("unlike", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
				CancellationToken.None);
		}

		#endregion

		#region FollowAsync

		/// <summary>
		/// Asynchronously follows a blog.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-up-follow
		/// </remarks>
		/// <param name="blogUrl">
		/// The url of the blog to follow.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		/// will carry a <see cref="TumblrException"/>
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogUrl"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogUrl"/> is empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task FollowAsync(string blogUrl)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogUrl == null)
				throw new ArgumentNullException("blogUrl");

			if (blogUrl.Length == 0)
				throw new ArgumentException("Blog url cannot be empty.", "blogUrl");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("FollowAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("url", blogUrl);

			return this.rawClient.CallApiMethodNoResultAsync(
				new UserMethod("follow", this.rawClient.OAuthToken, HttpMethod.Post, parameters),
				CancellationToken.None);
		}

		#endregion

		#region UnfollowAsync

		/// <summary>
		/// Asynchronously unfollows a blog.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-up-unfollow
		/// </remarks>
		/// <param name="blogUrl">
		/// The url of the blog to unfollow.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task fails, <see cref="Task.Exception"/> 
		/// will carry a <see cref="TumblrException"/>
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="blogUrl"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="blogUrl"/> is empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		public Task UnfollowAsync(string blogUrl)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (blogUrl == null)
				throw new ArgumentNullException("blogUrl");

			if (blogUrl.Length == 0)
				throw new ArgumentException("Blog url cannot be empty.", "blogUrl");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("UnfollowAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("url", blogUrl);

			return this.rawClient.CallApiMethodNoResultAsync(
				new UserMethod("unfollow", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
				CancellationToken.None);
		}

		#endregion

		#region GetTaggedPostsAsync

		/// <summary>
		/// Asynchronously retrieves posts that have been tagged with a specific <paramref name="tag"/>.
		/// </summary>
		/// <remarks>
		/// See: http://www.tumblr.com/docs/en/api/v2#m-up-tagged
		/// </remarks>
		/// <param name="tag">
		/// The tag on the posts to retrieve.
		/// </param>
		/// <param name="before">
		/// The timestamp of when to retrieve posts before. 
		/// </param>
		/// <param name="count">
		/// The number of posts to retrieve.
		/// </param>
		/// <param name="filter">
		/// A <see cref="PostFilter"/>.
		/// </param>
		/// <returns>
		/// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
		/// carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
		/// representing the error occurred during the call.
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// The object has been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="tag"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="tag"/> is empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="count"/> is less than 1 or greater than 20.
		/// </exception>
        public Task<IEnumerable<Post>> GetTaggedPostsAsync(string tag, DateTime? before = null, int count = 20, PostFilter filter = PostFilter.Html)
		{
			if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (tag == null)
				throw new ArgumentNullException("tag");

			if (tag.Length == 0)
				throw new ArgumentException("Tag cannot be empty.", "tag");

			if (count < 1 || count > 20)
				throw new ArgumentOutOfRangeException("count", "count must be between 1 and 20.");

			MethodParameterSet parameters = new MethodParameterSet();
			parameters.Add("api_key", this.apiKey);
			parameters.Add("tag", tag);
			parameters.Add("before", before.HasValue ? DateTimeHelper.ToTimestamp(before.Value).ToString() : null, null);
			parameters.Add("limit", count, 0);
			parameters.Add("filter", filter.ToString().ToLowerInvariant(), "html");

            return this.rawClient.CallApiMethodAsync<IEnumerable<Post>>(
				new ApiMethod("https://api.tumblr.com/v2/tagged", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
				CancellationToken.None,
				new JsonConverter[] { new PostArrayConverter() });
		}

		#endregion

		#region GetDashboardPostsAsync

        ///  <summary>
        ///  Asynchronously retrieves posts from the current user's dashboard.
        ///  </summary>
        ///  See:  http://www.tumblr.com/docs/en/api/v2#m-ug-dashboard
        ///  <param name="sinceId">
        ///   Return posts that have appeared after the specified ID. Use this parameter to page through the results: first get a set 
        ///   of posts, and then get posts since the last ID of the previous set.  
        ///  </param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="type">
        ///  The <see cref="PostType"/> to return.
        ///  </param>
        ///  <param name="includeReblogInfo">
        ///  Whether or not the response should include reblog info.
        ///  </param>
        ///  <param name="includeNotesInfo">
        ///  Whether or not the response should include notes info.
        ///  </param>
        ///  <returns>
        ///  A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        ///  representing the error occurred during the call.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="InvalidOperationException">
        ///  This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
        ///  </exception>
        ///  <exception cref="ArgumentOutOfRangeException">
        ///  <list type="bullet">
        ///  <item>
        /// 		<description>
        /// 			<paramref name="sinceId"/> is less than 0.
        /// 		</description>
        /// 	</item>
        ///  </list>
        ///  </exception>
        public Task<IEnumerable<Post>> GetDashboardPostsAsync(long sinceId, Pagination pagination, PostType type = PostType.All, bool includeReblogInfo = false, bool includeNotesInfo = false)
		{
            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (sinceId < 0)
				throw new ArgumentOutOfRangeException("sinceId", "sinceId must be greater or equal to zero.");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetDashboardPostsAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);
			parameters.Add("type", type.ToString().ToLowerInvariant(), "all");
			parameters.Add("since_id", sinceId, 0);
			parameters.Add("reblog_info", includeReblogInfo, false);
			parameters.Add("notes_info", includeNotesInfo, false);

            return this.rawClient.CallApiMethodAsync<PostCollectionResponse, IEnumerable<Post>>(
				new UserMethod("dashboard", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
				r => r.Posts,
				CancellationToken.None);
		}

		#endregion

		#region GetUserLikesAsync

        ///  <summary>
        ///  Asynchronously retrieves the current user's likes.
        ///  </summary>
        ///  <remarks>
        ///  See: http://www.tumblr.com/docs/en/api/v2#m-ug-likes
        ///  </remarks>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///  A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        ///  carry a <see cref="IEnumerable{Post}"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        ///  representing the error occurred during the call.
        ///  </returns>
        ///  <exception cref="ObjectDisposedException">
        ///  The object has been disposed.
        ///  </exception>
        ///  <exception cref="InvalidOperationException">
        ///  This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
        ///  </exception>
        public Task<IEnumerable<Post>> GetUserLikesAsync(Pagination pagination)
		{
            if (pagination == null)
            {
                throw new ArgumentNullException("pagination");
            }

            if (this.rawClient.Disposed)
				throw new ObjectDisposedException("TumblrClient");

			if (this.rawClient.OAuthToken == null)
				throw new InvalidOperationException("GetBlogLikesAsync method requires an OAuth token to be specified.");

			MethodParameterSet parameters = new MethodParameterSet();
            AddPaginationToParameters(pagination, parameters);

            return this.rawClient.CallApiMethodAsync<LikesResponse, IEnumerable<Post>>(
				new UserMethod("likes", this.rawClient.OAuthToken, HttpMethod.Get, parameters),
                response => response.Result,
				CancellationToken.None);
		}

		#endregion

		#endregion

        #endregion

		#region IDisposable Implementation

        public void Dispose()
        {
            this.rawClient.Dispose();
        }

        #endregion

        private static void AddPaginationToParameters(Pagination pagination, MethodParameterSet parameters)
        {
            parameters.Add("offset", pagination.StartIndex, 0);
            parameters.Add("limit", pagination.Count, 20);
        }

    }
}
