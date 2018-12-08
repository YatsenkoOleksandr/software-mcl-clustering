namespace ClusteringEntities.Core
{
    public interface IGraphClustering
    {
        double[,] Clusterize(double[,] matrix);
    }
}
