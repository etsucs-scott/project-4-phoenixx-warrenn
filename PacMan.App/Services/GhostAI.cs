using System.Collections.Generic;
using PacMan.App.Models;

namespace PacMan.App.Services;


/// provides pathfinding for ghosts using a PriorityQueue + satisfies the PriorityQueue data structure requirement
public static class GhostAI
{

    /// returns the best next tile for a ghost to move toward its target
    public static (int x, int y) GetNextMove(
        (int x, int y) ghostPos,
        (int x, int y) target,
        MazeGraph maze,
        (int dx, int dy) currentDir)
    {
        var frontier = new PriorityQueue<(int x, int y), int>();
        var cameFrom = new Dictionary<(int, int), (int, int)>();
        var costSoFar = new Dictionary<(int, int), int>();

        frontier.Enqueue(ghostPos, 0);
        costSoFar[ghostPos] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == target) break;

            foreach (var next in maze.GetNeighbors(current))
            {
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, target);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return TraceFirstStep(ghostPos, target, cameFrom);
    }


    /// traces path back from target to find the first step
    private static (int x, int y) TraceFirstStep(
        (int x, int y) start,
        (int x, int y) target,
        Dictionary<(int, int), (int, int)> cameFrom)
    {
        if (!cameFrom.ContainsKey(target)) return start;

        var current = target;
        while (cameFrom.ContainsKey(current) && cameFrom[current] != start)
            current = cameFrom[current];

        return current;
    }

    private static int Heuristic((int x, int y) a, (int x, int y) b)
        => System.Math.Abs(a.x - b.x) + System.Math.Abs(a.y - b.y);
}