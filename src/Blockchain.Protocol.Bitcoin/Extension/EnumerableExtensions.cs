// <copyright file="EnumerableExtensions.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Extension
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    #endregion

    /// <summary>
    /// This class defines extension methods.
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// This method filters the set based on a supplied predicate and if the set is empty, filters the set from another predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object defined in the set.
        /// </typeparam>
        /// <param name="source">
        /// The source set to filter.
        /// </param>
        /// <param name="eitherPredicate">
        /// The first predicate used to filter the set first.  If no items are returned, the second predicate is used.
        /// </param>
        /// <param name="orPredicate">
        /// The second predicate which is used to filter if the first predicate did not return any items in the filtered set.
        /// </param>
        /// <returns>
        /// A filtered set either from the first predicate or the second predicate.
        /// </returns>
        public static IEnumerable<T> Either<T>(this IEnumerable<T> source, Func<T, bool> eitherPredicate, Func<T, bool> orPredicate)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (eitherPredicate == null)
            {
                throw new ArgumentNullException("eitherPredicate");
            }

            if (orPredicate == null)
            {
                throw new ArgumentNullException("orPredicate");
            }

            // Filter based on first predicate
            var filteredSet = source.Where(eitherPredicate);
            if (filteredSet.Count() == 0)
            {
                // No items, filter on second set
                filteredSet = source.Where(orPredicate);
            }

            // Return filtered set
            return filteredSet;
        }

        /// <summary>
        /// The first or throw.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The first type.
        /// </returns>
        public static T FirstOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<Exception> action) where T : class
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return source.FirstOrAction(predicate, () => { throw action(); });
        }

        /// <summary>
        /// The first or throw.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The first type.
        /// </returns>
        public static T FirstOrThrow<T>(this IEnumerable<T> source, Func<Exception> action) where T : class
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return source.FirstOrAction(() => { throw action(); });
        }

        /// <summary>
        /// The first or throw.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The first type.
        /// </returns>
        public static T FirstOrAction<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T> action) where T : class
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return source.FirstOrDefault(predicate) ?? action();
        }

        /// <summary>
        /// The first or throw.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The first type.
        /// </returns>
        public static T FirstOrAction<T>(this IEnumerable<T> source, Func<T> action) where T : class
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return source.FirstOrDefault() ?? action();
        }

        /// <summary>
        /// Implemented a foreach extension for an enumerable object.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The first type.
        /// </typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Implemented a foreach extension for an enumerable object.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var index = 0;
            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }

        /// <summary>
        /// Split a collection in to batches of collections.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object in the source.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="parts">
        /// The number of parts.
        /// </param>
        /// <returns>
        /// The collection.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended design.")]
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int parts)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            parts = parts == 0 ? 1 : parts;
            var i = 0;
            var splits = from item in source group item by i++ % parts into part select part.AsEnumerable();
            return splits;
        }

        /// <summary>
        /// Check if an enumeration is empty.
        /// </summary>
        /// <typeparam name="T">The enumerated type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>True if empty.</returns>
        public static bool None<T>(this IEnumerable<T> source)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return !source.Any();
        }

        /// <summary>
        /// Check if an enumeration is empty.
        /// </summary>
        /// <typeparam name="T">The enumerated type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>True if empty.</returns>
        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return !source.Any(predicate);
        }

        /// <summary>
        /// Batch a collection in to fixed size batches.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="batchSize">The batch size.</param>
        /// <returns>The batch.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, batchSize - 1);
                }
            }
        }

        /// <summary>
        /// Inner method to batch a collection in to fixed size batches.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="batchSize">The batch size.</param>
        /// <returns>The batch.</returns>
        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }

        /// <summary>
        /// Aggregate with a stop condition.
        /// </summary>
        public static TAccumulate AggregateUntil<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, bool> condition)
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (condition == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            TAccumulate result = seed;
            foreach (TSource element in source)
            {
                result = func(result, element);
                if (condition(result)) break;
            }

            return result;
        }

        /// <summary>
        /// Distinct by akey.
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Remove elements from a queue in a batch.
        /// </summary>
        public static IEnumerable<T> TakeAndRemove<T>(this Queue<T> queue, int count)
        {
            var queuecount = Math.Min(queue.Count, count);
            for (var i = 0; i < queuecount; i++)
            {
                yield return queue.Dequeue();
            }
        }

        #endregion
    }
}