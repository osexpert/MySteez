using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topo
{
    /// <summary>
    /// https://github.com/servicetitan/Stl.Fusion/blob/04c4aecc59942ea544494b7874bb525de09d0d13/src/Stl/Collections/EnumerableExt.cs#L39
    /// </summary>
    public static class Stl_Fusion
    {
        public static IEnumerable<T> OrderByDependency<T>(
    this IEnumerable<T> source,
    Func<T, IEnumerable<T>> dependencySelector)
        {
            var processing = new HashSet<T>();
            var processed = new HashSet<T>();
            var stack = new Stack<T>(source);
            while (stack.TryPop(out var item))
            {
                if (processed.Contains(item))
                    continue;
                if (processing.Remove(item))
                {
                    processed.Add(item);
                    yield return item;
                    continue;
                }
                processing.Add(item);
                stack.Push(item); // Pushing item in advance assuming there are dependencies
                var stackSize = stack.Count;
                foreach (var dependency in dependencySelector(item))
                    if (!processed.Contains(dependency))
                    {
                        if (processing.Contains(dependency))
                            throw new Exception("Circular dependency");// Errors.CircularDependency(item);
                        stack.Push(dependency);
                    }
                if (stackSize == stack.Count)
                { // No unprocessed dependencies
                    stack.Pop(); // Popping item pushed in advance
                    processing.Remove(item);
                    processed.Add(item);
                    yield return item;
                }
            }
        }
    }
}
