using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.Designer
{
    public sealed class FlowScene
    {
        public HashSet<ActivityPrimitive> Activities { get; private set; }
        public ConnectionsCollection Connections { get; private set; }

        private FlowScene()
        {
            Activities = new HashSet<ActivityPrimitive>();
            Connections = new ConnectionsCollection();
        }

        public static FlowScene Build<TActivity>(TActivity activity)
            where TActivity : IActivity
        {
            var flow = new FlowScene();
            var activities = BuildActivity(activity);

            for (var i = 1; i < activities.Count; i += 2)
            {
                flow.Connections.Add(activities[i - 1].Output, activities[i].Input);
            }

            flow.Activities = activities.ToHashSet();
            return flow;
        }


        private static List<ActivityPrimitive> BuildActivity(IActivity activity)
        {
            if (activity == null)
            {
                return new List<ActivityPrimitive>(0);
            }

            var generics = activity.GetType().GenericTypeArguments;
            if (activity is ActivityGroup)
            {
                switch (generics.Length)
                {
                    case 0:
                        var cwti = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup.CurrentWithoutInput)));
                        var nwti = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup.NextWithoutInput)));

                        cwti.AddRange(nwti);

                        return cwti;

                    case 1:
                        var cwto = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup<object>.CurrentWithoutOutput)));
                        var cwo = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup<object>.CurrentWithOutput)));
                        var nwi = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup<object>.NextWithInput)));

                        cwto.AddRange(cwo);
                        cwto.AddRange(nwi);

                        return cwto;

                    case 3:
                        var cwo2 = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup<object, object, object>.CurrentWithOutput)));
                        var nwo = BuildActivity(GetGroupProperty(activity, nameof(ActivityGroup<object, object, object>.NextWithOutput)));

                        cwo2.AddRange(nwo);

                        return cwo2;
                }
            }

            return new List<ActivityPrimitive> { CreatePrimitive(activity, generics) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IActivity GetGroupProperty(IActivity group, string name)
            => group.GetType()
                    .GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .GetValue(group) as IActivity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="generics"></param>
        /// <returns></returns>
        private static ActivityPrimitive CreatePrimitive(IActivity activity, params Type[] generics)
        {
            var method = typeof(ActivityPrimitive)
                          .GetMethods()
                          .Where(x => x.Name == nameof(ActivityPrimitive.Create)
                              && x.GetGenericArguments().Length == generics.Length)
                          .Single();

            if (method.IsGenericMethod)
            {
                method = method.MakeGenericMethod(generics);
            }

            var primitive = method.Invoke(null, new object[] { activity });
            return primitive as ActivityPrimitive;
        }
    }
}
