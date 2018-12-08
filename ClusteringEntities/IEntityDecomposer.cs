using System.Collections.Generic;

namespace ClusteringEntities.Core
{
    public interface IEntityDecomposer
    {
        INode Decompose(ICollection<Entity> entities);
    }
}
