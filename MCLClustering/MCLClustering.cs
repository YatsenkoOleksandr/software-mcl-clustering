using ClusteringEntities.Core;
using System;

namespace MCLClustering
{
    public class MCLClustering : IGraphClustering
    {
        private readonly int expansionParameter;

        private readonly int inflationParameter;

        private readonly int accuracyDigit;

        private readonly double accuracyDelta;

        public MCLClustering(int expansionParameter = 2, int inflationParameter = 2, int accuracyDigit = 4)
        {
            this.expansionParameter = expansionParameter;
            this.inflationParameter = inflationParameter;
            this.accuracyDigit = accuracyDigit;
            this.accuracyDelta = Math.Pow(10, -this.accuracyDigit); 
        }

        public double[,] Clusterize(double[,] matrix)
        {
            bool matrixesAreEqual = false;

            // MCL: Add self loops
            double[,] result = this.AddSelfLoops(matrix);

            // MCL: Normalize columns
            result = this.NormalizeColumns(result);

            do
            {
                // MCL: Expansion Operator
                double[,] expansionResult = Expand(result, this.expansionParameter);

                // MCL: Inflation Operator
                result = Inflate(expansionResult, this.inflationParameter);

                // MCL: Diff
                matrixesAreEqual = this.MatrixesAreEqual(result, expansionResult);

            } while (!matrixesAreEqual);

            return result;
        }

        private double[,] CopyMatrix(double[,] matrix)
        {
            double[,] copy = new double[matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1];
            Array.Copy(matrix, copy, matrix.Length);
            return copy;
        }

        private bool MatrixesAreEqual(double[,] matrix1, double[,] matrix2)
        {
            int rows = matrix1.GetUpperBound(0) + 1;
            int cols = matrix1.GetUpperBound(1) + 1;

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    double delta = Math.Abs(matrix1[i, j] - matrix2[i, j]);
                    if (delta > this.accuracyDelta)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private double[,] AddSelfLoops(double[,] matrix)
        {
            double[,] matrixWithSelfLoops = this.CopyMatrix(matrix);

            for(int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
            {
                matrixWithSelfLoops[i, i] = 1;
            }

            return matrixWithSelfLoops;
        }

        private double[,] NormalizeColumns(double[,] matrix)
        {
            int rows = matrix.GetUpperBound(0) + 1;
            int cols = matrix.GetUpperBound(1) + 1;

            double[,] result = new double[rows, cols];

            for (int j = 0; j < cols; j++)                
            {
                double sum = 0;
                double tempSum = 0;
                for (int i = 0; i < rows; i++)
                {
                    sum += matrix[i, j];
                }

                for(int i = 0; i < rows; i++)
                {
                    result[i, j] = Math.Round(matrix[i, j] / sum, this.accuracyDigit);
                    tempSum += result[i, j];
                }
                //result[rows - 1, j] = Math.Round(1 - tempSum, this.accuracyDigit);
            }

            return result;
        }

        private double[,] Expand(double[,] matrix, int expansionParameter)
        {
            double[,] expansionResult = this.CopyMatrix(matrix);
            
            for(int i = 1; i < expansionParameter; i++)
            {
                // M = M ^ expansionParameter
                expansionResult = MultiplyMatrixes(expansionResult, matrix);
            }

            return expansionResult;
        }

        private double[,] MultiplyMatrixes(double[,] a, double[,] b)
        {
            // Need additional checks that matrixes can be multiplied
            int rows = a.GetUpperBound(0) + 1;
            int cols = a.GetUpperBound(1) + 1;
            int length = (rows > cols) ? cols : rows;
            double[,] result = new double[rows, cols];            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                    result[i, j] = Math.Round(result[i, j], this.accuracyDigit);
                }
            }
            return result;
        }

        private double[,] Inflate(double[,] matrix, int inflationParameter)
        {
            int rows = matrix.GetUpperBound(0) + 1;
            int cols = matrix.GetUpperBound(1) + 1;

            double[,] result = new double[rows, cols];

            for (int j = 0; j < cols; j++)                
            {
                // j-column: (M(i,j)) ^ inflationParameter
                double[] tempPows = new double[rows];
                double sum = 0;

                for (int i = 0; i < rows; i++)
                {
                    tempPows[i] = Math.Pow(matrix[i, j], inflationParameter);
                    sum += tempPows[i];
                }
                
                for (int i = 0; i < rows; i++)
                {
                    result[i, j] = Math.Round(tempPows[i] / sum, this.accuracyDigit);
                }
                //result[i, cols - 1] = Math.Round(1 - tempSum);
            }
            
            return result;
        }
    }
}
