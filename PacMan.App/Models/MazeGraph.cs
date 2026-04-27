using System;
using System.Collections.Generic;
using System.IO;

namespace PacMan.App.Models;

/// represents the maze as a graph where each walkable tile is a node and edges connect adjacent walkable tiles
public class MazeGraph
{
    public const int TileSize = 32;

    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public char[,] Tiles { get; private set; } = new char[0, 0];

    // adjacency list — each tile maps to its walkable neighbors
    private Dictionary<(int x, int y), List<(int x, int y)>> _adjacency = new();


    /// loads maze from a text file and builds the graph

    public void Load(string path)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            Rows = lines.Length;
            Cols = lines[0].Length;
            Tiles = new char[Rows, Cols];

            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Cols; x++)
                    Tiles[y, x] = x < lines[y].Length ? lines[y][x] : ' ';

            BuildGraph();
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Maze file not found: {path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading maze: {ex.Message}");
        }
    }


    /// builds adjacency list from loaded tile data

    private void BuildGraph()
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Cols; x++)
            {
                if (Tiles[y, x] == '#') continue;

                var neighbors = new List<(int, int)>();
                foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < Cols && ny < Rows
                        && Tiles[ny, nx] != '#')
                        neighbors.Add((nx, ny));
                }
                _adjacency[(x, y)] = neighbors;
            }
        }
    }


    /// returns walkable neighbors of a given tile position

    public List<(int x, int y)> GetNeighbors((int x, int y) pos)
        => _adjacency.TryGetValue(pos, out var n) ? n : new();


    /// returns true if the tile at (x, y) is walkable (not a wall)

    public bool IsWalkable(int x, int y)
        => x >= 0 && y >= 0 && x < Cols && y < Rows && Tiles[y, x] != '#';
}