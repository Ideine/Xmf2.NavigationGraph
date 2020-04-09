using System.Collections.Generic;

namespace Xmf2.NavigationGraph.Core.Extensions
{
	internal static class CollectionExtensions
	{
		// todo à mettre dans une lib d'extensions netstandard ?
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
	}
}