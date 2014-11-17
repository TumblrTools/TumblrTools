namespace TumblrSharp2.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TumblrSharp2.Requests;
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Intermediate;
    using TumblrSharp2.Responses.Posts;

    public interface ITumblrClient : IDisposable
    {
        ITumblrRawClient RawClient { get; }

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
        Task<BlogInfo> GetBlogInfoAsync(string blogName);

        Task<TPost> GetPostAsync<TPost>(
            string blogName,
            long postId) where TPost : Post;

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
        /// carry a <see cref="BlogPostsResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
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
        Task<BlogAndPosts> GetPostsAsync(string blogName, Pagination pagination, PostType type = PostType.All, bool includeReblogInfo = false, bool includeNotesInfo = false, PostFilter filter = PostFilter.Html, string tag = null);

        /// <summary>
        /// Asynchronously retrieves the publicly exposed likes from a blog.
        /// </summary>
        /// <remarks>
        /// See: http://www.tumblr.com/docs/en/api/v2#blog-likes
        /// </remarks>
        /// <param name="blogName">
        /// The name of the blog.
        /// </param>
        /// <param name="startIndex">
        /// The offset at which to start retrieving the likes. Use 0 to start retrieving from the latest like.
        /// </param>
        /// <param name="count">
        /// The number of likes to retrieve. Must be between 1 and 20.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry a <see cref="LikesResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
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
        Task<Paginated<Post>> GetBlogLikesAsync(string blogName, Pagination pagination);

        /// <summary>
        /// Asynchronously retrieves a blog's followers.
        /// </summary>
        /// <remarks>
        /// See: http://www.tumblr.com/docs/en/api/v2#blog-followers
        /// </remarks>
        /// <param name="blogName">
        /// The name of the blog.
        /// </param>
        /// <param name="startIndex">
        /// The offset at which to start retrieving the followers. Use 0 to start retrieving from the latest follower.
        /// </param>
        /// <param name="count">
        /// The number of followers to retrieve. Must be between 1 and 20.
        /// </param>
        /// <returns>
        ///  A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry a <see cref="FollowersCollectionResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        /// A <see cref="FollowersCollectionResponse"/> instance.
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
        Task<Paginated<Follower>> GetFollowersAsync(string blogName, Pagination pagination);

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
        Task<long> CreatePostAsync(string blogName, PostData postData);

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
        Task<long> CreatePostAsync(string blogName, PostData postData, CancellationToken cancellationToken);

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
        Task<long> EditPostAsync(string blogName, long postId, PostData postData);

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
        Task<long> EditPostAsync(string blogName, long postId, PostData postData, CancellationToken cancellationToken);

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
        Task<long> ReblogAsync(string blogName, long postId, string reblogKey, string comment = null);

        /// <summary>
        /// Asynchronously returns posts in the current user's queue.
        /// </summary>
        /// <remarks>
        /// See: http://www.tumblr.com/docs/en/api/v2#blog-queue
        /// </remarks>
        /// <param name="blogName">
        /// The name of the blog for which to retrieve queued posts.
        /// </param>
        /// <param name="startIndex">
        /// The offset at which to start retrieving the posts. Use 0 to start retrieving from the latest post.
        /// </param>
        /// <param name="count">
        /// The number of posts to retrieve. Must be between 1 and 20.
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
        /// <exception cref="InvalidOperationException">
        /// This <see cref="TumblrClient"/> instance does not have an OAuth token specified.
        /// </exception>
        Task<IEnumerable<Post>> GetQueuedPostsAsync(string blogName, Pagination pagination, PostFilter filter = PostFilter.Html);

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
        Task<IEnumerable<Post>> GetDraftPostsAsync(string blogName, long sinceId = 0, PostFilter filter = PostFilter.Html);

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
        Task<IEnumerable<Post>> GetSubmissionPostsAsync(string blogName, long startIndex = 0, PostFilter filter = PostFilter.Html);

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
        Task DeletePostAsync(string blogName, long postId);

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
        Task<User> GetUserInfoAsync();

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
        /// carry a <see cref="FollowingCollectionResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry the <see cref="TumblrException"/>
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
        Task<Paginated<Following>> GetFollowingAsync(Pagination pagination);

        /// <summary>
        /// Asynchronously retrieves the current user's likes.
        /// </summary>
        /// <remarks>
        /// See: http://www.tumblr.com/docs/en/api/v2#m-ug-likes
        /// </remarks>
        /// <param name="startIndex">
        /// The offset at which to start retrieving the likes. Use 0 to start retrieving from the latest like.
        /// </param>
        /// <param name="count">
        /// The number of likes to retrieve. Must be between 1 and 20.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry a <see cref="LikesResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry the <see cref="TumblrException"/>
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
        Task<Paginated<Post>> GetLikesAsync(Pagination pagination);

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
        Task LikeAsync(long postId, string reblogKey);

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
        Task UnlikeAsync(long postId, string reblogKey);

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
        Task FollowAsync(string blogUrl);

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
        Task UnfollowAsync(string blogUrl);

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
        Task<IEnumerable<Post>> GetTaggedPostsAsync(string tag, DateTime? before = null, int count = 20, PostFilter filter = PostFilter.Html);

        /// <summary>
        /// Asynchronously retrieves posts from the current user's dashboard.
        /// </summary>
        /// See:  http://www.tumblr.com/docs/en/api/v2#m-ug-dashboard
        /// <param name="sinceId">
        ///  Return posts that have appeared after the specified ID. Use this parameter to page through the results: first get a set 
        ///  of posts, and then get posts since the last ID of the previous set.  
        /// </param>
        /// <param name="startIndex">
        /// The post number to start at.
        /// </param>
        /// <param name="count">
        /// The number of posts to return.
        /// </param>
        /// <param name="type">
        /// The <see cref="PostType"/> to return.
        /// </param>
        /// <param name="includeReblogInfo">
        /// Whether or not the response should include reblog info.
        /// </param>
        /// <param name="includeNotesInfo">
        /// Whether or not the response should include notes info.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry an array of posts. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        /// representing the error occurred during the call.
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
        ///			<paramref name="sinceId"/> is less than 0.
        ///		</description>
        ///	</item>
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
        Task<IEnumerable<Post>> GetDashboardPostsAsync(long sinceId, Pagination pagination, PostType type = PostType.All, bool includeReblogInfo = false, bool includeNotesInfo = false);

        /// <summary>
        /// Asynchronously retrieves the current user's likes.
        /// </summary>
        /// <remarks>
        /// See: http://www.tumblr.com/docs/en/api/v2#m-ug-likes
        /// </remarks>
        /// <param name="startIndex">
        /// The offset at which to start retrieving the likes. Use 0 to start retrieving from the latest like.
        /// </param>
        /// <param name="count">
        /// The number of likes to retrieve. Must be between 1 and 20.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}"/> that can be used to track the operation. If the task succeeds, the <see cref="Task{T}.Result"/> will
        /// carry a <see cref="LikesResponse"/> instance. Otherwise <see cref="Task.Exception"/> will carry a <see cref="TumblrException"/>
        /// representing the error occurred during the call.
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
        Task<IEnumerable<Post>> GetUserLikesAsync(Pagination pagination);
    }
}