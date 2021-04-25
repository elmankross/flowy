namespace Engine.Designer
{
    public struct Connection
    {
        public Anchor Source { get; }
        public Anchor Target { get; }


        public Connection(Anchor source, Anchor target)
        {
            Source = source;
            Target = target;
        }
    }
}
