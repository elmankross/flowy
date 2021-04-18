using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Engine
{
    internal static class ActivityExtensions
    {
        internal static Task EmptyCompleteTask() => Task.CompletedTask;
        internal static Task CompleteTask<TSource>(TSource _) => EmptyCompleteTask();

        internal static TActivity CreateActivity<TActivity>(this Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var activity = new TActivity();
            builder?.Invoke(activity);
            return activity;
        }

        internal static void Fork<TCurrent, TNext>(TCurrent current, TNext next,
            Action bothExists = null,
            Action currentExists = null,
            Action nextExists = null,
            Action bothEmpty = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
            where TCurrent : IActivity
            where TNext : IActivity
        {
            var category = "@[" + lineNumber + "] " + memberName;
            if (current != null && next != null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is presented", category);
                bothExists?.Invoke();
            }
            else if (current != null && next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", category);
                currentExists?.Invoke();
            }
            else if (current == null && next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", category);
                nextExists?.Invoke();
            }
            else
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", category);
                bothEmpty?.Invoke();
            }
        }
    }
}
