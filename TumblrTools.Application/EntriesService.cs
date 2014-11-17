namespace TumblrTools.Application
{
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    public class EntriesService
    {
        private readonly ILogger logger;
        private readonly IDownloadRepository downloadRepository;


        public EntriesService(
            ILogger logger,
            IDownloadRepository downloadRepository)
        {
            this.logger = logger;
            this.downloadRepository = downloadRepository;
        }

        public void AddOrUpdateEntry(
            string blogId,
            string[] tagList)
        {
            this.logger.Info("Add/update {0} and downloading", blogId);
            DownloadEntry existingEntry = this.downloadRepository.Get(blogId);
            if (existingEntry == null)
            {
                DownloadEntry newEntry = new DownloadEntry(blogId);
                newEntry.UpdateTags(tagList);
                this.downloadRepository.Create(newEntry);
                this.logger.Info("The download entry {0} has been added successfully", newEntry);
            }
            else
            {
                existingEntry.UpdateTags(tagList);
                this.downloadRepository.Update(existingEntry);
                this.logger.Info("The download entry {0} has been updated successfully", existingEntry);
            }
        }

        public void RemoveEntry(string blogId)
        {
            this.logger.Info("Removing {0}", blogId);
            if (this.downloadRepository.Remove(blogId))
            {
                this.logger.Info("The download entry has been removed successfully");
            }
            else
            {
                this.logger.Error("The download entry does not exist");
            }
        }
    }
}