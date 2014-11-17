namespace TumblrTools.Generic
{
    public interface IConfiguration
    {
        string ConnectionString { get; }
        string TableName { get; }
        string ConsumerKey { get;  }
        string ConsumerSecret { get; }
        string PhotosDirectory { get; }
    }
}