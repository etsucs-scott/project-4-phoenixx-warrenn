using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PacMan.App.Models;

namespace PacMan.App;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private MazeGraph _maze = new();
    private Texture2D _wallTexture = null!;
    private Texture2D _pelletTexture = null!;

    private Player _player = new();
    private Texture2D _playerTexture = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // load the maze from file
        _maze.Load("Content/Levels/level1.txt");

        // resize window to fit the maze exactly
        _graphics.PreferredBackBufferWidth = _maze.Cols * MazeGraph.TileSize;
        _graphics.PreferredBackBufferHeight = _maze.Rows * MazeGraph.TileSize;
        _graphics.ApplyChanges();

        base.Initialize();

        _maze.Load("Content/Levels/level1.txt");

        // temporary debug check — remove later!!!!!!!
        Console.WriteLine($"Maze loaded: {_maze.Rows} rows, {_maze.Cols} cols");

        // set player position
        _player.X = 9 * MazeGraph.TileSize;
        _player.Y = 15 * MazeGraph.TileSize;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // create simple colored textures for now !!!!replace with sprites later!!!!
        _wallTexture = new Texture2D(GraphicsDevice, 1, 1);
        _wallTexture.SetData(new[] { Color.DarkBlue });

        _pelletTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pelletTexture.SetData(new[] { Color.White });

        _playerTexture = new Texture2D(GraphicsDevice, 1, 1);
        _playerTexture.SetData(new[] { Color.Yellow });
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        var kb = Keyboard.GetState();

        if (kb.IsKeyDown(Keys.Left)) { _player.NextDirX = -1; _player.NextDirY = 0; }
        if (kb.IsKeyDown(Keys.Right)) { _player.NextDirX = 1; _player.NextDirY = 0; }
        if (kb.IsKeyDown(Keys.Up)) { _player.NextDirX = 0; _player.NextDirY = -1; }
        if (kb.IsKeyDown(Keys.Down)) { _player.NextDirX = 0; _player.NextDirY = 1; }

        _player.Update(_maze, MazeGraph.TileSize);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        int tileSize = MazeGraph.TileSize;

        for (int y = 0; y < _maze.Rows; y++)
        {
            for (int x = 0; x < _maze.Cols; x++)
            {
                var rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                switch (_maze.Tiles[y, x])
                {
                    case '#':
                        _spriteBatch.Draw(_wallTexture, rect, Color.DarkBlue);
                        break;
                    case '.':
                        // draw small pellet dot in center of tile
                        var pelletRect = new Rectangle(
                            x * tileSize + tileSize / 2 - 3,
                            y * tileSize + tileSize / 2 - 3,
                            6, 6);
                        _spriteBatch.Draw(_pelletTexture, pelletRect, Color.White);
                        break;
                }
            }
        }

        var playerRect = new Rectangle(
            (int)_player.X + 2,
            (int)_player.Y + 2,
            MazeGraph.TileSize - 4,
            MazeGraph.TileSize - 4);
            _spriteBatch.Draw(_playerTexture, playerRect, Color.Yellow);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}