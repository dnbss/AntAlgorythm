using AntAlgorythm;

double alpha = 1;

double beta = 2;

(double Weight, double Pheromone)[,] matrix = new (double Weight, double Pheromone)[5, 5]
{
    { (0, 0), (1000, 0), (1, 0), (1, 0), (1000, 0)},
    { (1000, 0), (0, 0), (1000, 0), (1, 0), (1, 0) },
    { (1, 0), (1000, 0), (0, 0), (1000, 0), (1, 0) },
    { (1, 0), (1, 0), (1000, 0), (0, 0), (1000, 0) },
    { (1000, 0), (1, 0), (1, 0), (1000, 0), (0, 0) }
};

AntAlgorythm.AntAlgorythm algorythm = new AntAlgorythm.AntAlgorythm();

algorythm.Alpha = alpha;
algorythm.Beta = beta;

algorythm.InitializeAnts(100, matrix.GetLength(0));

algorythm.ResetPheromones(matrix);

algorythm.Run(100, matrix, 0.5, 50);

for (int i = 0; i < matrix.GetLength(0); i++)
{
    for (int j = 0; j < matrix.GetLength(1); j++)
    {
        Console.Write($"{matrix[i, j]} ");
    }

    Console.WriteLine();
}