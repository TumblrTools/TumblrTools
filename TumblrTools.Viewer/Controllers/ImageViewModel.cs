namespace TumblrTools.Viewer.Controllers
{
    public class ImageViewModel
    {
        public ImageViewModel(string imageUrl, string thumbnailUrl, string postUrl, string blogName)
        {
            this.ImageUrl = imageUrl;
            this.ThumbnailUrl = thumbnailUrl;
            this.PostUrl = postUrl;
            this.BlogName = blogName;
        }

        public string ImageUrl { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string PostUrl { get; private set; }
        public string BlogName { get; private set; }
    }
}