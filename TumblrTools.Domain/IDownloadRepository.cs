namespace TumblrTools.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IDownloadRepository
    {
        string NewId();
        DownloadEntry Get(string blogId);
        void Create(DownloadEntry entry);
        void Update(DownloadEntry entry);
        IList<DownloadEntry> GetAll();
        IList<DownloadEntry> GetAllSortedByBlogId();
        bool Remove(string blogId);
        void UpdateLastDownloadTime(DownloadEntry entry, DateTime lastDownload);
    }
}