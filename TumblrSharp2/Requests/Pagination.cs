namespace TumblrSharp2.Requests
{
    using System;

    public class Pagination
    {
        public Pagination(long startIndex = 0, int count = 20)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "startIndex must be greater or equal to zero.");
            }

            if (count < 1 || count > 20)
            {
                throw new ArgumentOutOfRangeException("count", "count must be between 1 and 20.");
            }

            this.StartIndex = startIndex;
            this.Count = count;
        }

        public long StartIndex { get; private set; }
        public int Count { get; private set; }
    }
}