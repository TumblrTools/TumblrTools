namespace TumblrTools.Application
{
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Posts;

    public interface IPhotoLocalFileNamer
    {
        string GetLocalFileName(int photoSetIndex, PhotoInfo photoInfo, Photo photo, PhotoPost photoPost, BlogInfo blogInfo);
    }
}