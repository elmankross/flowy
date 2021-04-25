using System.Drawing;

namespace Engine.Designer
{
    public sealed class ActivityPrimitive : IPrimitive
    {
        public Point Location { get; } = new Point(1, 1);
        public Size Size { get; } = new Size(50, 25);
        public string Name { get; }
        public string Description { get; }
        public Anchor Input { get; private set; } = Anchor.Empty;
        public Anchor Output { get; private set; } = Anchor.Empty;

        private ActivityPrimitive(IActivity activity)
        {
            Name = activity.Meta[ActivityMeta.Key.Name];
            Description = activity.Meta[ActivityMeta.Key.Description];
        }

        public static ActivityPrimitive Create(IActivity activity) => new ActivityPrimitive(activity);
        public static ActivityPrimitive Create<TAccept>(IActivity<TAccept> activity)
        {
            var instance = Create((IActivity)activity);
            instance.Input = new Anchor(typeof(TAccept));
            return instance;
        }

        public static ActivityPrimitive Create<TAccept, TReturn>(IActivity<TAccept, TReturn> activity)
        {
            var instance = Create<TAccept>(activity);
            instance.Output = new Anchor(typeof(TReturn));
            return instance;
        }
    }
}
