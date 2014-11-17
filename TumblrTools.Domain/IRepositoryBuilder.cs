namespace TumblrTools.Domain
{
    public interface IRepositoryBuilder
    {
        IDownloadRepository DownloadRepository { get; }
        IPostRepository PostRepository { get; }
        void Save();
    }
}