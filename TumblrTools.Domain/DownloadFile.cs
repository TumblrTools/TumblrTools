namespace TumblrTools.Domain
{
    using System;
    using TumblrTools.Domain.Properties;

    public class DownloadFile
    {
        [UsedImplicitly]
        private DownloadFile()
        {
        }

        public DownloadFile(DownloadFileId downloadFileId, string localFilePath, string postUrl)
        {
            this.DownloadFileId = downloadFileId;
            this.LocalFilePath = localFilePath;
            this.PostUrl = postUrl;
            this.DownloadDate = DateTime.Now;
        }

        public int Id { get; [UsedImplicitly] private set; }

        public DownloadFileId DownloadFileId { get; private set; }

        public string LocalFilePath { get; private set; }

        public string PostUrl { get; set; }

        public DateTime DownloadDate { get; private set; }

        public override bool Equals(object obj)
        {
            DownloadFile file = (DownloadFile)obj;

            return this.LocalFilePath == file.LocalFilePath;
        }

        public override int GetHashCode()
        {
            return this.LocalFilePath.GetHashCode();
        }

        public override string ToString()
        {
            return this.LocalFilePath;
        }
    }
}