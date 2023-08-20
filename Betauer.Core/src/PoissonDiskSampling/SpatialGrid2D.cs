using System;
using System.Collections.Generic;
using Betauer.Core.PoissonDiskSampling.Utils;
using Godot;

namespace Betauer.Core.PoissonDiskSampling;

/// <summary>
/// A basic 2D grid of items for simple spatial queries.
/// </summary>
public sealed class SpatialGrid2D<T> {
    /// <summary>
    /// The width of the grid.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// The height of the grid.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// The dimensions of an individual cell of the grid.
    /// </summary>
    public float CellLength { get; }

    /// <summary>
    /// Number of cells in the x-dimension of the grid.
    /// </summary>
    public int CellsPerX { get; }

    /// <summary>
    /// Number of cells in the y-dimension of the grid.
    /// </summary>
    public int CellsPerY { get; }

    /// <summary>
    /// The grid cells.
    /// </summary>
    private readonly List<SpatialCell> _gridCells;

    /// <summary>
    /// The items in the grid.
    /// </summary>
    private readonly List<T> _gridItems;

    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    public SpatialGrid2D(float width, float height, float minRadius, float maxRadius) {
        Width = width;
        Height = height;
        CellLength = ((minRadius + maxRadius) * 0.5f) / Mathf.Sqrt2;
        CellsPerX = (int)Math.Ceiling(Width / CellLength);
        CellsPerY = (int)Math.Ceiling(Height / CellLength);

        var totalCells = CellsPerX * CellsPerY;

        _gridCells = new List<SpatialCell>(totalCells);
        _gridItems = new List<T>(totalCells);

        for (var y = 0; y < CellsPerY; ++y) {
            for (var x = 0; x < CellsPerX; ++x) {
                _gridCells.Add(new SpatialCell(x, y, CellLength));
            }
        }
    }

    // ---------------------------------------------------------------------------------
    // Adding
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Adds the item at the given position to the list. Does not perform clearance checking.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="radius"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Add(T item, float x, float y, float radius) {
        var index = GetIndex(x, y);

        if (index == -1) {
            // Out of bounds.
            return false;
        }

        Add(item, index, x, y, radius);

        return true;
    }

    /// <summary>
    /// Adds the item at the specified position only if there are no intersections with other items in the grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public bool AddIfOpen(T item, float x, float y, float radius) {
        var index = GetIndex(x, y);

        if (!IsOpen(index, x, y, radius)) {
            return false;
        }

