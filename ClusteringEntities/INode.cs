using System.Collections.Generic;

namespace ClusteringEntities.Core
{
    // Interface to describe decomposition output
    public interface INode
    {
        List<INode> Childs { get; }
        string Name { get; set; }
        NodeType ImageType { get; }
        void Add(INode child);
        void AddRange(IEnumerable<INode> childs);
        void RemoveChildByName(string name);
        int ClusterId { get; set; }
    }
}
