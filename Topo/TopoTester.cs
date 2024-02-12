using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topo;

namespace ConsoleApplication1
{
    static class topotester
    {


        public static void test()
        {
            List<string> items = new List<string>();
            //items.Add("A");
            //items.Add("B");
            //items.Add("C");
            //items.Add("D");
            //items.Add("E");
            //items.Add("F");
            //items.Add("G");
            //items.Add("H");
            //items.Add("I");
            //items.Add("E");
            //items.Add("D");
            //items.Add("A");
            //items.Add("G");
            //items.Add("F");
            //items.Add("H");
            //items.Add("B");
            //items.Add("I");
            //items.Add("C");

            items.Add("F");
            items.Add("H");
            items.Add("D");
            items.Add("G");
            items.Add("I");
            items.Add("B");
            items.Add("E");
            items.Add("C");
            items.Add("A");

            Dictionary<string, List<string>> deps = new Dictionary<string, List<string>>();
            deps.Add("I", new List<string> { "H", "G", "B", "F" });
            deps.Add("H", new List<string> { "G", "A" });
            deps.Add("G", new List<string> { "E", "A" });
            deps.Add("E", new List<string> { "D", "C", "A" });
            deps.Add("C", new List<string> { "A" });
            deps.Add("B", new List<string> { "A" });

            deps.Add("A", new List<string> { "F" });
            deps.Add("F", new List<string> { "D" });

            //Dictionary<string, List<string>> depsClone = Clone(deps);

            // new simpler
            items.Clear();
            deps.Clear();
            items.Add("0");
            items.Add("1");
            items.Add("2");
            items.Add("3");
            items.Add("4");
            items.Add("5");
            items.Add("6");
            deps.Add("0", new List<string> { "1", "2" });
            deps.Add("1", new List<string> { "3", "4" });
            deps.Add("2", new List<string> { "5", "6" });



            //	deps.Add("D", new List<string> { "I" });

            //			Shuffle(items);

            //var x = TopologicalSort2(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);

            //foreach (var v in x)
            //	Console.WriteLine(v);
            //			int diffs = 0;
            //			int loop = 0;
            while (true)
            {
                Shuffle(items);
                var x34 = TopoSortRecursive.TopologicalSort(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);
                var x54 = StackBased.TopologicalSequenceDFS(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]).ToList();

                //                var bfs = StackBased.TopologicalSequenceBFS_v1(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]).ToList();
                //                var bfs2 = StackBased.TopologicalSequenceBFS_v2(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]).ToList();

                //              if (!Enumerable.SequenceEqual(bfs, bfs2))
                //                    throw new Exception();

                //			continue;

                //            Dictionary<string, List<string>> deps2 = new Dictionary<string, List<string>>();
                //deps2.Add("I", new List<string> { "H", "G", "B", "F" });
                //deps2.Add("H", new List<string> { "G", "A" });
                //deps2.Add("G", new List<string> { "E", "A" });
                //deps2.Add("E", new List<string> { "D", "C", "A" });
                //deps2.Add("C", new List<string> { "A" });
                //deps2.Add("B", new List<string> { "A" });

                //deps2.Add("A", new List<string> { "F" });
                //deps2.Add("F", new List<string> { "D" });

                var deps2 = Clone(deps);

                foreach (var g in x54)
                {
                    List<string> l;
                    if (deps2.TryGetValue(g, out l))
                    {
                        if (l.Count > 0)
                            throw new Exception("got dep");
                    }
                    else
                    {
                        // no deps. ok
                    }

                    // remove from all deps
                    deps2.ToList().ForEach(e => e.Value.Remove(g));
                }

                if (!Enumerable.SequenceEqual(x34, x54))
                    throw new Exception();




                //loop++;
            }
            //Console.WriteLine();
            var t = DateTime.Now.Ticks;
            IEnumerable<string> x2 = null;
            //for (int i = 0; i < 1000000000; i++)
            //x2 = TopologicalSequenceDFS_struct(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);

            Console.WriteLine("bfs: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);
            foreach (var v in x2)
                Console.WriteLine(v);

            return;

            Console.WriteLine();


            t = DateTime.Now.Ticks;
            for (int i = 0; i < 1000000; i++)
                x2 = StackBased.TopologicalSequenceBFS_v2(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);

            Console.WriteLine("bfs: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);
            foreach (var v in x2)
                Console.WriteLine(v);

            Console.WriteLine();



            t = DateTime.Now.Ticks;
            IEnumerable<string> x3 = null;
            for (int i = 0; i < 1000000; i++)
                x3 = TopoSortRecursive. TopologicalSort(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);
            Console.WriteLine("org: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);

            foreach (var v in x3)
                Console.WriteLine(v);

            t = DateTime.Now.Ticks;
            IEnumerable<string> x4 = null;
            for (int i = 0; i < 1000000; i++)
                x4 = StackBased. TopologicalSequenceDFS(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);
            Console.WriteLine("dfs: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);

            foreach (var v in x3)
                Console.WriteLine(v);

            Console.WriteLine();

            t = DateTime.Now.Ticks;
            IEnumerable<string> x5 = null;
            //for (int i = 0; i < 1000000; i++)
            //	x5 = TopologicalSequenceBFS_struct(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);

            Console.WriteLine("BFS_Struct: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);
            foreach (var v in x5)
                Console.WriteLine(v);

            Console.WriteLine();

            t = DateTime.Now.Ticks;
            IEnumerable<string> x6 = null;
            //for (int i = 0; i < 1000000; i++)
            //x6 = TopologicalSequenceDFS_struct(items, f => !deps.ContainsKey(f) ? Enumerable.Empty<string>() : deps[f]);

            Console.WriteLine("DFS_Struct: " + new TimeSpan(DateTime.Now.Ticks - t).TotalSeconds);
            foreach (var v in x6)
                Console.WriteLine(v);

        }

        private static Dictionary<string, List<string>> Clone(Dictionary<string, List<string>> deps)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();

            foreach (var kv in deps)
            {
                res.Add(kv.Key, kv.Value.ToList());
            }

            return res;
        }

        //http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
