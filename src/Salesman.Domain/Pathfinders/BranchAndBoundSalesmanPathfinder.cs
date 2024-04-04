using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;
public sealed class BranchAndBoundSalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>, IMinMaxValue<V>
{
    public Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default)
    {
        var matrix = graph.ToAdjacencyMatrix();
        var tree = new List<TreeNode>();
        var context = new BracnhAndBoundsContext(matrix, graph.Nodes, graph.Nodes, []);
        var lowerBound = context.Reduce();
        TreeNode? root = null;
        TreeNode? finalNode = null;
        var nodes = new List<TreeNode>();
        while (finalNode is null)
        {
            var (taken, notTaken) = context.Branch(lowerBound);
            taken = taken with { Parent = root };
            notTaken = notTaken with { Parent = root };
            nodes.Add(taken);
            nodes.Add(notTaken);
            var node = tree.Prepend(taken)
                .Prepend(notTaken)
                .MinBy(n => n.Value)!;
            if (node != taken)
            {
                tree.Add(taken);
            }
            if (node != notTaken)
            {
                tree.Add(notTaken);
            }
            if (node != taken && node != notTaken)
            {
                var bound = node.Context.Reduce();
                lowerBound = V.Max(lowerBound, bound);
            }
            if (node.Context.Size == 2)
            {
                var (lastTaken, _) = node.Context.Branch(lowerBound);
                lastTaken = lastTaken with { Parent = node };
                finalNode = new TreeNode(
                    lastTaken.Context.Rows[0],
                    lastTaken.Context.Columns[0],
                    lastTaken.Value,
                    true,
                    lastTaken.Context,
                    lastTaken);
            }
            if (node.Context.Size == 1)
            {
                var final = new TreeNode(
                    node.Context.Rows[0],
                    node.Context.Columns[0],
                    lowerBound + node.Context.Reduce(),
                    node.IsTaken,
                    node.Context,
                    node);
                nodes.Add(final);
                finalNode = final;
            }
            tree.Remove(node);
            lowerBound = V.Max(node.Value, lowerBound);
            context = node.Context;
            root = node;
        }
        var lenght = finalNode.Value;
        var edges = new List<(N From, N To)>();
        while (finalNode is not null)
        {
            if (finalNode.IsTaken)
            {
                edges.Add((finalNode.From, finalNode.To));
            }
            finalNode = finalNode.Parent;
        }
        var currentNode = graph.Nodes[0];
        var path = new List<N> { currentNode };
        while (edges.Count != 0)
        {
            try
            {
                var edge = edges.First(e => e.From.Equals(currentNode));
                path.Add(edge.To);
                edges.Remove(edge);
                currentNode = edge.To;
            }
            catch (Exception)
            {
                throw;
            }
        }
        return Task.FromResult(new PathResult<N, V>(path, lenght));
    }

    private record TreeNode(
        N From,
        N To,
        V Value,
        bool IsTaken,
        BracnhAndBoundsContext Context,
        TreeNode? Parent = null);

    private static V[,] RemoveBranch(V[,] matrix, int row, int column)
    {
        var result = new V[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                result[i, j] = matrix[i, j];
            }
            for (int j = column + 1; j < matrix.GetLength(1); j++)
            {
                result[i, j - 1] = matrix[i, j];
            }
        }
        for (int i = row + 1; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < column; j++)
            {
                result[i - 1, j] = matrix[i, j];
            }
            for (int j = column + 1; j < matrix.GetLength(1); j++)
            {
                result[i - 1, j - 1] = matrix[i, j];
            }
        }
        return result;
    }

    private sealed class BracnhAndBoundsContext
    {
        private readonly V[,] _matrix;
        private readonly N[] _rows;
        private readonly N[] _columns;
        private readonly int _n;
        private readonly IList<N> _visited;
        public BracnhAndBoundsContext(V[,] matrix, IEnumerable<N> rows, IEnumerable<N> columns, IList<N> visited)
        {
            _matrix = matrix;
            _n = matrix.GetLength(0);
            _rows = rows.ToArray();
            _columns = columns.ToArray();
            _visited = visited;
        }

        public int Size => _n;
        public N[] Rows => _rows;

        public N[] Columns => _columns;

        public V Reduce()
        {
            var rowMins = FindRowMins();
            ReduceRows(rowMins);
            var columnMins = FindColumnMins();
            ReduceColumns(columnMins);
            return rowMins.Zip(columnMins)
                .Select(pair => pair.First + pair.Second)
                .Aggregate(V.Zero, (accumulator, value) => accumulator + value);
        }

        public (TreeNode Taken, TreeNode NotTaken) Branch(V grade)
        {
            var (from, to, value) = GetAllZeroEdges().MaxBy(tuple => tuple.Value);
            int i = Array.IndexOf(_rows, from);
            int j = Array.IndexOf(_columns, to);
            int reversedI = Array.IndexOf(_rows, to);
            int reversedJ = Array.IndexOf(_columns, from);
            _matrix[i, j] = V.MaxValue;
            var notTakeMatrix = new V[_n, _n];
            Array.Copy(_matrix, notTakeMatrix, _matrix.Length);
            if (reversedI >= 0 && reversedJ >= 0)
            {
                _matrix[reversedI, reversedJ] = V.MaxValue;
            }
            var reducedMatrix = RemoveBranch(_matrix, i, j);
            var takenContext = new BracnhAndBoundsContext(
                reducedMatrix,
                _rows.Except([from]),
                _columns.Except([to]),
                [.. _visited, from]);
            var takenGrade = takenContext.Reduce();
            var taken = new TreeNode(
                from,
                to,
                V.MaxValue - takenGrade < grade ? V.MaxValue : takenGrade + grade,
                true,
                takenContext);
            var notTaken = new TreeNode(
                from,
                to,
                V.MaxValue - value < grade ? V.MaxValue : grade + value,
                false,
                new BracnhAndBoundsContext(notTakeMatrix, _rows, _columns, _visited));
            return (taken, notTaken);
        }

        public List<V> FindRowMins() => Enumerable.Range(0, _n)
            .Select(i => Enumerable.Range(0, _n)
                    .Select(j => _matrix[i, j]).Min()!)
                .ToList();

        public List<V> FindColumnMins() => Enumerable.Range(0, _n)
            .Select(i => Enumerable.Range(0, _n)
                    .Select(j => _matrix[j, i]).Min()!)
                .ToList();

        public void ReduceRows(IList<V> mins)
        {
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _matrix[i, j] -= mins[i];
                }
            }
        }

        public void ReduceColumns(IList<V> mins)
        {
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _matrix[i, j] -= mins[j];
                }
            }
        }

        public IEnumerable<(N From, N To, V Value)> GetAllZeroEdges() =>
            Enumerable.Range(0, _n)
                .SelectMany(i => Enumerable.Range(0, _n).Select(j => (i, j)))
                .Where(tuple => _matrix[tuple.i, tuple.j] == V.Zero)
                //.Where(tuple => !_visited.Contains(_columns[tuple.j]))
                //.Where(tuple => !_visited.Contains(_rows[tuple.j]))
                .Select(tuple => (
                    _rows[tuple.i],
                    _columns[tuple.j],
                    v: GetGrade(tuple.i, tuple.j)));

        private V GetGrade(int i, int j)
        {
            _matrix[i, j] = V.MaxValue;
            var rowValue = Enumerable.Range(0, _n)
                .Select(column => _matrix[i, column])
                .Min()!;
            var columnValue = Enumerable.Range(0, _n)
                .Select(row => _matrix[row, j])
                .Min()!;
            var result = V.MaxValue - rowValue < columnValue ?
                V.MaxValue : rowValue + columnValue;
            _matrix[i, j] = V.Zero;
            return result;
        }
    }
}
