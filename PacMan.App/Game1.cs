using System;
using System.Collections.Generic;
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

    private SpriteFont _scoreFont = null!;
    private Services.ScoreManager _scoreManager = new();
    private Dictionary<string, int> _highScores = new();

    private List<Ghost> _ghosts = new();
    private Texture2D _ghostTexture = null!;

    private GameState _gameState = GameState.StartScreen;

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
        // Console.WriteLine($"Maze loaded: {_maze.Rows} rows, {_maze.Cols} cols");

        // set player position
        _player.X = 9 * MazeGraph.TileSize;
        _player.Y = 15 * MazeGraph.TileSize;

        // ghosts
        _ghosts.Add(new Ghost(9 * MazeGraph.TileSize, 8 * MazeGraph.TileSize, Color.Red));
        _ghosts.Add(new Ghost(10 * MazeGraph.TileSize, 8 * MazeGraph.TileSize, Color.Pink));
        _ghosts.Add(new Ghost(9 * MazeGraph.TileSize, 9 * MazeGraph.TileSize, Color.Cyan));
        _ghosts.Add(new Ghost(10 * MazeGraph.TileSize, 9 * MazeGraph.TileSize, Color.Orange));
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

        // fonts and high scores
        _scoreFont = Content.Load<SpriteFont>("FontScore");
        _highScores = _scoreManager.LoadScores();

        _ghostTexture = new Texture2D(GraphicsDevice, 1, 1);
        _ghostTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var kb = Keyboard.GetState();

        switch (_gameState)
        {
            case GameState.StartScreen:
                // Press Enter to start
                if (kb.IsKeyDown(Keys.Enter))
                    _gameState = GameState.Playing;
                break;

            case GameState.Playing:
                if (kb.IsKeyDown(Keys.Left)) { _player.NextDirX = -1; _player.NextDirY = 0; }
                if (kb.IsKeyDown(Keys.Right)) { _player.NextDirX = 1; _player.NextDirY = 0; }
                if (kb.IsKeyDown(Keys.Up)) { _player.NextDirX = 0; _player.NextDirY = -1; }
                if (kb.IsKeyDown(Keys.Down)) { _player.NextDirX = 0; _player.NextDirY = 1; }

                _player.Update(_maze, MazeGraph.TileSize);

                // pellet collection
                var (tx, ty) = _player.TilePosition(MazeGraph.TileSize);
                if (_maze.EatPellet(tx, ty))
                    _player.Score += 10;

                // ghost updates
                var playerTile = _player.TilePosition(MazeGraph.TileSize);
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                foreach (var ghost in _ghosts)
                {
                    ghost.Update(_maze, playerTile, MazeGraph.TileSize, delta);

                    var ghostTile = ghost.TilePosition(MazeGraph.TileSize);
                    if (ghostTile == playerTile)
                    {
                        if (ghost.State == Ghost.GhostState.Frightened)
                        {
                            ghost.State = Ghost.GhostState.Eaten;
                            _player.Score += 200;
                        }
                        else if (ghost.State == Ghost.GhostState.Normal)
                        {
                            _player.Lives--;
                            _player.X = 9 * MazeGraph.TileSize;
                            _player.Y = 15 * MazeGraph.TileSize;
                        }
                    }
                }

                // check win/loss conditions
                if (_player.Lives <= 0)
                {
                    _highScores["Player"] = _player.Score;
                    _scoreManager.SaveScores(_highScores);
                    _gameState = GameState.GameOver;
                }

                if (_maze.AllPelletsEaten())
                {
                    _highScores["Player"] = _player.Score;
                    _scoreManager.SaveScores(_highScores);
                    _gameState = GameState.Win;
                }
                break;

            case GameState.GameOver:
            case GameState.Win:
                // press R to restart
                if (kb.IsKeyDown(Keys.R))
                    ResetGame();
                break;
        }

        base.Update(gameTime);
    }

    /// resets all game state back to starting conditions
    private void ResetGame()
    {
        _maze.Load("Content/Levels/level1.txt");
        _player.X = 9 * MazeGraph.TileSize;
        _player.Y = 15 * MazeGraph.TileSize;
        _player.DirX = 0;
        _player.DirY = 0;
        _player.Lives = 3;
        _player.Score = 0;

        _ghosts.Clear();
        _ghosts.Add(new Ghost(9 * MazeGraph.TileSize, 8 * MazeGraph.TileSize, Color.Red));
        _ghosts.Add(new Ghost(10 * MazeGraph.TileSize, 8 * MazeGraph.TileSize, Color.Pink));
        _ghosts.Add(new Ghost(9 * MazeGraph.TileSize, 9 * MazeGraph.TileSize, Color.Cyan));
        _ghosts.Add(new Ghost(10 * MazeGraph.TileSize, 9 * MazeGraph.TileSize, Color.Orange));

        _gameState = GameState.Playing;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        switch (_gameState)
        {
            case GameState.StartScreen:
                _spriteBatch.DrawString(_scoreFont,
                    "PAC-MAN",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 40,
                        _maze.Rows * MazeGraph.TileSize / 2 - 40),
                    Color.Yellow);
                _spriteBatch.DrawString(_scoreFont,
                    "Press ENTER to Start",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 90,
                        _maze.Rows * MazeGraph.TileSize / 2),
                    Color.White);
                break;

            case GameState.Playing:
                DrawMaze();
                DrawPlayer();
                DrawGhosts();
                DrawHUD();
                break;

            case GameState.GameOver:
                _spriteBatch.DrawString(_scoreFont,
                    "GAME OVER",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 45,
                        _maze.Rows * MazeGraph.TileSize / 2 - 40),
                    Color.Red);
                _spriteBatch.DrawString(_scoreFont,
                    $"Final Score: {_player.Score}",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 70,
                        _maze.Rows * MazeGraph.TileSize / 2),
                    Color.White);
                _spriteBatch.DrawString(_scoreFont,
                    "Press R to Restart",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 85,
                        _maze.Rows * MazeGraph.TileSize / 2 + 40),
                    Color.White);
                break;

            case GameState.Win:
                _spriteBatch.DrawString(_scoreFont,
                    "YOU WIN!",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 40,
                        _maze.Rows * MazeGraph.TileSize / 2 - 40),
                    Color.Yellow);
                _spriteBatch.DrawString(_scoreFont,
                    $"Final Score: {_player.Score}",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 70,
                        _maze.Rows * MazeGraph.TileSize / 2),
                    Color.White);
                _spriteBatch.DrawString(_scoreFont,
                    "Press R to Restart",
                    new Vector2(
                        _maze.Cols * MazeGraph.TileSize / 2 - 85,
                        _maze.Rows * MazeGraph.TileSize / 2 + 40),
                    Color.White);
                break;
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    /// draws all maze tiles
    private void DrawMaze()
    {
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
                        if (_maze.HasPellet(x, y))
                        {
                            var pelletRect = new Rectangle(
                                x * tileSize + tileSize / 2 - 3,
                                y * tileSize + tileSize / 2 - 3,
                                6, 6);
                            _spriteBatch.Draw(_pelletTexture, pelletRect, Color.White);
                        }
                        break;
                }
            }
        }
    }

    /// draws the player
    private void DrawPlayer()
    {
        var playerRect = new Rectangle(
            (int)_player.X + 2,
            (int)_player.Y + 2,
            MazeGraph.TileSize - 4,
            MazeGraph.TileSize - 4);
        _spriteBatch.Draw(_playerTexture, playerRect, Color.Yellow);
    }

    /// draws all ghosts
    private void DrawGhosts()
    {
        foreach (var ghost in _ghosts)
        {
            var ghostColor = ghost.State == Ghost.GhostState.Frightened
                ? Color.Blue
                : ghost.GhostColor;
            var ghostRect = new Rectangle(
                (int)ghost.X + 2,
                (int)ghost.Y + 2,
                MazeGraph.TileSize - 4,
                MazeGraph.TileSize - 4);
            _spriteBatch.Draw(_ghostTexture, ghostRect, ghostColor);
        }
    }

    /// draws score and lives HUD
    private void DrawHUD()
    {
        _spriteBatch.DrawString(
            _scoreFont,
            $"Score: {_player.Score}   Lives: {_player.Lives}   Hi: {(_highScores.ContainsKey("Player") ? _highScores["Player"] : 0)}",
            new Vector2(8, 8),
            Color.White);
    }
}