        Add(item, index, x, y, radius);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    private void Add(T item, int cellIndex, float x, float y, float radius) {
        var cellRadius = (int)Math.Ceiling(radius / CellLength);

        var index = _gridItems.Count;
        _gridItems.Add(item);

        SpatialGridItem spatialItem = new SpatialGridItem() {
            Index = index,
            X = x,
            Y = y,
            Radius = radius
        };

        // Add the item to any cells it intersects.
        for (var iy = -cellRadius; iy <= cellRadius; ++iy) {
            for (var ix = -cellRadius; ix <= cellRadius; ++ix) {
                int neighbor = cellIndex + ix + (iy * CellsPerX);

                if ((neighbor < 0) || (neighbor >= _gridCells.Count)) {
                    continue;
                }

                if (_gridCells[neighbor].IntersectsCell(x, y, radius)) {
                    _gridCells[neighbor].Contents.Add(spatialItem);
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------
    // Removing
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Removes the specified item from the grid, if it is present.
    /// </summary>
    /// <param name="t"></param>
    public void Remove(T t) {
        var index = 0;

        // Remove the direct reference to the item
        for (; index < _gridItems.Count; ++index) {
            if (Comparer<T>.Default.Compare(_gridItems[index], t) == 0) {
                _gridItems.RemoveUnorderedAt(index);
                break;
            }
        }

        // Remove the index reference that may be in any cells
        for (var i = 0; i < _gridCells.Count; ++i) {
            SpatialCell cell = _gridCells[i];

            for (int j = (cell.Contents.Count - 1); j >= 0; --j) {
                if (cell.Contents[j].Index == index) {
                    cell.Contents.RemoveUnorderedAt(j);
                }
            }
        }
    }

    /// <summary>
    /// Clears away all items in the grid.
    /// </summary>
    public void Clear() {
        _gridItems.Clear();

        foreach (var cell in _gridCells) {
            cell.Contents.Clear();
        }
    }


    // ---------------------------------------------------------------------------------
    // Querying
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Returns true if the radius around the specified position is open.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public bool IsOpen(float x, float y, float radius) {
        int centerIndex = GetIndex(x, y);
        return IsOpen(centerIndex, x, y, radius);
    }

    /// <summary>
    /// Returns true if the radius around the specified position is open.
    /// </summary>
    /// <param name="centerIndex"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool IsOpen(int centerIndex, float x, float y, float radius) {
        if (centerIndex == -1) {
            // Out of bounds.
            return false;
        }

        int cellRadius = (int)Math.Ceiling(radius / CellLength);

        // Check if the circle is too close to any in the prospective cell and the neighboring cells.
        for (int iy = -cellRadius; iy <= cellRadius; ++iy) {
            for (int ix = -cellRadius; ix <= cellRadius; ++ix) {
                int neighbor = centerIndex + ix + (iy * CellsPerX);

                if (neighbor < 0 || (neighbor >= _gridCells.Count)) {
                    continue;
                }

                if (_gridCells[neighbor].IntersectsCell(x, y, radius) &&
                    _gridCells[neighbor].IntersectsChild(x, y, radius)) {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Retrieves the index of the cell that contains the specified point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetIndex(float x, float y) {
        if ((x < 0.0f) || (x > Width) || (y < 0.0f) || (y > Height)) {
            return -1;
        }

        int dx = (int)(x / CellLength);
        int dy = (int)(y / CellLength);

        return (dx + (dy * CellsPerX));
    }

    /// <summary>
    /// Gets all items in the radius around the specified point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<T> GetAt(float x, float y, float radius) {
        List<T> result = new List<T>();
        int cellIndex = GetIndex(x, y);

        if (cellIndex != -1) {
            List<SpatialGridItem> items = _gridCells[cellIndex].GetIntersections(x, y, radius);

            for (int i = 0; i < items.Count; ++i) {
                result.Add(_gridItems[items[i].Index]);
            }
        }

        return result;
    }

    // ---------------------------------------------------------------------------------
    // Inner Class - SpatialCell
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// An individual cell in the grid.
    /// </summary>
    private sealed class SpatialCell {
        public int OffsetX;
        public int OffsetY;

        public float OriginX;
        public float OriginY;
        public float Dimension;

        public List<SpatialGridItem> Contents;

        public SpatialCell(int x, int y, float dimension) {
            OffsetX = x;
            OffsetY = y;
            OriginX = ((float)OffsetX * dimension);
            OriginY = ((float)OffsetY * dimension);
            Dimension = dimension;
            Contents = new List<SpatialGridItem>();
        }

        /// <summary>
        /// Does this cell contain the specified point?
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool ContainsPoint(float x, float y) {
            return (x >= OriginX) && (x < (OriginX + Dimension)) &&
                   (y >= OriginY) && (y < (OriginY + Dimension));
        }

        /// <summary>
        /// Does the given circle intersect this cell?
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public bool IntersectsCell(float x, float y, float radius) {
            return BoxCircle(OriginX, OriginX + Dimension, OriginY, OriginY + Dimension, x, y, radius);
        }

        /// <summary>
        /// Does the given circle intersect an item contained in this cell?
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public bool IntersectsChild(float x, float y, float radius) {
            for (int i = 0; i < Contents.Count; ++i) {
                if (DistanceSq(x, y, Contents[i].X, Contents[i].Y) < (radius * radius)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all children that intersect with the given circle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public List<SpatialGridItem> GetIntersections(float x, float y, float radius) {
            List<SpatialGridItem> intersections = new List<SpatialGridItem>(Contents.Count);

            for (int i = 0; i < Contents.Count; ++i) {
                if (DistanceSq(x, y, Contents[i].X, Contents[i].Y) < (radius * radius)) {
                    intersections.Add(Contents[i]);
                }
            }

            return intersections;
        }

        /// <summary>
        /// Equivalent to (bx - ax)^2 + (by - ay)^2.
        /// in Godot new Vector2(ax, ay).DistanceSquaredTo(new Vector2(bx, by))
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static float DistanceSq(float ax, float ay, float bx, float by) {
            float dx = (bx - ax);
            float dy = (by - ay);
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Returns true if the circle intersects the box.
        /// </summary>
        /// <param name="boxMinX"></param>
        /// <param name="boxMaxX"></param>
        /// <param name="boxMinY"></param>
        /// <param name="boxMaxY"></param>
        /// <param name="circleX"></param>
        /// <param name="circleY"></param>
        /// <param name="circleRadius"></param>
        /// <returns></returns>
        public static bool BoxCircle(float boxMinX, float boxMaxX, float boxMinY, float boxMaxY, float circleX, float circleY, float circleRadius) {
            float closestX = Mathf.Clamp(circleX, boxMinX, boxMaxX);
            float closestY = Mathf.Clamp(circleY, boxMinY, boxMaxY);

            float distanceX = (circleX - closestX);
            float distanceY = (circleY - closestY);

            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

            return (distanceSquared < (circleRadius * circleRadius));
        }
    }

    // ---------------------------------------------------------------------------------
    // Inner Class - SpatialGridItem
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Tracks a single item in the grid.
    /// </summary>
    private sealed class SpatialGridItem {
        /// <summary>
        /// Index of the item in <see cref="SpatialGrid2D{T}._gridItems"/>.
        /// </summary>
        public int Index;

        public float X;
        public float Y;
        public float Radius;
    }
}