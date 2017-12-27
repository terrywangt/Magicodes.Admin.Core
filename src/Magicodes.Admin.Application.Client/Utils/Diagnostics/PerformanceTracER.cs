using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Utils.Diagnostics
{
    public static class PerformanceTracer2
    {
        private static readonly List<TraceItem> TraceItems = new List<TraceItem>();

        public static void Execute(string name, Action action)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                action();
            }
            finally
            {
                sw.Stop();
                AddItem(name, sw);
            }
        }

        public static async Task ExecuteAsync(string name, Func<Task> action)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await action();
            }
            finally
            {
                sw.Stop();
                AddItem(name, sw);
            }
        }

        public static async Task<T> ExecuteAsync<T>(string name, Func<Task<T>> action)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                return await action();
            }
            finally
            {
                sw.Stop();
                AddItem(name, sw);
            }
        }

        private static void AddItem(string name, Stopwatch sw)
        {
            var traceItem = new TraceItem(name, sw.Elapsed.TotalMilliseconds);
            TraceItems.Add(traceItem);
            Debug.WriteLine("*** PERFORMANCE TRACER *** " + traceItem);
        }

        public class TraceItem
        {
            public string Name { get; }

            public double Duration { get; }

            public DateTime Time { get; }

            public TraceItem(string name, double duration)
            {
                Name = name;
                Duration = duration;
                Time = DateTime.Now;
            }

            public override string ToString()
            {
                return $"- {Name} -> {Duration:0} ms.";
            }
        }


        public static string Report()
        {
            if (!TraceItems.Any())
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var performanceItem in TraceItems)
            {
                sb.AppendLine(performanceItem.ToString());
            }

            return sb.ToString();
        }

        public static void Initialize()
        {
            TraceItems.Clear();
        }
    }
}