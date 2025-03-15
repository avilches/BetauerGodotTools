using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.Image;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class CityExit(Intersection intersection, MazeNode mazeFrom, MazeNode destination) {
    public MazeNode From { get; }  = mazeFrom;
    public MazeNode Destination { get; } = destination;
    public Intersection Intersection { get; } = intersection;
}

public class CityMaze {
    public MazeZones MazeZones { get; }
    public MazeGraph MazeGraph => MazeZones.MazeGraph;
    public int SectionSize { get; }
    public Array2D<MazeNode?> NodesGrid { get; }
    public City City { get; }
    public CityGenerator Generator { get; }
    
    public List<CityExit> Exits { get; } = [];

    public CityMaze(MazeZones zones, int sectionSize) {
        MazeZones = zones;
        NodesGrid = zones.MazeGraph.ToArray2D();
        SectionSize = sectionSize;
        City = new City(NodesGrid.Size * sectionSize);
        Generator = new CityGenerator(City);
    }

    /// <summary>
    /// Method example of the steps to use this class
    /// </summary>
    /// <param name="desiredDensity"></param>
    /// <param name="tries"></param>
    public void Generate(float desiredDensity = 0, int tries = 20) {
        Generator.Options.StartPosition = GetStartPosition();
        AddLimits(new Other('#'));
        Generator.Generate(Validate, desiredDensity, tries);
        ProcessSectionsWithMazeNodes();
    }

    public Vector2I GetStartPosition() {
        var mazeStartPosition = (MazeZones.Start.Position - MazeZones.MazeGraph.GetOffset()) * SectionSize;
        return mazeStartPosition + new Vector2I(SectionSize / 2, SectionSize / 2);
    }

    /// <summary>
    /// Call to this method before Generate/Grow paths to ensure the paths follows the maze nodes
    /// </summary>
    /// <param name="limit"></param>
    public void AddLimits(ICityTile limit) {
        var emptyArray = new Array2D<bool>(SectionSize, SectionSize);
        emptyArray.Fill(true);
        var limits = MazeGraph.ToArray2D((pos, node) => {
            if (node == null) return emptyArray;
            var array2D = new Array2D<bool>(SectionSize, SectionSize);
            array2D.Fill(false);
            // if (node != zones.Start) return array2D;
            var lastCol = array2D.Width - 1;
            var lastRow = array2D.Height - 1;
            if (node.Right == null) Draw.Line(lastCol, 0, lastCol, lastRow, 1, (x, y) => array2D[y, x] = true);
            if (node.Left == null) Draw.Line(0, 0, 0, lastRow, 1, (x, y) => array2D[y, x] = true);
            if (node.Up == null) Draw.Line(0, 0, lastCol, 0, 1, (x, y) => array2D[y, x] = true);
            if (node.Down == null) Draw.Line(0, lastRow, lastCol, lastRow, 1, (x, y) => array2D[y, x] = true);
            return array2D;
        }).Expand(SectionSize, (pos, array2D) => array2D);

        foreach (var (pos, value) in limits.GetIndexedValues()) {
            if (value) {
                City.Data[pos] = limit;
            }
        }
    }
    
