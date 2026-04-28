using System;
using Microsoft.Xna.Framework;

namespace PacMan.App.Models;


/// represents the player character, tracking position,movement direction, speed, and lives
public class Player
{
    public float X { get; set; }
    public float Y { get; set; }
    public int Lives { get; set; } = 3;
    public int Score { get; set; } = 0;

    // current movement direction
    public int DirX { get; set; } = 0;
    public int DirY { get; set; } = 0;

    // queued next direction (from player input)
    public int NextDirX { get; set; } = 0;
    public int NextDirY { get; set; } = 0;

    private float _speed = 3.0f;


    /// returns the tile position the player currently occupies
    public (int x, int y) TilePosition(int tileSize)
        => ((int)(X + tileSize / 2) / tileSize,
            (int)(Y + tileSize / 2) / tileSize);


    /// moves player, checking maze for wall collision + queues direction changes for smoother turning
    public void Update(MazeGraph maze, int tileSize)
    {
        var (tx, ty) = TilePosition(tileSize);

        // try to turn if player is aligned to a tile
        bool alignedX = Math.Abs(X - tx * tileSize) < _speed;
        bool alignedY = Math.Abs(Y - ty * tileSize) < _speed;

        if (alignedX && alignedY)
        {
            // try queued direction first
            if (maze.IsWalkable(tx + NextDirX, ty + NextDirY))
            {
                DirX = NextDirX;
                DirY = NextDirY;
            }

            // stop if current direction hits a wall
            if (!maze.IsWalkable(tx + DirX, ty + DirY))
            {
                DirX = 0;
                DirY = 0;
            }
        }

        X += DirX * _speed;
        Y += DirY * _speed;
    }
}