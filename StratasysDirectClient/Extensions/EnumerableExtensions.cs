using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratasysDirect
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T item in enumerable)
			{
				action (item);
			}
		}

		public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T, int> action)
		{
			int index = 0;
			foreach (T item in enumerable)
			{
				action (item, index++);
			}
		}
	}
}