    /// <summary>
    /// Loop over maze nodes and keep only one path between zones 
    /// </summary>
    public void ProcessSectionsWithMazeNodes() {
        var ignored = new HashSet<Intersection>();
        Exits.Clear();
        for (var gridY = 0; gridY < NodesGrid.Height; gridY++) {
            for (var gridX = 0; gridX < NodesGrid.Width; gridX++) {
                var mazeNode = NodesGrid[gridY, gridX];
                var sectionRect = new Rect2I(gridX * SectionSize, gridY * SectionSize, SectionSize, SectionSize);

                // Sections with no maze, remove all paths
                if (mazeNode == null) {
                    // si has llamado a AddLimit anteriormente, no se habrán generado paths en esta saction. Asi que este bucle no borrará nada...
                    foreach (var path in City.FindPathsInSection(sectionRect).ToList()) {
                        City.RemovePath(path);
                    }
                    
                } else {
                    // Check for upward border
                    if (gridY > 0 && NodesGrid[gridY - 1, gridX] != null) {
                        var lineStart = new Vector2I(sectionRect.Position.X, sectionRect.Position.Y);
                        var lineEnd = new Vector2I(sectionRect.Position.X + sectionRect.Size.X - 1, sectionRect.Position.Y);

                        if (mazeNode.Up != null) {
                            if (mazeNode.Up.ZoneId != mazeNode.ZoneId) {
                                // Create intersections when there is a connection between different zones
                                var nodeIsEntry = mazeNode.Up.ZoneId < mazeNode.ZoneId;
                                if (nodeIsEntry) {
                                    // The other node has the exit to this node, so move the line outside
                                    lineStart += Vector2I.Up;
                                    lineEnd += Vector2I.Up;
                                }
                                var i = CreateIntersectionsOnLine(lineStart, lineEnd);
                                ignored.Add(i);
                                if (nodeIsEntry) {
                                    Exits.Add(new CityExit(i, mazeNode, mazeNode.Up));
                                } else {
                                    Exits.Add(new CityExit(i, mazeNode.Up, mazeNode));
                                }
                            } else {
                                // MazeNodes belongs to the same zone, keep all the connections
                            }

                        } else  {
                            // Remove paths if there is a neighbour on the left but there is no connection bet
                            RemovePathsOnLine(lineStart, lineEnd);
                        }
                    }

                    // Check for leftward border
                    if (gridX > 0 && NodesGrid[gridY, gridX - 1] != null) {
                        var lineStart = new Vector2I(sectionRect.Position.X, sectionRect.Position.Y);
                        var lineEnd = new Vector2I(sectionRect.Position.X, sectionRect.Position.Y + sectionRect.Size.Y - 1);

                        if (mazeNode.Left != null) {
                            if (mazeNode.Left.ZoneId != mazeNode.ZoneId) {
                                // Create intersections when there is a connection between different zones
                                var nodeIsEntry = mazeNode.Left.ZoneId < mazeNode.ZoneId;
                                if (nodeIsEntry) {
                                    // The other node has the exit to this node, so move the line outside
                                    lineStart += Vector2I.Left;
                                    lineEnd += Vector2I.Left;
                                }
                                var i = CreateIntersectionsOnLine(lineStart, lineEnd);
                                ignored.Add(i);
                                if (nodeIsEntry) {
                                    Exits.Add(new CityExit(i, mazeNode, mazeNode.Left));
                                } else {
                                    Exits.Add(new CityExit(i, mazeNode.Left, mazeNode));
                                }
                            } else {
                                // MazeNodes belongs to the same zone, keep all the connections
                            }
                        } else {
                            // Remove paths if there is a neighbour on the left but there is no connection bet
                            RemovePathsOnLine(lineStart, lineEnd);
                        }
                    }
                }
            }
            City.FlatAllIntersections(i => !ignored.Contains(i));
        }
        return;

        // Helper method to remove paths crossing a line defined by start and end points
        void RemovePathsOnLine(Vector2I lineStart, Vector2I lineEnd) {
            var isHorizontal = lineStart.Y == lineEnd.Y;
            Draw.Line(lineStart, lineEnd, 1, (x, y) => {
                if (City.Data[y, x] is not Path path) return;
                
                // Check if this path crosses the line
                var pathCrossesLine = (isHorizontal && (path.Direction == Vector2I.Up || path.Direction == Vector2I.Down)) ||
                                      (!isHorizontal && (path.Direction == Vector2I.Left || path.Direction == Vector2I.Right));

                if (pathCrossesLine) {
                    City.RemovePath(path);
                }
            });
        }

        // Helper method to create intersections on paths crossing a line defined by start and end points
        Intersection CreateIntersectionsOnLine(Vector2I lineStart, Vector2I lineEnd) {
            var isHorizontal = lineStart.Y == lineEnd.Y;
            // Create intersections on paths crossing the line
            var crossingPaths = new List<(Path Path, Vector2I Position)>();
            Draw.Line(lineStart, lineEnd, 1, (x, y) => {
                var pos = new Vector2I(x, y);
                if (City.Data[pos] is Path path) {
                    // Check if this path crosses the line
                    var pathCrossesLine = (isHorizontal && (path.Direction == Vector2I.Up || path.Direction == Vector2I.Down)) ||
                                          (!isHorizontal && (path.Direction == Vector2I.Left || path.Direction == Vector2I.Right));

                    if (pathCrossesLine) {
                        crossingPaths.Add((Path: path, Position: pos));
                    }
                }
                // if (City.Data[pos] == null) {
                    // if ((x + y) % 2 == 0) {
                        // City.Data[pos] = new Other('·');
                    // }
                // }
            });
            if (crossingPaths.Count == 0) {
                throw new Exception("Bug: at least one crossing paths is expected");
            }

            // pseudo random without random
            var hash = HashCode.Combine(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, isHorizontal, crossingPaths.Count);
            var candidate = crossingPaths[Math.Abs(hash) % crossingPaths.Count];

            foreach (var pathPos in crossingPaths.Where(pathPos => pathPos != candidate)) {
                City.RemovePath(pathPos.Path);
            }
            
            var intersection = City.SplitPath(candidate.Path, candidate.Position);
            return intersection;
        }
    }

    public bool Validate() {
        foreach (var (pos, rect) in City.Data.GetRects(SectionSize, SectionSize)) {
            var pathsInSection = City.FindPathsInSection(rect).ToList();
            if (NodesGrid[pos] != null && pathsInSection.Count == 0) {
                return false;
            }
        }
        return true;
    }
}