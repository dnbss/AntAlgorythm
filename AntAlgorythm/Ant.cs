namespace AntAlgorythm
{
    public class Ant
    {
        private List<int> _viewed;
        public List<int> Viewed => _viewed.ToList();
        public Ant()
        {
            _viewed = new List<int>();
        }
        public Ant(int startedVertex)
        {
            _viewed = new List<int>();

            _viewed.Add(startedVertex);
        }
        public void AddVertex(int vertex)
        { 
            if (_viewed.Contains(vertex))
            {
                throw new ArgumentException("this vertex is viewed");
            }

            _viewed.Add(vertex);
        }

        public void ChangeStartedVertex(int vertex)
        {
            _viewed[0] = vertex;
        }

        public void ClearViewed()
        {
            int started = _viewed.First();

            _viewed.Clear();

            _viewed.Add(started);
        }

        public bool IsVertexViewed(int vertex) => _viewed.Contains(vertex);

        public bool IsEdgeViewed(int startedVertex, int endedVertex)
        {
            if (!(_viewed.Contains(startedVertex) && _viewed.Contains(endedVertex)))
            {
                return false;
            }

            return (_viewed.IndexOf(startedVertex) + 1) % _viewed.Count == _viewed.IndexOf(endedVertex);
        }
    }
}