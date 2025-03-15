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
    public MazeNode From { get; } = mazeFrom;
    public MazeNode Destination { get; } = destination;
    public Intersection Intersection { get; } = intersection;
    public Path FromPath { get; set; }
    public Path DestinationPath { get; set; }
}

public static class CityMazeExtensions {
    public static MazeNode? GetMazeNode(this Path path) => path.Attributes().GetAs<MazeNode>("mazeNode");
    public static void SetMazeNode(this Path path, MazeNode mazeNode) => path.Attributes().Set("mazeNode", mazeNode);
    public static MazeNode? GetMazeNode(this Intersection i) => i.Attributes().GetAs<MazeNode>("mazeNode");
    public static void SetMazeNode(this Intersection i, MazeNode mazeNode) => i.Attributes().Set("mazeNode", mazeNode);

    public static void AddPath(this MazeNode node, Path path) => node.GetPaths().Add(path);
    public static List<Path?> GetPaths(this MazeNode node) => node.Attributes().GetOrCreate<List<Path?>>("paths", () => []);
}

public class CityMaze {
    public MazeZones MazeZones { get; }
    public MazeGraph MazeGraph => MazeZones.MazeGraph;
    public int SectionSize { get; }
    public Array2D<MazeNode?> NodesGrid { get; }
    public City City { get; }
    public CityGenerator Generator { get; }

    public List<CityExit> Exits { get; } = [];
    
    // CrossingPaths are the paths that start in one section and end in another. Maybe the sections belongs to the same zone:
    // 1111111122222222
    // 11X==========X22
    // 1111111122222222
    //
    // or maybe it's a different zone. In this case, the intersection is just in the border
    // 1111111122222222
    // 1111111X=====X22
    // 1111111122222222
    
    // It could be the case a path is so tiny that it has no body, just 2 intersections together
    // 1111111122222222
    // 1111111XX2222222
    // 1111111122222222
    public List<Path> CrossingPaths { get; } = [];

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
        AddLimits(new Other('#'));
        Generator.Options.StartPosition = GetStartPosition();
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
    /// Loop over maze nodes and keep only one path between zones.
    /// The path that connects one node with the other:
    ///  - It has an intersection in the zone with the lower id. So, the path from zone 0 to zone 1 has an intersection in the zone in the border.
    ///  - The intersection and the two paths have an Attributes().Get("mazeNode") accesible using GetMazeZone()
    ///
    /// All the intersections has Attributes().Get("mazeNode") accesible using intersection.GetMazeZone()
    
