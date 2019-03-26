using System;
using System.Collections.Generic;

namespace Xmf2.NavigationGraph.Core.Extensions
{
	public static class CollectionExtensions
	{
		public static List<T> Sublist<T>(this IList<T> source, int start, int count)
		{
			var result = new List<T>(count);
			var end = start + count;
			for (var i = start; i < end; ++i)
			{
				result.Add(source[i]);
			}

			return result;
		}

		public static List<TResult> ConvertAll<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> mapper)
		{
			var result = new List<TResult>(source.Count);
			for (var i = 0; i < source.Count; i++)
			{
				result.Add(mapper(source[i]));
			}

			return result;
		}
	}
}