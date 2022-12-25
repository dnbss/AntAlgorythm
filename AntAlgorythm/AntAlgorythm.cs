using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntAlgorythm
{
    public class AntAlgorythm
    {
        public double Alpha { get; set; }
        public double Beta { get; set; }

        private List<Ant> _ants;
        public AntAlgorythm(int alpha = 1, int beta = 1)
        {
            Alpha = alpha;

            Beta = beta;

            _ants = new List<Ant>();
        }

        public void ResetPheromones((double Weight, double Pheromone)[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    matrix[i, j].Pheromone = 1;
                }
            }
        }
        private void InitializeAnts(int countAnts, int countVertex)
        {
            _ants.Clear();

            Random random = new Random();

            for (int i = 0; i < countAnts; i++)
            {
                _ants.Add(new Ant(random.Next(countVertex)));
            }
        }

        public double SumWeightShortestCycle((double Weight, double Pheromone)[,] matrix)
        {
            var path = GetShortestHamiltonCycle(matrix);

            double result = 0;

            for (int i = 0; i < path.Count; i++)
            {

                result += matrix[path[i], path[(i + 1) % path.Count]].Weight;
            }

            return result;
        }
        public List<int> GetShortestHamiltonCycle((double Weight, double Pheromone)[,] matrix)
        {
            List<int> path = new();

            path.Add(0);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                int lastVertex = path.Last();

                int maxPheromoneIndex = lastVertex;

                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (!path.Contains(j) 
                        && matrix[lastVertex, maxPheromoneIndex].Pheromone < matrix[lastVertex, j].Pheromone)
                    {
                        maxPheromoneIndex = j;
                    }
                }

                if (lastVertex != maxPheromoneIndex)
                {
                    path.Add(maxPheromoneIndex);
                }
            }

            return path;
        }
        public void Run(int countIteration, int countAnts, (double Weight, double Pheromone)[,] matrix, double evaporationRate, double Q)
        {
            for (int i = 0; i < countIteration; i++)
            {
                Run(countAnts, matrix, evaporationRate, Q);
            }
        }

        // Single iteration running
        public void Run(int countAnts, (double Weight, double Pheromone)[,] matrix, double evaporationRate, double Q)
        {
            InitializeAnts(countAnts, matrix.GetLength(0));

            var returnedAnts = new List<Ant>();

            for (int j = 0; j < _ants.Count; j++)
            {
                int startedVertex = _ants[j].Viewed.First();

                int currentVertex = startedVertex;

                do
                {
                    currentVertex = NextVertex(matrix, _ants[j]);

                    if (currentVertex != -1)
                    {
                        _ants[j].AddVertex(currentVertex);
                    }

                } while (currentVertex != -1);

                int endVertex = _ants[j].Viewed.Last();

                if (matrix[endVertex, startedVertex].Weight > 0)
                {
                    returnedAnts.Add(_ants[j]);
                }
            }

            UpdatePheromones(matrix, returnedAnts, evaporationRate, Q);
        }
        private void UpdatePheromones((double Weight, double Pheromone)[,] matrix, List<Ant> ants, double evaporationRate, double Q)
        {
            int countVertex = matrix.GetLength(0);

            for (int i = 0; i < countVertex; i++)
            {
                for (int j = 0; j < countVertex; j++)
                {
                    double sumDeltaPheromone = 0;

                    for (int k = 0; k < ants.Count; k++)
                    {
                        double pheromoneDelta = ants[k].IsEdgeViewed(i, j) ? Q / WeightSum(matrix, ants[k]) : 0;

                        sumDeltaPheromone += pheromoneDelta;
                    }

                    matrix[i, j].Pheromone = (1 - evaporationRate) * matrix[i, j].Pheromone + sumDeltaPheromone;
                }
            }

        }
        private double WeightSum((double Weight, double Pheromone)[,] matrix, Ant ant)
        {
            double weightSum = 0;

            var viewedVertexes = ant.Viewed;

            for (int i = 0; i < viewedVertexes.Count; i++)
            {
                weightSum += matrix[viewedVertexes[i], viewedVertexes[(i + 1) % viewedVertexes.Count]].Weight;
            }

            return weightSum;
        }
        private int NextVertex((double Weight, double Pheromone)[,] matrix, Ant ant)
        {
            int started = ant.Viewed.Last();

            List<int> adjacencyVertexes = new List<int>();

            int countVertex = matrix.GetLength(0);

            for (int k = 0; k < countVertex; k++)
            {
                if (matrix[started, k].Weight > 0 && !ant.IsVertexViewed(k))
                {
                    adjacencyVertexes.Add(k);
                }
            }

            if (adjacencyVertexes.Count == 0)
            {
                return -1;
            }

            Random random = new Random();

            var probabilities = new List<double>();

            for (int i = 0; i < adjacencyVertexes.Count; i++)
            {
                probabilities.Add(MoveProbability(matrix, started, adjacencyVertexes[i], ant));
            }

            List<double> points = new();

            points.Add(0);

            for (int i = 1; i < probabilities.Count + 1; i++)
            {
                points.Add(points[i - 1] + probabilities[i - 1]);
            }

            double randomPoint = random.NextDouble();

            int j = 0;

            while (j < points.Count && points[j] < randomPoint)
            {
                j++;
            }

            j = j == 0 ? 1 : j;

            return adjacencyVertexes[j - 1];
        }
        private double MoveProbability((double Weight, double Pheromone)[,] matrix, int i, int j, Ant ant)
        {
            double sum = 0;

            int countVertex = matrix.GetLength(0);

            for (int k = 0; k < countVertex; k++)
            {
                if (ant.IsVertexViewed(k))
                {
                    continue;
                }

                sum += Math.Pow(matrix[i, k].Pheromone, Alpha) * Math.Pow(1 / matrix[i, k].Weight, Beta);
            }

            return Math.Pow(matrix[i, j].Pheromone, Alpha) * Math.Pow(1 / matrix[i, j].Weight, Beta) / sum;
        }
    }
}
