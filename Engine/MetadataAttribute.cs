using System;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MetadataAttribute : Attribute
    {
        public string Name { get; set; }
        public string Category { get; set; } = "Common";
        public string Description { get; set; }
    }
}
