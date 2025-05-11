using System;
using System.Collections.Generic;
using System.Linq;


class Program
{
    // Константы для символов ключей и дверей
    static readonly char[] keys_char = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();
    static readonly char[] doors_char = keys_char.Select(char.ToUpper).ToArray();
    
    // Метод для чтения входных данных
    static List<List<char>> GetInput()
    {
        var data = new List<List<char>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }
        return data;
    }

    struct QueueItem
    {
        public State State;
        public int Steps;
    }

    static int Solve(List<List<char>> data)
    {
        int rows = data.Count;
        if (rows == 0) return -1;
        int cols = data[0].Count;

        List<Tuple<int, int>> robots = new List<Tuple<int, int>>();
        int keyCount = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (data[i][j] == '@')
                {
                    robots.Add(Tuple.Create(i, j));
                }
                else if (data[i][j] >= 'a' && data[i][j] <= 'z')
                {
                    keyCount++;
                }
            }
        }
        if (robots.Count != 4) return -1;
        var initialState = new
        {
            R1 = robots[0].Item1,
            C1 = robots[0].Item2,
            R2 = robots[1].Item1,
            C2 = robots[1].Item2,
            R3 = robots[2].Item1,
            C3 = robots[2].Item2,
            R4 = robots[3].Item1,
            C4 = robots[3].Item2,
            Keys = 0
        };
        var queue = new Queue<QueueItem>();
        queue.Enqueue(new { State = initialState, Steps = 0 });
        var visited = new HashSet<string>();
        visited.Add($"{initialState.R1},{initialState.C1},{initialState.R2},{initialState.C2},{initialState.R3},{initialState.C3},{initialState.R4},{initialState.C4},{initialState.Keys}");
        var directions = new int[][]
        {
            new int[] { -1, 0 },
            new int[] { 1, 0 },
            new int[] { 0, -1 },
            new int[] { 0, 1 }
        };
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var state = current.State;
            var steps = current.Steps;
            var count = 0;
            var keys = state.Keys;
            while (keys != 0)
            {
                count++;
                keys &= keys - 1;
            }
            if (count == keyCount)
                return steps;
            for (var robot = 0; robot < 4; robot++)
            {
                var x, y;
                switch (robot)
                {
                    case 0:
                        x = state.R1;
                        y = state.C1;
                        break;
                    case 1:
                        x = state.R2;
                        y = state.C2;
                        break;
                    case 2:
                        x = state.R3;
                        y = state.C3;
                        break;
                    default:
                        x = state.R4;
                        y = state.C4;
                        break;
                }
                foreach (var dir in directions)
                {
                    var nx = x + dir[0];
                    var ny = y + dir[1];
                    if (nx >= 0 && nx < rows && ny >= 0 && ny < cols && data[nx][ny] != '#')
                    {
                        var cell = data[nx][ny];
                        var newKeys = state.Keys;
                        var canMove = true;
                        if (cell >= 'A' && cell <= 'Z')
                        {
                            var keyNeeded = 1 << (cell - 'A');
                            if ((state.Keys & keyNeeded) == 0)
                                canMove = false;
                        }

                        if (canMove)
                        {
                            if (cell >= 'a' && cell <= 'z')
                                newKeys |= 1 << (cell - 'a'); 
                            QueueItem newState;
                            switch (robot)
                            {
                                case 0:
                                    newState = new
                                    {
                                        R1 = nx,
                                        C1 = ny,
                                        R2 = state.R2,
                                        C2 = state.C2,
                                        R3 = state.R3,
                                        C3 = state.C3,
                                        R4 = state.R4,
                                        C4 = state.C4,
                                        Keys = newKeys
                                    };
                                    break;
                                case 1:
                                    newState = new
                                    {
                                        R1 = state.R1,
                                        C1 = state.C1,
                                        R2 = nx,
                                        C2 = ny,
                                        R3 = state.R3,
                                        C3 = state.C3,
                                        R4 = state.R4,
                                        C4 = state.C4,
                                        Keys = newKeys
                                    };
                                    break;
                                case 2:
                                    newState = new
                                    {
                                        R1 = state.R1,
                                        C1 = state.C1,
                                        R2 = state.R2,
                                        C2 = state.C2,
                                        R3 = nx,
                                        C3 = ny,
                                        R4 = state.R4,
                                        C4 = state.C4,
                                        Keys = newKeys
                                    };
                                    break;
                                default:
                                    newState = new
                                    {
                                        R1 = state.R1,
                                        C1 = state.C1,
                                        R2 = state.R2,
                                        C2 = state.C2,
                                        R3 = state.R3,
                                        C3 = state.C3,
                                        R4 = nx,
                                        C4 = ny,
                                        Keys = newKeys
                                    };
                                    break;
                            }
                            var stateStr = $"{newState.R1},{newState.C1},{newState.R2},{newState.C2},{newState.R3},{newState.C3},{newState.R4},{newState.C4},{newState.Keys}";
                            if (!visited.Contains(stateStr))
                            {
                                visited.Add(stateStr);
                                queue.Enqueue(new { State = newState, Steps = steps + 1 });
                            }
                        }
                    }
                }
            }
        }
        return -1;
    }
    
    static void Main()
    {
        var data = GetInput();
        int result = Solve(data);
        
        if (result == -1)
        {
            Console.WriteLine("No solution found");
        }
        else
        {
            Console.WriteLine(result);
        }
    }
}