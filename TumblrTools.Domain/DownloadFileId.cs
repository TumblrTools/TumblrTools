namespace TumblrTools.Domain
{
    using System;
    using System.Linq.Expressions;
    using TumblrTools.Domain.Properties;

    public class DownloadFileId
    {
        [UsedImplicitly]
        private DownloadFileId()
        {
        }

        public DownloadFileId(string blogId, string postId) : this(blogId, postId, 0)
        {
        }

        public DownloadFileId(string blogId, string postId, int photosetIndex)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException("blogId");
            }

            if (postId == null)
            {
                throw new ArgumentNullException("postId");
            }

            if (photosetIndex < 0)
            {
                throw new ArgumentOutOfRangeException("photosetIndex");
            }

            this.BlogId = blogId;
            this.PostId = postId;
            this.PhotosetIndex = photosetIndex;
        }

        public string BlogId { get; private set; }

        public string PostId { get; private set; }

        public int PhotosetIndex { get; private set; }

        public override bool Equals(object obj)
        {
            DownloadFileId other = (DownloadFileId)obj;

            return
                other.BlogId == this.BlogId &&
                other.PostId == this.PostId &&
                other.PhotosetIndex == this.PhotosetIndex;
        }

        public Expression<Func<DownloadFile, bool>> ExpressionEquals()
        {
            return first =>
                first.DownloadFileId.BlogId == this.BlogId &&
                first.DownloadFileId.PostId == this.PostId &&
                first.DownloadFileId.PhotosetIndex == this.PhotosetIndex;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.BlogId, this.PostId, this.PhotosetIndex).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("File[{0},{1},{2}]", this.BlogId, this.PostId, this.PhotosetIndex);
        }
    }
}