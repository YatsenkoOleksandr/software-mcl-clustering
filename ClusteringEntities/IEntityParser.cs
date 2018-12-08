using System.Collections.Generic;

namespace ClusteringEntities.Core
{
    public interface IEntityParser
    {
        ICollection<Entity> Parse();
    }
}
