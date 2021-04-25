using System;
using System.Collections.Generic;

namespace Engine.Designer
{
    public sealed class ConnectionsCollection : HashSet<Connection>
    {
        private readonly HashSet<Guid> _usedSources;
        private readonly HashSet<Guid> _usedTargets;

        public ConnectionsCollection()
        {
            _usedSources = new HashSet<Guid>();
            _usedTargets = new HashSet<Guid>();
        }

        public ConnectionsCollection(ICollection<Connection> connections)
        {
            foreach(var connection in connections)
            {
                Add(connection.Source, connection.Target);
            }
        }

        public void Add(Anchor source, Anchor target)
        {
            if (_usedSources.Contains(source.Id))
            {
                throw new Exception("This source already connected.");
            }

            if (_usedTargets.Contains(target.Id))
            {
                throw new Exception("This target already connected.");
            }

            _usedTargets.Add(source.Id);
            Add(new Connection(source, target));
        }

        new private void Add(Connection connection)
        {
            base.Add(connection);
        }
    }
}
