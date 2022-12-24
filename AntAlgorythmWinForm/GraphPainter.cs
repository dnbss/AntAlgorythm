using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntAlgorythmWinForm
{
    public class GraphPainter
    {
        private Graphics _graphics;

        private Bitmap _bitmap;

        private List<PointF> _vertexPoints;

        public GraphPainter(Graphics graphics, Bitmap bitmap)
        {
            _graphics = graphics;

            _bitmap = bitmap;

            _vertexPoints = new List<PointF>();
        }

        private void BuildVertexes(int countVertex, double radiusGraph)
        {
            _vertexPoints.Clear();

            double angle = 2 * Math.PI / countVertex;

            PointF center = new PointF(_bitmap.Width / 2, _bitmap.Height / 2);

            for (int i = 0; i < countVertex; i++)
            {
                _vertexPoints.Add(new PointF((float)(center.X + radiusGraph * Math.Sin(i * angle))
                    , (float)(center.Y + radiusGraph * Math.Cos(i * angle))));
            }
        }

        public void DrawCompleteGraph((double Weight, double Pheromone)[,] matrix)
        {
            int countVertex = matrix.GetLength(0);

            _graphics.Clear(Color.White);

            double radiusVertex = 13;

            double radiusGraph = 215;

            BuildVertexes(countVertex, radiusGraph);

            DrawEdges(matrix);

            DrawVertexes(countVertex, radiusVertex);
        }
        public void DrawVertexes(int countVertex, double radiusVertex)
        {
            for (int i = 0; i < countVertex; i++)
            {
                _graphics.FillEllipse(
                    new SolidBrush(Color.Gold),
                    (float)(_vertexPoints[i].X - radiusVertex),
                    (float)(_vertexPoints[i].Y - radiusVertex),
                    (float)(2 * radiusVertex),
                    (float)(2 * radiusVertex));

                _graphics.DrawString(
                    (i + 1).ToString(),
                    new Font("Arial", 10),
                    new SolidBrush(Color.Black),
                    new PointF(_vertexPoints[i].X - 7, _vertexPoints[i].Y - 7));
            }
        }
        public void DrawEdges((double Weight, double Pheromone)[,] matrix)
        {
            Color color = Color.LightBlue;

            int width = 3;

            double maxPheromone = MaxPheromone(matrix);

            for (int i = 0; i < _vertexPoints.Count; i++)
            {
                for (int j = 0; j < _vertexPoints.Count; j++)
                {
                    int newColorA = maxPheromone == 0
                        ? color.A
                        : (int)(color.A * (matrix[i, j].Pheromone / maxPheromone));

                    int newColorR = maxPheromone == 0 
                        ? color.R 
                        : (int)(color.R * (matrix[i, j].Pheromone / maxPheromone + 1));

                    int newColorG = maxPheromone == 0
                        ? color.G
                        : (int)(color.G * (matrix[i, j].Pheromone / maxPheromone));

                    int newColorB = maxPheromone == 0
                        ? color.B
                        : (int)(color.B * (matrix[i, j].Pheromone / maxPheromone));

                    int max = newColorR;

                    if (max < newColorG)
                    {
                        max = newColorG;
                    }

                    if (max < newColorB)
                    {
                        max = newColorB;
                    }

                    newColorR = (int)((double)newColorR / max * 255);
                    newColorG = (int)((double)newColorG / max * 255);
                    newColorB = (int)((double)newColorB / max * 255);

                    Color newColor = Color.FromArgb(newColorA, newColorR, newColorG, newColorB);

                    _graphics.DrawLine(
                        new Pen(new SolidBrush(newColor), width),
                        _vertexPoints[i], 
                        _vertexPoints[j]);
                }
            }
        }

        public void DrawCycle(List<int> vertexes)
        {
            Color color = Color.Green;

            int width = 5;

            for (int i = 0; i < vertexes.Count; i++)
            {
                _graphics.DrawLine(
                        new Pen(new SolidBrush(color), width),
                        _vertexPoints[vertexes[i]],
                        _vertexPoints[vertexes[(i + 1) % vertexes.Count]]);
            }

            double radiusVertex = 13;

            DrawVertexes(vertexes.Count, radiusVertex);
        }
        private double MaxPheromone((double Weight, double Pheromone)[,] matrix)
        {
            double result = 0;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (result < matrix[i,j].Pheromone)
                    {
                        result = matrix[i, j].Pheromone;
                    }
                }
            }

            return result;
        }
    }
}
