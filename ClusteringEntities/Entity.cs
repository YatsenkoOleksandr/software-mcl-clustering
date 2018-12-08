using System.Collections.Generic;

namespace ClusteringEntities.Core
{
    public class Entity
    {
        public string Name { get; set; }

        public ICollection<EntityRelation> Relations { get; set; }
    }
}
