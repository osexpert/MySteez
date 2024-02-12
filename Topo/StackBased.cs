using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topo
{
    /// <summary>
    /// https://stackoverflow.com/a/51235189/2671330
    /// </summary>
    public static class StackBased
    {
        public static IEnumerable<T> TopologicalSequenceDFS<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> deps)
        {
            var yielded = new HashSet<T>();
            var visited = new HashSet<T>();
            var stack = new Stack<(T, IEnumerator<T>)>();

            foreach (T t in source)
            {
                if (visited.Add(t))
                    stack.Push((t, deps(t).GetEnumerator()));

                while (stack.Any())
                {
                    var p = stack.Peek();
                    bool depsPushed = false;
                    while (p.Item2.MoveNext())
                    {
                        var curr = p.Item2.Current;
                        if (visited.Add(curr))
                        {
                            stack.Push((curr, deps(curr).GetEnumerator()));
                            depsPushed = true;
                            break;
                        }
                        else if (!yielded.Contains(curr))
                            throw new Exception("cyclic");
                    }

                    if (!depsPushed)
                    {
                        p = stack.Pop();
                        p.Item2.Dispose();
                        if (!yielded.Add(p.Item1))
                            throw new Exception("bug");
                        yield return p.Item1;
                    }
                }
            }
        }

        public static IEnumerable<T> TopologicalSequenceBFS_v2<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> deps)
        {
            var yielded = new HashSet<T>();
            var visited = new HashSet<T>();
            var stack = new Stack<(T, bool)>();

            foreach (var t in source)
            {
                stack.Push((t, false));

                while (stack.Any())
                {
                    var item = stack.Pop();
                    if (item.Item2)
                    {
                        if (!yielded.Add(item.Item1))
                            throw new Exception("bug");
                        yield return item.Item1;
                    }
                    else
                    {
                        if (visited.Add(item.Item1))
                        {
                            stack.Push((item.Item1, true)); // yield after processing dependencies
                            foreach (var dep in deps(item.Item1))
                                stack.Push((dep, false));
                        }
                        else if (!yielded.Contains(item.Item1))
                            throw new Exception("cyclic");
                    }
                }
            }
        }


        public static IEnumerable<T> TopologicalSequenceBFS_v1<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
        {
            var yielded = new HashSet<T>();
            var visited = new HashSet<T>();
            var stack = new Stack<(T, bool)>(source.Select(s => (s, false))); // bool signals Add to sorted

            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (!item.Item2)
                {
                    if (visited.Add(item.Item1))
                    {
                        stack.Push((item.Item1, true)); // To be added after processing the dependencies
                        foreach (var dep in dependencies(item.Item1))
                            stack.Push((dep, false));
                    }
                    else if (!yielded.Contains(item.Item1))
                        throw new Exception("cyclic");
                }
                else
                {
                    if (!yielded.Add(item.Item1))
                        throw new Exception("bug");
                    yield return item.Item1;
                }
            }
        }
    }
}
