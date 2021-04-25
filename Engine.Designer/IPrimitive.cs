using System.Drawing;

namespace Engine.Designer
{
    public interface IPrimitive
    {
        Point Location { get; }
        Size Size { get; }
    }
}
