-- Phoenix Warren

-- CSCI 1260-002: Object-Oriented Programming

-- Spring 2026


This project is a simplified, C# version of Pac-Man built using MonoGame in Visual Studio 2026. The player navigates through a premade maze (inspired by the original 1980 Namco arcade game) while avoiding the ghosts and collecting food pellets.


Enjoy :))

__________________________

***CONTROLS:***

arrow keys (←, ↑, ↓, →): move pac-man
enter key: save score
R: restart
esc (escape): quit

_______________________________

***Tests***
All 10 tests are in `PacMan.Tests/UnitTest1.cs`.


## Tech Stack
| Tool | Purpose |
| C# / .NET 9 | Primary language |
| MonoGame (DesktopGL) | Game framework, rendering, input |
| xUnit | Unit testing |
| System.Text.Json | Score serialization |

---

## Key Features
- Maze loaded from `PacMan.App/Content/Levels/level1.txt` at runtime
- Ghost pathfinding using Dijkstra's algorithm (PriorityQueue)
- High scores saved to `scores.json` between sessions
- Ghost frightened state when power pellets are eaten
- Start screen, game over screen, and win screen

---

## Data Structures Used
| Structure | Where | Why |
| `Dictionary<(int,int), List<(int,int)>>` | MazeGraph | Adjacency list for maze graph |
| `HashSet<(int,int)>` | MazeGraph | Tracks eaten pellets efficiently |
| `PriorityQueue<T,int>` | GhostAI | Dijkstra's pathfinding for ghosts |
| `Dictionary<string,int>` | ScoreManager | Stores named high scores |

---

## Where Data is Stored
- **Maze layout** — `PacMan.App/Content/Levels/level1.txt`
- **High scores** — `scores.json` (created at runtime in the output folder)

---

## UML Diagram
PacMan_UML —> covers all major classes and their relationships including:
	- `Game1`, `Player`, `Ghost`, `MazeGraph`, `GhostAI`, `ScoreManager`, 			`GameState`

-----

## Submission
Submitted via GitHub Classroom: https://github.com/etsucs-scott/project-4-phoenixx-warrenn.git

---

## External Resources
- MonoGame documentation: https://docs.monogame.net
- Dijkstra's algorithm reference: Introduction to Algorithms (CLRS