    /// All the paths that start and end in the same zone are stored in:
    /// - mazeNode.Attributes().Get("paths") accesible using mazeNode.GetPaths()
    /// - the path has the zone in Attributes().Get("mazeNode") accesible using path.GetMazeZone()
    ///
    /// The path that belongs to two maze zone don't have path.GetMazeZone() and are stored in CrossingPaths
    /// </summary>
    public void ProcessSectionsWithMazeNodes() {
        var ignored = new HashSet<Intersection>();
        Exits.Clear();
        foreach (var ((gridX, gridY), sectionRect) in City.Data.GetRects(SectionSize, SectionSize)) {
            var mazeNode = NodesGrid[gridY, gridX];

            // Sections with no maze, remove all paths
            if (mazeNode == null) {
                // si has llamado a AddLimit anteriormente, no se habrán generado paths en esta section. Asi que este bucle no borrará nada...
                foreach (var path in City.FindPathsInSection(sectionRect).ToList()) {
                    City.RemovePath(path);
                }
            } else {
                foreach (var i in City.FindIntersectionsInSection(sectionRect).ToList()) {
                    i.SetMazeNode(mazeNode);
                }

                // Check for upward border
                if (gridY > 0 && NodesGrid[gridY - 1, gridX] != null) {
                    var lineStart = new Vector2I(sectionRect.Position.X, sectionRect.Position.Y);
                    var lineEnd = new Vector2I(sectionRect.Position.X + sectionRect.Size.X - 1, sectionRect.Position.Y);

                    if (mazeNode.Up != null) {
                        if (mazeNode.Up.ZoneId != mazeNode.ZoneId) {
                            // Create intersections when there is a connection between different zones
                            var upLowerZone = mazeNode.Up.ZoneId < mazeNode.ZoneId;
                            var direction = upLowerZone ? Vector2I.Down : Vector2I.Up;
                            if (upLowerZone) {
                                // The other node has the exit to this entry node, so move the line outside
                                // because the intersection should belong to the exit node
                                lineStart += Vector2I.Up;
                                lineEnd += Vector2I.Up;
                            }
                            var i = CreateIntersectionsOnLine(lineStart, lineEnd, direction);
                            ignored.Add(i);

                            // Find the paths connecting to this intersection
                            var upPath = i.FindPathTo(Vector2I.Up)!;
                            var downPath = i.FindPathTo(Vector2I.Down)!;
                            upPath.SetMazeNode(mazeNode.Up);
                            downPath.SetMazeNode(mazeNode);

                            if (upLowerZone) {
                                // up is the exit to this node, so the new intersection belongs to the up node
                                i.SetMazeNode(mazeNode.Up);
                                var cityExit = new CityExit(i, mazeNode.Up, mazeNode) {
                                    FromPath = upPath,
                                    DestinationPath = downPath
                                };
                                Exits.Add(cityExit);
                            } else {
                                // This node is the exit, so the new intersection belongs to this node
                                i.SetMazeNode(mazeNode);
                                var cityExit = new CityExit(i, mazeNode, mazeNode.Up) {
                                    FromPath = downPath,
                                    DestinationPath = upPath
                                };
                                Exits.Add(cityExit);
                            }
                        } else {
                            // MazeNodes belongs to the same zone, keep all the connections
                        }
                    } else {
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
                            var leftLowerZone = mazeNode.Left.ZoneId < mazeNode.ZoneId;
                            var direction = leftLowerZone ? Vector2I.Right : Vector2I.Left;
                            if (leftLowerZone) {
                                // The other node has the exit to this entry node, so move the line outside
                                // because the intersection should belong to the exit node
                                lineStart += Vector2I.Left;
                                lineEnd += Vector2I.Left;
                            }
                            var i = CreateIntersectionsOnLine(lineStart, lineEnd, direction);
                            ignored.Add(i);

                            // Find the paths connecting to this intersection
                            var leftPath = i.FindPathTo(Vector2I.Left)!;
                            var rightPath = i.FindPathTo(Vector2I.Right)!;
                            leftPath.SetMazeNode(mazeNode.Left);
                            rightPath.SetMazeNode(mazeNode);

                            if (leftLowerZone) {
                                // left is the exit to this node, so the new intersection belongs to the up node
                                i.SetMazeNode(mazeNode.Left);
                                var cityExit = new CityExit(i, mazeNode.Left, mazeNode) {
                                    FromPath = leftPath,
                                    DestinationPath = rightPath
                                };
                                Exits.Add(cityExit);
                            } else {
                                // This node is the exit, so the new intersection belongs to this node
                                i.SetMazeNode(mazeNode);
                                var cityExit = new CityExit(i, mazeNode, mazeNode.Left) {
                                    FromPath = rightPath,
                                    DestinationPath = leftPath
                                };
                                Exits.Add(cityExit);
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

        var crossingPaths = new HashSet<Path>();
        foreach (var ((gridX, gridY), sectionRect) in City.Data.GetRects(SectionSize, SectionSize)) {
            var mazeNode = NodesGrid[gridY, gridX];

            // Sections with no maze, remove all paths
            if (mazeNode != null) {
                foreach (var path in City.FindPathsInSection(sectionRect).ToList()) {
                    var node = path.Start.GetMazeNode();
                    var nodeEnd = path.End!.GetMazeNode();
                    if (node == null || nodeEnd == null) {
                        throw new Exception("Bug: all intersections should have a mazeNode");
                    }
                    if (node == nodeEnd) {
                        path.SetMazeNode(node);
                        node.AddPath(path);
                    } else {
                        crossingPaths.Add(path);
                        // Console.WriteLine("Crossing path start: "+path.Start.Position+" to "+path.End.Position);
                    }
                }
            }
        }
        CrossingPaths.Clear();
        CrossingPaths.AddRange(crossingPaths);

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
        // It removes all the crossing path and keep only one, creating a new intersection.
        Intersection CreateIntersectionsOnLine(Vector2I lineStart, Vector2I lineEnd, Vector2I direction) {
            var isHorizontal = lineStart.Y == lineEnd.Y;
            // Create intersections on paths crossing the line
            var crossingPaths = new List<(Path Path, Vector2I Position)>();
            Draw.Line(lineStart, lineEnd, 1, (x, y) => {
                var pos = new Vector2I(x, y);
                var tile = City.Data[pos];
                if (tile is Path path) {
                    // Check if this path crosses the line
                    var pathCrossesLine = (isHorizontal && (path.Direction == Vector2I.Up || path.Direction == Vector2I.Down)) ||
                                          (!isHorizontal && (path.Direction == Vector2I.Left || path.Direction == Vector2I.Right));
                    if (pathCrossesLine) {
                        crossingPaths.Add((Path: path, Position: pos));
                    }
                } else if (tile is Intersection intersection) {
                    var exitPath = intersection.FindPathTo(direction);
                    if (exitPath != null) {
                        // It's an intersection fork with a crossing line and an extra path
                        crossingPaths.Add((Path: exitPath, Position: pos));
                    }
                } 
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

            var tile = City.Data[candidate.Position];
            if (tile is Intersection intersection) {
                return intersection;
            }
            intersection = City.SplitPath(candidate.Path, candidate.Position);
            return intersection;
        }
    }

    public bool Validate() {
        foreach (var (pos, rect) in City.Data.GetRects(SectionSize, SectionSize)) {
            var pathsInSection = City.FindPathsInSection(rect).ToList();
            if (NodesGrid[pos] != null && pathsInSection.Count == 0) {
                return false;
            } else if (NodesGrid[pos] == null && pathsInSection.Count > 0) {
                throw new Exception($"Please, don't create paths in sections without node in the first place. User {nameof(AddLimits)} before");
            }
        }
        return true;
    }
}