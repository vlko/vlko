using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Tools
{
    public static class EnumerationExtensions
    {
#if NET6
        [Obsolete("Use build-in .NET 6 Chunk linq batcher.")]
#endif
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        /// <summary>
        /// Groups a sequence to groups while the predicate for subsequent elements returns true.
        /// 
        /// The predicate takes the elements in the current group, the key currently proccessed and the key of the currently evaluated item.
        /// 
        /// Inspired by https://codereview.stackexchange.com/a/6625
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector">Function to select the grouping key.</param>
        /// <param name="elementSelector">Function to select the elements in the grouping.</param>
        /// <param name="predicate">The group is yielded when this returns false.</param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupWhile<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<IEnumerable<TElement>, TKey, TKey, bool> predicate)
        {
            if (source.Count() == 0)
            {
                yield break;
            }

            TKey currentKey = keySelector(source.First());
            var foundItems = new List<TElement>();
            foreach (var item in source)
            {
                TKey key = keySelector(item);

                if (!predicate(foundItems, currentKey, key))
                {
                    yield return new Grouping<TKey, TElement>(currentKey, foundItems);
                    currentKey = key;
                    foundItems = new List<TElement>();
                }

                foundItems.Add(elementSelector(item));
            }

            if (foundItems.Count > 0)
            {
                yield return new Grouping<TKey, TElement>(currentKey, foundItems);
            }
        }

        public class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
        {
            public TKey Key { get; set; }

            public Grouping(TKey key, IEnumerable<TElement> elements)
            {
                Key = key;
                AddRange(elements);
            }
        }
    }
}
