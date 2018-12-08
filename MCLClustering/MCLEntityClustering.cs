using ClusteringEntities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLClustering
{
    public class MCLEntityClustering: IEntityDecomposer
    {
        private readonly MCLClustering mclClusteringObject;

        public MCLEntityClustering()
        {
            this.mclClusteringObject = new MCLClustering();
        }

        public INode Decompose(ICollection<Entity> entities)
        {
            double[,] undirectedMatrix = this.EntitiesToGraphMatrix(entities);

            /*
             
            {
                { 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0 }
            };
             
             */

            double[,] mclResult = this.mclClusteringObject.Clusterize(undirectedMatrix);

            INode cluster = this.MCLResultToEntities(entities, mclResult);

            return cluster;
        }

        private double[,] EntitiesToGraphMatrix(ICollection<Entity> entities)
        {
            int length = entities.Count;
            double[,] undirectedGraphMatrix = new double[length, length];

            for(int i = 0; i < length; i++)
            {
                Entity current = entities.ElementAt(i);

                foreach(var relation in current.Relations)
                {
                    string dependenceEntityName = relation.DependenceEntity;

                    int dependenceEntityIndex = -1;
                    for(int z = 0; z < entities.Count; z++)
                    {
                        if (entities.ElementAt(z).Name == dependenceEntityName)
                        {
                            dependenceEntityIndex = z;
                            break;
                        }
                    }

                    if (dependenceEntityIndex > -1)
                    {
                        undirectedGraphMatrix[i, dependenceEntityIndex] = 1;
                        undirectedGraphMatrix[dependenceEntityIndex, i] = 1;
                    }
                }
            }

            return undirectedGraphMatrix;
        }

        private INode MCLResultToEntities(
            ICollection<Entity> originalEntities,
            double[,] clusteringResult)
        {
            // MCL: Interpret result

            // Key: cluster core Entity, Values: Entities in that cluster
            Dictionary<string, List<string>> clusters = new Dictionary<string, List<string>>();

            int rows = clusteringResult.GetUpperBound(0) + 1;
            int cols = clusteringResult.GetUpperBound(1) + 1;

            for(int j = 0; j < cols; j++)
            {
                double maxValue = 0;
                int rowIndex = 0;

                for (int i = 0; i < rows; i++)
                {
                    double cellValue = clusteringResult[i, j];

                    if (cellValue == 0)
                    {
                        continue;
                    }

                    // Column has only one non-zero value - 1
                    if (cellValue == 1)
                    {
                        maxValue = 1;
                        rowIndex = i;
                        break;
                    }

                    // There are value between 0 and 1
                    // We need to find what cluster core is more attracted

                    if (cellValue > maxValue)
                    {
                        rowIndex = i;
                        maxValue = cellValue;
                    }
                }
                Entity clusterCoreEntity = originalEntities.ElementAt(rowIndex);
                Entity attachedEntity = originalEntities.ElementAt(j);

                string clusterCoreName = clusterCoreEntity.Name;
                string attachedEntityName = attachedEntity.Name;

                if (!clusters.ContainsKey(clusterCoreName))
                {
                    clusters.Add(clusterCoreName, new List<string>());
                }

                clusters[clusterCoreName].Add(attachedEntityName);
            }

            
            int clusterId = 0;

            Node clusterResult = new Node("Decomposed Software", clusterId++, NodeType.Folder);
            foreach(var clusterCoreEntityName in clusters.Keys)
            {
                Node folder = new Node("Subsystem #" + clusterId, clusterId, NodeType.Folder);

                foreach(var entityNameInCluster in clusters[clusterCoreEntityName])
                {
                    Node classInFolder = new Node(entityNameInCluster, clusterId, NodeType.Class);
                    folder.Add(classInFolder);
                }
                clusterResult.Add(folder);

                clusterId++;
            }

            return clusterResult;                        
        }
    }
}
