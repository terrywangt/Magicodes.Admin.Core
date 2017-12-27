using System;
using System.Threading.Tasks;

namespace Magicodes.Admin.Core.Threading
{
    public class AsyncRunner
    {
        public static void Run(Task task, Action<Task> onError = null)
        {
            if (onError == null)
            {
                task.ContinueWith((task1, o) => { }, TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                task.ContinueWith(onError, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }
}