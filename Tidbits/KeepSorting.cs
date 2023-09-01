using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MySteez
{
	public static class KeepSorting
	{
		/// <summary>
		/// mutator(T current, T prev)
		/// Only current can be mutatet, prev is only for reading\reference.
		/// You can only mutate so that current will sort ofter or equal to the prev (if any), else will throw.
		/// When you mutate and current need to skip some later elements to become sorted, the same element will go into the mutator callback later.
		/// Use case: I am not sure if there are any :-) I think you can acomplish much the same with a regular CompareTo-method,
		/// but there may be very special cases where you need to decide the order based on the order of previous element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="mutator"></param>
		public static void MutateAndKeepSorting<T>(IList<T> list, Func<T, T, bool> mutator) where T : class, IComparable<T>
		{
			T prev = null;
			//bool prevIsSet = false; // generics and null...

			for (int i = 0; i < list.Count; i++)
			{
			start:;

				var curr = list[i];
				if (prev != null && curr.CompareTo(prev) < 0)
					throw new Exception("Not sorted");

				if (mutator(curr, prev))
				{
					// curr was changed
					if (prev != null)//prevIsSet)
					{
						var cmp = curr.CompareTo(prev);
						if (cmp < 0)
						{
							throw new Exception("Can not change order backwards");
						}
					}

					bool gotIt = false;
					for (int j = i + 1; j < list.Count; j++)
					{
						// find the element larger than curr. curr should be before that
						var ahead = list[j];
						var c = ahead.CompareTo(curr);
						if (c >= 0)
						{
							// curr larger than ahead
							// move curr before ahead
							if (i != j - 1)
							{
								list.RemoveAt(i); // the next element now take the place of i. We will miss this in the next iteration?
								list.Insert(j - 1, curr);
								goto start; // We will not miss it, since we goto start and keep i
							}

							// if we got here, we did not need to move it

							gotIt = true;
							break;
						}
					}

					if (!gotIt)
					{
						// no elements after or we did not find anything larger than curr?
						// in any case, we go last
						if (i != list.Count - 1)
						{
							list.RemoveAt(i);
							list.Add(curr);
							goto start; // We will not miss it, since we goto start and keep i
						}
					}
				}

				prev = curr;
//				prevIsSet = true;
			}
		}

	}
}
