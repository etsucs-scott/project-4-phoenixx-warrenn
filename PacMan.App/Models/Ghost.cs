using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PacMan.App.Services;

namespace PacMan.App.Models;

/// represents a ghost enemy with position, movement, and state
public class Ghost
{
    public float X { get; set; }
    public float Y { get; set; }
    public Color GhostColor { get; private set; }

    public int DirX { get; set; } = 0;
    public int DirY { get; set; } = 1;

    private float _speed = 2.0f;

    public enum GhostState { Normal, Frightened, Eaten }
    public GhostState State { get; set; } = GhostState.Normal;

    // timer for frightened state
    private float _frightenedTimer = 0f;
    private const float FrightenedDuration = 8.0f;

    public Ghost(float startX, float startY, Color color)
    {
        X = startX;
        Y = startY;
        GhostColor = color;
    }


    /// returns the tile position the ghost currently occupies
    public (int x, int y) TilePosition(int tileSize)
        => ((int)(X + tileSize / 2) / tileSize,
            (int)(Y + tileSize / 2) / tileSize);


    /// sets ghost to frightened state for a fixed duration
    public void Frighten()
    {
        if (State != GhostState.Eaten)
        {
            State = GhostState.Frightened;
            _frightenedTimer = FrightenedDuration;
        }
    }

    /// updates ghost movement using GhostAI for pathfinding
    public void Update(MazeGraph maze, (int x, int y) playerTile,
        int tileSize, float deltaTime)
    {
        // count down frightened timer
        if (State == GhostState.Frightened)
        {
            _frightenedTimer -= deltaTime;
            if (_frightenedTimer <= 0)
                State = GhostState.Normal;
        }

        var (tx, ty) = TilePosition(tileSize);
        bool alignedX = Math.Abs(X - tx * tileSize) < _speed;
        bool alignedY = Math.Abs(Y - ty * tileSize) < _speed;

        if (alignedX && alignedY)
        {
            (int x, int y) target = State == GhostState.Frightened
                ? GetFleeTarget(tx, ty, playerTile, maze)
                : playerTile;

            var next = GhostAI.GetNextMove((tx, ty), target, maze, (DirX, DirY));
            DirX = next.x - tx;
            DirY = next.y - ty;
        }

        X += DirX * _speed;
        Y += DirY * _speed;
    }

    /// returns a target tile away from the player when frightened
    private (int x, int y) GetFleeTarget(int tx, int ty,
        (int x, int y) playerTile, MazeGraph maze)
    {
        // move away from player by picking the opposite direction
        int fleeX = tx + (tx - playerTile.x);
        int fleeY = ty + (ty - playerTile.y);

        // clamp to maze bounds
        fleeX = Math.Clamp(fleeX, 0, maze.Cols - 1);
        fleeY = Math.Clamp(fleeY, 0, maze.Rows - 1);

        return maze.IsWalkable(fleeX, fleeY)
            ? (fleeX, fleeY)
            : (tx, ty);
    }
}