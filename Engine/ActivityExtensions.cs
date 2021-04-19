using System;

namespace Engine
{
    internal static class ActivityExtensions
    {
        internal static TActivity Create<TActivity>(this Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var instance = new TActivity();
            builder?.Invoke(instance);
            return instance;
        }
    }
}
