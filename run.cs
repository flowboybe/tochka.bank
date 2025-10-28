using System;
using System.Collections.Generic;
using System.Collections.Immutable;

record State
{
    public required ImmutableArray<Unit> Units { get; init; }
    public int Cost { get; init; }

    public bool IsUnitShouldStay(Unit unit)
    {
        var targetX = unit.Type switch
        {
            'A' => 3,
            'B' => 5,
            'C' => 7,
            'D' => 9,
            _ => -1
        };
        if (unit.X != targetX || unit.Y < 2)
            return false;
        foreach (var u in Units)
        {
            if (u.X == unit.X && u.Y > unit.Y && u.Type != unit.Type)
                return false;
        }

        return true;
    }

    public bool IsFinal() => Units.All(IsUnitShouldStay);
}

record Unit(char Type, int X, int Y);

class Program
{
    private static readonly Dictionary<char, int> Costs = new()
    {
        { 'A', 1 },
        { 'B', 10 },
        { 'C', 100 },
        { 'D', 1000 },
    };

    static int Solve(List<string> lines)
    {
        var queue = new PriorityQueue<State, int>();
        var visited = new HashSet<string>();
        queue.Enqueue(ParseFirstState(lines), 0);
        while (queue.Count > 0)
        {
            var previousState = queue.Dequeue();
            var key = string.Join(",", previousState.Units.Select(u => $"{u.Type}{u.X}{u.Y}"));
            if (visited.Contains(key))
                continue;
            visited.Add(key);
            if (previousState.IsFinal())
                return previousState.Cost;
            var states = GetNextStates(previousState);
            foreach (var state in states)
                queue.Enqueue(state, state.Cost);
        }

        throw new Exception("No solution found");
    }

    static State ParseFirstState(List<string> lines)
    {
        var units = new List<Unit>();
        for (var y = 0; y < lines.Count; y++)
        for (var x = 0; x < lines[y].Length; x++)
            if (Costs.ContainsKey(lines[y][x]))
                units.Add(new Unit(lines[y][x], x, y));
        return new State
        {
            Units = [..units],
            Cost = 0
        };
    }

    static State[] GetNextStates(State previousState)
    {
        var nextStates = new List<State>();
        var occupiedPositions = previousState.Units.ToDictionary(u => (u.X, u.Y));
        var roomDepth = previousState.Units.Length / 4;

        for (var i = 0; i < previousState.Units.Length; i++)
        {
            var unit = previousState.Units[i];

            if (previousState.IsUnitShouldStay(unit))
                continue;

            var unitType = unit.Type;
            var costPerStep = Costs[unitType];

            var targetRoomX = unitType switch
            {
                'A' => 3, 'B' => 5, 'C' => 7, 'D' => 9, _ => -1
            };

            if (unit.Y > 1) 
            {
                var canMoveOutOfRoom = true;
                for (var y = unit.Y - 1; y >= 2; y--)
                {
                    if (occupiedPositions.ContainsKey((unit.X, y)))
                    {
                        canMoveOutOfRoom = false;
                        break;
                    }
                }
                if (!canMoveOutOfRoom) continue;
                
                for (var targetX = unit.X - 1; targetX >= 1; targetX--)
                {
                    if (occupiedPositions.ContainsKey((targetX, 1))) break; 
                    if (targetX == 3 || targetX == 5 || targetX == 7 || targetX == 9) continue;
                    var distance = (unit.Y - 1) + (unit.X - targetX);
                    var newCost = previousState.Cost + distance * costPerStep;
                    var newUnits = previousState.Units.SetItem(i, new Unit(unitType, targetX, 1));
                    nextStates.Add(new State { Units = newUnits, Cost = newCost });
                }

                for (var targetX = unit.X + 1; targetX <= 11; targetX++)
                {
                    if (occupiedPositions.ContainsKey((targetX, 1))) break;
                    if (targetX == 3 || targetX == 5 || targetX == 7 || targetX == 9) continue;

                    var distance = (unit.Y - 1) + (targetX - unit.X);
                    var newCost = previousState.Cost + distance * costPerStep;
                    var newUnits = previousState.Units.SetItem(i, new Unit(unitType, targetX, 1));
                    nextStates.Add(new State { Units = newUnits, Cost = newCost });
                }
            }
            
            else
            {
                var roomIsReady = previousState.Units.Where(u => u.X == targetRoomX).All(u => u.Type == unitType);
                if (!roomIsReady) continue;

                var pathIsClear = true;
                if (unit.X < targetRoomX) {
                    for (var x = unit.X + 1; x <= targetRoomX; x++) {
                        if (occupiedPositions.ContainsKey((x, 1))) { pathIsClear = false; break; }
                    }
                } else {
                    for (int x = unit.X - 1; x >= targetRoomX; x--) {
                        if (occupiedPositions.ContainsKey((x, 1))) { pathIsClear = false; break; }
                    }
                }
                if (!pathIsClear) continue;
                var unitsInRoomYs = previousState.Units
                    .Where(u => u.X == targetRoomX)
                    .Select(u => u.Y)
                    .ToHashSet();

                var targetY = -1;
                for (var y = 1 + roomDepth; y >= 2; y--)
                {
                    if (!unitsInRoomYs.Contains(y))
                    {
                        targetY = y;
                        break;
                    }
                }
                if (targetY == -1) continue;

                var distance = Math.Abs(targetRoomX - unit.X) + (targetY - 1);
                var newCost = previousState.Cost + distance * costPerStep;
                var newUnits = previousState.Units.SetItem(i, new Unit(unitType, targetRoomX, targetY));
                nextStates.Add(new State { Units = newUnits, Cost = newCost });
            }
        }
        return nextStates.ToArray();
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