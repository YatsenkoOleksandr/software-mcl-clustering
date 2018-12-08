using ClusteringEntities.Core;
using MCLClustering;
using System;
using System.Collections.Generic;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = "F:\\LABS\\MASTER\\src\\test_graph.txt";

            JGrokFileParser parser = new JGrokFileParser(filename);

            ICollection<Entity> entities = parser.Parse();

            MCLEntityClustering mcl = new MCLEntityClustering();

            INode cluster = mcl.Decompose(entities);

            double[,] testMatrix = new double[12, 12]
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

            

            MCLClustering.MCLClustering clustering = new MCLClustering.MCLClustering();

            double[,] result = clustering.Clusterize(testMatrix);

            for(int i = 0; i < result.GetUpperBound(0) + 1; i++)
            {
                for(int j = 0; j < result.GetUpperBound(1) + 1; j++)
                {
                    Console.Write("{0}\t", result[i, j]);
                }
                Console.WriteLine();
            }

        }
    }
}
