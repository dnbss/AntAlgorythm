namespace AntAlgorythmWinForm
{
    public partial class Form1 : Form
    {
        private Graphics _graphics;

        private Bitmap _graphBitmap;

        private GraphPainter _graphPainter;

        private (double Weight, double Pheromone)[,] _matrix;

        private List<int> _shortestPath;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _graphBitmap = new Bitmap(graphPictureBox.Width, graphPictureBox.Height);

            _graphics = Graphics.FromImage(_graphBitmap);

            _graphPainter = new GraphPainter(_graphics, _graphBitmap);

            _matrix = new (double Weight, double Pheromone)[(int)countVertex.Value, (int)countVertex.Value];

            _graphPainter.DrawCompleteGraph(_matrix);

            graphPictureBox.Image = _graphBitmap;

            ResetPheromones();

            InitializeMatrix();

            endedResultButton.Checked = true;

            shortestPathLabel.Text = "";

            sumWeightLabel.Text = "";

            _shortestPath = new();
        }
        private void countAntsNumeric_ValueChanged(object sender, EventArgs e)
        {

        }
        private void countVertex_ValueChanged(object sender, EventArgs e)
        {
            _matrix = new (double Weight, double Pheromone)[(int)countVertex.Value, (int)countVertex.Value];

            _graphPainter.DrawCompleteGraph(_matrix);

            graphPictureBox.Image = _graphBitmap;

            InitializeMatrix();
        }
        private void randomWeightsButton_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            double startedPheromone = 1;

            int maxRandom = (int)maxRandomNumeric.Value;

            for (int i = 0; i < countVertex.Value; i++)
            {
                for (int j = 0; j < countVertex.Value; j++)
                {
                    if (i != j)
                    {
                        _matrix[i, j] = (random.Next(maxRandom) + 1, startedPheromone);
                    }

                    if (isSimmetricCheckBox.Checked && j <= i)
                    {
                        _matrix[j, i] = _matrix[i, j];
                    }
                }
            }

            InitializeMatrix();
        }
        private void InitializeMatrix()
        {
            matrixGridView.RowCount = (int)countVertex.Value;

            matrixGridView.ColumnCount = (int)countVertex.Value;

            pheromonesGridView.RowCount = (int)countVertex.Value;

            pheromonesGridView.ColumnCount = (int)countVertex.Value;

            for (int i = 0; i < (int)countVertex.Value; i++)
            {
                matrixGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();

                matrixGridView.Columns[i].HeaderCell.Value = (i + 1).ToString();

                pheromonesGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();

                pheromonesGridView.Columns[i].HeaderCell.Value = (i + 1).ToString();
            }

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(0); j++)
                {
                    matrixGridView.Rows[i].Cells[j].Value = _matrix[i, j].Weight;

                    matrixGridView.Columns[j].Width = 40;

                    matrixGridView.Rows[i].Cells[j].ReadOnly = isSimmetricCheckBox.Checked && j <= i;

                    pheromonesGridView.Rows[i].Cells[j].Value = _matrix[i, j].Pheromone;

                    pheromonesGridView.Columns[j].Width = 80;
                }
            }
        }
        private void matrixGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            int column = e.ColumnIndex;

            if (row < 0 || column < 0)
            {
                return;
            }

            var oldValue = _matrix[row, column];

            int newValue = 0;

            if (int.TryParse(matrixGridView.Rows[row].Cells[column].Value.ToString(), out newValue) 
                && newValue > 0)
            {
                _matrix[row, column] = (newValue, _matrix[row, column].Pheromone);
            }
            else
            {
                matrixGridView.Rows[row].Cells[column].Value = oldValue.Weight;
            }

            if (isSimmetricCheckBox.Checked)
            {
                matrixGridView.Rows[column].Cells[row].Value = matrixGridView.Rows[row].Cells[column].Value;
            }
        }
        private void isSimmetricCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = i + 1; j < _matrix.GetLength(0); j++)
                {
                    _matrix[j, i] = _matrix[i, j];
                }
            }

            InitializeMatrix();
        }
        private async void startButton_Click(object sender, EventArgs e)
        {
            AntAlgorythm.AntAlgorythm algorythm = new AntAlgorythm.AntAlgorythm(
                (int)alphaNumeric.Value,
                (int)betaNumeric.Value);

            ResetPheromones();

            if (endedResultButton.Checked)
            {
                startButton.Enabled = false;

                await Task.Run(
                () => algorythm.Run(
                    (int)countIterationsNumeric.Value,
                    (int)countAntsNumeric.Value,
                    _matrix,
                    (double)evaporationRatioNumeric.Value,
                    (double)qRatioNumeric.Value));

                _graphPainter.DrawCompleteGraph(_matrix);

                graphPictureBox.Image = _graphBitmap;

                startButton.Enabled = true;
            }
            else if (iterationResultButton.Checked)
            {
                startButton.Enabled = false;

                await Task.Run(async () =>
                {
                    for (int i = 0; i < (int)countIterationsNumeric.Value; i++)
                    {
                        await Task.Delay((int)speedAnimationNumeric.Value);

                        algorythm.Run((int)countAntsNumeric.Value,
                         _matrix,
                        (double)evaporationRatioNumeric.Value,
                        (double)qRatioNumeric.Value);

                        _graphPainter.DrawCompleteGraph(_matrix);

                        graphPictureBox.Image = _graphBitmap;
                    }
                });

                startButton.Enabled = true;
            }

            for(int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(0); j++)
                {
                    pheromonesGridView.Rows[i].Cells[j].Value = _matrix[i, j].Pheromone;
                }
            }

            _shortestPath = algorythm.GetShortestHamiltonCycle(_matrix);

            shortestPathLabel.Text = String.Join("->", _shortestPath.Select(v => v + 1).ToList());

            sumWeightLabel.Text = ((int)algorythm.SumWeightShortestCycle(_matrix)).ToString();
        }
        public void ResetPheromones()
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(0); j++)
                {
                    _matrix[i, j].Pheromone = i == j ? 0: 1;
                }
            }
        }
        private void showPathButton_Click(object sender, EventArgs e)
        {
            _graphPainter.DrawCycle(_shortestPath);

            graphPictureBox.Image = _graphBitmap;
        }
    }
}