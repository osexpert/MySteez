using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topo
{
    /// <summary>
    /// https://www.codeproject.com/Messages/5371192/Non-recursive-version
    /// PS: This may not be TOPO, only recursive...
    /// </summary>
    public static class CodeProjectNon_recursive_version
    {
        private static IEnumerable<T> SelectRecursiveIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren, bool includeTopLevel)
        {
            var stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(source.GetEnumerator());
                while (stack.Count != 0)
                {
                    var iter = stack.Peek();
                    if (iter.MoveNext())
                    {
                        T item = iter.Current;

                        if (includeTopLevel || stack.Count != 1)
                        {
                            yield return item;
                        }

                        var children = getChildren(item);
                        if (children != null)
                        {
                            stack.Push(children.GetEnumerator());
                        }
                    }
                    else
                    {
                        iter.Dispose();
                        stack.Pop();
                    }
                }
            }
            finally
            {
                while (stack.Count != 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }

        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));
            return SelectRecursiveIterator(source, getChildren, false);
        }

        public static IEnumerable<T> SelectManyAllInclusive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (getChildren == null) throw new ArgumentNullException(nameof(getChildren));
            return SelectRecursiveIterator(source, getChildren, true);
        }
    }
}
