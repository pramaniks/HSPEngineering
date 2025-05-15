using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// BlockingCollection does not have a constructor that accepts a generic IEnumerable&lt;T&gt;.  This is problematic
    /// during certain deserialization routines.  This provides the missing constructor.
    /// </summary>
    public class BlockingCollectionAdapter<T> : BlockingCollection<T>
    {
        public BlockingCollectionAdapter()
        {

        }

        public BlockingCollectionAdapter(int boundedCapacity)
            : base(boundedCapacity)
        {

        }

        /// <summary>
        /// Added constructor for accepting a regular IEnumerable&lt;T&gt;
        /// </summary>
        public BlockingCollectionAdapter(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public BlockingCollectionAdapter(IProducerConsumerCollection<T> collection)
            : base(collection)
        {

        }

        public BlockingCollectionAdapter(IProducerConsumerCollection<T> collection, int boundedCapacity)
            : base(collection, boundedCapacity)
        {

        }
    }
}
