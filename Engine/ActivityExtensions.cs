using System;

namespace Engine
{
    internal static class ActivityExtensions
    {
        internal static TActivity SetupActivity<TActivity>(this Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var activity = new TActivity();
            builder(activity);
            return activity;
        }
    }
}
