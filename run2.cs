using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(List<(string, string)> edges)
    {
        var result = new List<string>();
        var edgesDict = new Dictionary<string, HashSet<string>>();
        foreach (var edgePair in edges)
        {
            if (!edgesDict.ContainsKey(edgePair.Item1))
                edgesDict[edgePair.Item1] = [];
            edgesDict[edgePair.Item1].Add(edgePair.Item2);
            if (!edgesDict.ContainsKey(edgePair.Item2))
                edgesDict[edgePair.Item2] = [];
            edgesDict[edgePair.Item2].Add(edgePair.Item1);
        }

        var gates = edgesDict
            .Where(x => char.IsUpper(x.Key[0]))
            .Select(x => x.Key)
            .ToHashSet();
        var position = "a";
        while (true)
        {
            var targetPath = FindPathToClosestGate(edgesDict, position, gates);
            if (targetPath.Length == 0)
                break;
            
            var edgeToCLose = string.Join("-", targetPath[^1].Item2, targetPath[^1].Item1);
            edgesDict[targetPath[^1].Item1].Remove(targetPath[^1].Item2);
            edgesDict[targetPath[^1].Item2].Remove(targetPath[^1].Item1);
            var movePath = FindPathToClosestGate(edgesDict, position, gates);
            if (movePath.Length > 0)
                position = movePath[0].Item2;
            result.Add(edgeToCLose);
        }

        return result;
    }

    private static (string, string)[] FindPathToClosestGate(Dictionary<string, HashSet<string>> edgesDict, string position,
        ISet<string> gates)
    {
        var pathsToGates = new List<(string, string)[]>();
        foreach (var gate in gates)
        {
            var path = FindMinPathToGate(edgesDict, position, gate);
            if (path.Length > 0)
                pathsToGates.Add(path);
        }
        if (pathsToGates.Count == 0)
            return Array.Empty<(string, string)>();
        pathsToGates
            .Sort((x, y) =>
            {
                if (x.Length.CompareTo(y.Length) != 0) return x.Length.CompareTo(y.Length);
                return x[^1].Item2[0].CompareTo(y[^1].Item2[0]);
            });
        return pathsToGates[0];
    }

    private static (string, string)[] FindMinPathToGate(Dictionary<string, HashSet<string>> edgesDict,
        string position, string gate)
    {
        if (position == gate)
            return new List<(string, string)>().ToArray();
        var queue = new Queue<string>();
        var visited = new Dictionary<string, string>();
        var pathEdges = new List<(string, string)>();
        queue.Enqueue(position);
        visited[position] = null;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == gate)
            {
                var node = gate;
                while (visited[node] != null)
                {
                    pathEdges.Add((visited[node], node));
                    node = visited[node];
                }
                pathEdges.Reverse();
                return pathEdges.ToArray();
            }
            if (edgesDict.TryGetValue(current, out var value))
                foreach (var neighbor in value.OrderBy(n => n))
                    if (visited.TryAdd(neighbor, current))
                        queue.Enqueue(neighbor);
        }
        return [];
    }

    static void Main()
    {
        var edges = new List<(string, string)>();
        string line;

        while ((line = Console.ReadLine()) != null && line.Length > 0)
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('-');
                if (parts.Length == 2)
                {
                    edges.Add((parts[0], parts[1]));
                }
            }
        }

        var result = Solve(edges);
        foreach (var edge in result)
        {
            Console.WriteLine(edge);
        }
    }
}