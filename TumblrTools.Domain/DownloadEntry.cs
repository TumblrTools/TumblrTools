namespace TumblrTools.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TumblrTools.Domain.Properties;

    public class DownloadEntry
    {
        private IEnumerable<string> tags;

        public DownloadEntry(string blogId)
        {
            this.Id = Guid.NewGuid().ToString();
            this.BlogId = blogId;
            this.Tags = new HashSet<string>();
            this.LastUpdate = DateTime.Now;
        }

        public string Id { get; private set; }

        public string BlogId { get; private set; }

        public IEnumerable<string> Tags
        {
            get { return this.tags; }
            private set
            {
                this.tags = value;
            }
        }

        public bool Removed { get; [UsedImplicitly] private set; }


        public static string JoinTags(IEnumerable<string> value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return string.Join("|", value.Select(tag => tag.ToLower().Trim()).OrderBy(tag => tag));
        }

        public static IEnumerable<string> SplitTags(string tagList)
        {
            if (string.IsNullOrEmpty(tagList))
            {
                return new string[0];
            }

            return NormalizeTags(tagList.Split('|'));
        }

        public static IEnumerable<string> NormalizeTags(IEnumerable<string> tags)
        {
            if (tags == null)
            {
                return new String[0];
            }

            return tags.Select(tag => tag.ToLower().Trim()).Where(tag => !string.IsNullOrEmpty(tag)).OrderBy(tag => tag);
        }

        public string TagList()
        {
            return JoinTags(this.Tags);
        }

        public void UpdateTags(IEnumerable<string> tags)
        {
            this.Tags = NormalizeTags(tags);
        }

        public DateTime LastUpdate { get; set; }

        // public virtual ICollection<DownloadFile> Files { get; private set; }

        public override bool Equals(object obj)
        {
            DownloadEntry other = obj as DownloadEntry;

            if (other == null)
            {
                return false;
            }

            return this.BlogId == other.BlogId;
        }

        public override int GetHashCode()
        {
            return this.BlogId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.BlogId, this.TagList());
        }
    }
}
