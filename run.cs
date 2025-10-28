using System;
using System.Collections.Generic;

record State
{
    public required System.Collections.Immutable.ImmutableArray<Unit> Units { get; init; }
    public int Cost { get; init; }

    public static bool IsFinal()
    {
        throw new NotImplementedException();
    }
}

record Unit(char Type, int X, int Y);

class Program
{
    static int Solve(List<string> lines)
    {
        var queue = new PriorityQueue<State, int>();
        queue.Enqueue(ParseFirstState(lines), 0);
        while (true)
        {
            var previousState = queue.Dequeue();
            if (State.IsFinal())
                return previousState.Cost;
            var states = GetNextStates(previousState);
            foreach (var state in states)
                queue.Enqueue(state, state.Cost);
        }
    }

    static State ParseFirstState(List<string> lines)
    {
        throw new NotImplementedException();
    }

    static State[] GetNextStates(
        State previousState)
    {
        throw new NotImplementedException();
    }

    static void Main()
    {
        var lines = new List<string>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
            lines.Add(line);
        var result = Solve(lines);
        Console.WriteLine(result);
    }
}