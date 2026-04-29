using PacMan.App.Models;
using PacMan.App.Services;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PacMan.Tests;

public class GameTests
{
    // helper that builds a small test maze
    private MazeGraph BuildTestMaze()
    {
        var maze = new MazeGraph();
        File.WriteAllLines("test_maze.txt", new[]
        {
            "#####",
            "#...#",
            "#.#.#",
            "#...#",
            "#####"
        });
        maze.Load("test_maze.txt");
        return maze;
    }

    /// test 1: maze loads correct dimensions
    [Fact]
    public void Maze_LoadsCorrectDimensions()
    {
        var maze = BuildTestMaze();
        Assert.Equal(5, maze.Rows);
        Assert.Equal(5, maze.Cols);
    }

    /// test 2: walls are not walkable
    [Fact]
    public void Maze_WallsAreNotWalkable()
    {
        var maze = BuildTestMaze();
        Assert.False(maze.IsWalkable(0, 0));
    }

    /// test 3: open tiles are walkable
    [Fact]
    public void Maze_OpenTilesAreWalkable()
    {
        var maze = BuildTestMaze();
        Assert.True(maze.IsWalkable(1, 1));
    }

    /// test 4: graph neighbors are correct
    [Fact]
    public void MazeGraph_ReturnsCorrectNeighbors()
    {
        var maze = BuildTestMaze();
        var neighbors = maze.GetNeighbors((1, 1));
        Assert.Contains((2, 1), neighbors);
        Assert.Contains((1, 2), neighbors);
    }

    /// test 5: pellet exists before being eaten
    [Fact]
    public void Pellet_ExistsBeforeEating()
    {
        var maze = BuildTestMaze();
        Assert.True(maze.HasPellet(1, 1));
    }

    /// test 6: pellet disappears after being eaten
    [Fact]
    public void Pellet_DisappearsAfterEating()
    {
        var maze = BuildTestMaze();
        maze.EatPellet(1, 1);
        Assert.False(maze.HasPellet(1, 1));
    }

    /// test 7: EatPellet returns true when pellet is there
    [Fact]
    public void EatPellet_ReturnsTrueWhenPelletPresent()
    {
        var maze = BuildTestMaze();
        Assert.True(maze.EatPellet(1, 1));
    }

    /// test 8: EatPellet returns false when already eaten
    [Fact]
    public void EatPellet_ReturnsFalseWhenAlreadyEaten()
    {
        var maze = BuildTestMaze();
        maze.EatPellet(1, 1);
        Assert.False(maze.EatPellet(1, 1));
    }

    /// test 9: ScoreManager saves and loads correctly
    [Fact]
    public void ScoreManager_SavesAndLoads()
    {
        var manager = new ScoreManager("test_scores.json");
        var scores = new Dictionary<string, int> { { "Player", 500 } };
        manager.SaveScores(scores);
        var loaded = manager.LoadScores();
        Assert.Equal(500, loaded["Player"]);
    }

    /// test 10: ScoreManager returns empty dict when file missing
    [Fact]
    public void ScoreManager_ReturnsEmpty_WhenFileNotFound()
    {
        var manager = new ScoreManager("nonexistent.json");
        var scores = manager.LoadScores();
        Assert.Empty(scores);
    }
}