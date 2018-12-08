using System.Collections.Generic;
using System.Linq;

namespace ClusteringEntities.Core
{
    // Class to describe decomposition outputs
    public class Node : INode
    {
        private List<INode> _childs = new List<INode>();

        public List<INode> Childs => _childs.Where(node => node is Node).ToList();// _childs.Where(node => node is Cluster).ToList()

        public string Name { get; set; }

        public NodeType ImageType { get; }

        // For "Folder" - id of folder, for "Class" -> id of parent folder
        public int ClusterId { get; set; }

        public void Add(INode child)
        {
            _childs.Add(child);
        }

        public void AddRange(IEnumerable<INode> childs)
        {
            _childs.AddRange(childs);
        }

        public void RemoveChildByName(string name)
        {
            int indexToRemove = _childs.FindIndex(child => child.Name == name);
            if (indexToRemove > -1)
            {
                _childs.RemoveAt(indexToRemove);
            }
        }

        public Node(string name, List<INode> methods)
        {
            Name = name;
            _childs = methods;
            ImageType = NodeType.Class;
        }

        public Node(string name, INode left, INode right)
        {
            Name = name;
            ImageType = NodeType.Folder;
            _childs.Add(left);
            _childs.Add(right);
        }

        public Node(string name, int clusterId, NodeType nodeType = NodeType.Folder)
        {
            Name = name;
            ClusterId = clusterId;
            ImageType = nodeType;
        }

        public Node(string name, NodeType nodeType)
        {
            Name = name;
            ImageType = nodeType;
        }
    }
}
