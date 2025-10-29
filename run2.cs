using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(List<(string, string)> edges)
    {
        var result = new List<string>();
        var adj = new Dictionary<string, HashSet<string>>();
        foreach (var edgePair in edges)
        {
            if (!adj.ContainsKey(edgePair.Item1)) adj[edgePair.Item1] = new HashSet<string>();
            adj[edgePair.Item1].Add(edgePair.Item2);
            if (!adj.ContainsKey(edgePair.Item2)) adj[edgePair.Item2] = new HashSet<string>();
            adj[edgePair.Item2].Add(edgePair.Item1);
        }

        var gates = adj.Keys.Where(x => char.IsUpper(x[0])).ToHashSet();
        var position = "a";
        while (true)
        {
            if (FindPathToClosestGate(adj, position, gates).Length == 0)
                break;
            var possibleMoves = new List<string>();
            foreach (var gate in gates.OrderBy(g => g))
                if (adj.TryGetValue(gate, out var value))
                    foreach (var neighbor in value.OrderBy(n => n))
                        possibleMoves.Add($"{gate}-{neighbor}");

            string bestMove = null;
            foreach (var move in possibleMoves)
                if (IsWinningMove(adj, gates, position, move))
                {
                    bestMove = move;
                    break;
                }

            var parts = bestMove.Split('-');
            var node1 = parts[0];
            var node2 = parts[1];
            adj[node1].Remove(node2);
            adj[node2].Remove(node1);
            result.Add(bestMove);
            var virusMovePath = FindPathToClosestGate(adj, position, gates);
            if (virusMovePath.Length > 0)
                position = virusMovePath[0].Item2;
        }

        return result;
    }

    private static bool IsWinningMove(
        Dictionary<string, HashSet<string>> currentAdj,
        ISet<string> gates,
        string currentVirusPos,
        string move)
    {
        var adjCopy = new Dictionary<string, HashSet<string>>();
        foreach (var item in currentAdj)
            adjCopy[item.Key] = [..item.Value];
        var parts = move.Split('-');
        adjCopy[parts[0]].Remove(parts[1]);
        adjCopy[parts[1]].Remove(parts[0]);
        var virusMovePath = FindPathToClosestGate(adjCopy, currentVirusPos, gates);
        if (virusMovePath.Length > 0)
            currentVirusPos = virusMovePath[0].Item2;

        while (true)
        {
            var path = FindPathToClosestGate(adjCopy, currentVirusPos, gates);
            if (path.Length == 0) return true;
            var gateToCut = path.Last().Item2;
            var nodeToCut = path.Last().Item1;
            adjCopy[gateToCut].Remove(nodeToCut);
            adjCopy[nodeToCut].Remove(gateToCut);
            var nextVirusPath = FindPathToClosestGate(adjCopy, currentVirusPos, gates);
            if (nextVirusPath.Length > 0)
                currentVirusPos = nextVirusPath[0].Item2;
        }
    }

    private static (string, string)[] FindPathToClosestGate(Dictionary<string, HashSet<string>> edgesDict,
        string position,
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