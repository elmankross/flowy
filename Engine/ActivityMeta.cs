using System.Collections.Generic;

namespace Engine
{
    public interface IActivityMeta : IReadOnlyDictionary<string, string>
    {
        string this[ActivityMeta.Key key] { get; }
    }

    public sealed class ActivityMeta : Dictionary<string, string>, IActivityMeta
    {
        public enum Key
        {
            Name,
            Description
        }

        public string this[Key key]
        {
            get => this[key.ToString("G")];
            set => this[key.ToString("G")] = value;
        }
    }
}
