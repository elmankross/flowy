using System;
using System.Drawing;

namespace Engine.Designer
{
    public class Anchor : IPrimitive
    {
        public Guid Id { get; private set; }
        public string Type { get; }
        public Point Location { get; }
        public Size Size { get; }

        public static Anchor Empty { get; } = new Anchor(typeof(It.Nothing)) { Id = Guid.Empty };

        public Anchor(Type type)
        {
            Id = Guid.NewGuid();
            Type = type.Name;
        }
    }
}
