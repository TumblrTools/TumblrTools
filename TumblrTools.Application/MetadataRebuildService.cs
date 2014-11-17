namespace TumblrTools.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using TumblrSharp2;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    public class MetadataRebuildService
    {
        private readonly IDownloadRepository downloadRepo;
        private readonly ILogger logger;
        private readonly IPostRepository postRepo;

        public MetadataRebuildService(ILogger logger, IPostRepository postRepo, IDownloadRepository downloadRepo)
        {
            this.logger = logger;
            this.postRepo = postRepo;
            this.downloadRepo = downloadRepo;
        }

        public void RebuildDraftStatus()
        {
            foreach (PhotoPost post in this.postRepo.GetAll())
            {
                this.ProcessPost(post);
            }
        }

        public void RebuildDraftStatus(string blogId)
        {
            foreach (PhotoPost post in this.postRepo.GetFromBlog(blogId))
            {
                this.ProcessPost(post);
            }
        }

        private void ProcessPost(PhotoPost post)
        {
            this.logger.Debug("Rebuilding state for post {0}", post.Id);
            List<string> tags = this.downloadRepo.Get(post.BlogName).Tags.ToList();

            PostCreationState newState;
            if (this.PostMatchesTags(post, tags))
            {
                newState = PostCreationState.Published;
            }
            else
            {
                newState = PostCreationState.Draft;
            }

            PostCreationState oldState = post.State;

            if (newState != oldState)
            {
                post.State = newState;
                this.postRepo.Update(post);
                this.logger.Info("Post {0} changed draft from {1} to {2}", post.Id, oldState, post.State);
            }
        }

        private bool PostMatchesTags(Post post, IList<string> tags)
        {
            if (!tags.Any() || tags.Any(post.Tags.Contains))
            {
                return true;
            }

            if (post.Tags.Any())
            {
                this.logger.Trace("No tag match (post tags: {0})", string.Join(", ", post.Tags));
            }
            else
            {
                this.logger.Trace("No tag match (no tags)");
            }

            return false;
        }
    }
}