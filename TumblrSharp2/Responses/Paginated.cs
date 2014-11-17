using System.Collections.Generic;

namespace TumblrSharp2.Responses
{
    using System.Collections;

    public class Paginated<TResult> : IEnumerable<TResult>
    {
        private readonly IEnumerable<TResult> store;

        public Paginated(IEnumerable<TResult> store, long totalCount, long offset)
        {
            this.store = store;
            this.TotalCount = totalCount;
            this.Offset = offset;
        }

        public long TotalCount { get; private set; }
        public long Offset { get; private set; }
        public IEnumerator<TResult> GetEnumerator()
        {
            return this.store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.store).GetEnumerator();
        }
    }
}
