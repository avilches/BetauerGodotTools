using System;
using Betauer.Core.Image;
using Godot;

namespace Betauer.TileSet.Image;

public class CellImage {
    public FastImage FastImage { get; private set; }
    public int Columns { get; }
    public int Rows { get; }
    public int ImageWidth { get; }
    public int ImageHeight { get; }
    public int CellSize { get; }

    /// <summary>
    /// Create an empty image with the given size
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="cellSize"></param>
    public CellImage(int columns, int rows, int cellSize) {
        Columns = columns;
        Rows = rows;
        CellSize = cellSize;
        ImageWidth = columns * cellSize;
        ImageHeight = rows * cellSize;
        FastImage = new FastImage(ImageWidth, ImageHeight);
    }

    /// <summary>
    /// Load an image and get the columns and rows from the cellSize (ensure that the image size is a multiple of cellSize)
    /// </summary>
    /// <param name="fastImage"></param>
    /// <param name="cellSize"></param>
    /// <exception cref="Exception"></exception>
    public CellImage(FastImage fastImage, int cellSize) {
        if (fastImage.Width % cellSize != 0 || fastImage.Height % cellSize != 0) {
            throw new Exception($"The image size ({fastImage.Width},{fastImage.Height}) must be a multiple of cell size {cellSize}");
        }  
        Columns = fastImage.Width / cellSize;
        Rows = fastImage.Height / cellSize;
        CellSize = cellSize;
        ImageWidth = fastImage.Width;
        ImageHeight = fastImage.Height;
        FastImage = fastImage;
    }
    
    /// <summary>
    /// Load and image and get the cellSize from the rows and columns (ensure that the image size is a multiple of rows and columns)
    /// </summary>
    /// <param name="fastImage"></param>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    /// <exception cref="Exception"></exception>
    public CellImage(FastImage fastImage, int rows, int columns) {
        if (fastImage.Width % columns != 0) {
            throw new Exception($"The image width {fastImage.Width} must be a multiple of columns {columns}");
        }  
        if (fastImage.Height % rows != 0) {
            throw new Exception($"The image height {fastImage.Height} must be a multiple of rows {rows}");
        }
        var cellWidth = fastImage.Width / columns;
        var cellHeight = fastImage.Height / rows;
        if (cellWidth != cellHeight) {
            throw new Exception($"The cell size ({cellWidth},{cellHeight}) must be a square (same width and height)");
        }
        Columns = columns;
        Rows = rows;
        CellSize = cellWidth;
        ImageWidth = fastImage.Width;
        ImageHeight = fastImage.Height;
        FastImage = fastImage;
    }

    /// <summary>
    /// Change the overlying image 
    /// </summary>
    /// <param name="fastImage"></param>
    /// <exception cref="Exception"></exception>
    public void SetImage(FastImage fastImage) {
        if (fastImage.Width % Columns != 0) {
            throw new Exception($"The image width {fastImage.Width} must be a multiple of columns {Columns}");
        }  
        if (fastImage.Height % Rows != 0) {
            throw new Exception($"The image height {fastImage.Height} must be a multiple of rows {Rows}");
        }
        var cellWidth = fastImage.Width / Columns;
        var cellHeight = fastImage.Height / Rows;
        if (cellWidth != cellHeight) {
            throw new Exception($"The cell size ({cellWidth},{cellHeight}) must be a square (same width and height)");
        }
        FastImage = fastImage;
    }
    
    public Color[,] Copy(int cellX, int cellY) {
        return FastImage.GetRegion(cellX * CellSize, cellY * CellSize, CellSize, CellSize);
    }

    public Color[,] Copy(int cellX, int cellY, int x, int y, int width, int height) {
        return FastImage.GetRegion(cellX * CellSize + x, cellY * CellSize + y, width, height);
    }

    public void Paste(Color[,] cell, int cellX, int cellY, bool blend = true) {
        FastImage.SetRegion(cell, cellX * CellSize, cellY * CellSize, width: CellSize, height: CellSize, blend: blend);
    }

    public void PastePart(Color[,] cell, int cellX, int cellY, int x, int y, bool blend = true) {
        FastImage.SetRegion(cell, cellX * CellSize + x, cellY * CellSize + y, -1, -1, blend);
    }

    public void SavePng(string filename) {
        FastImage.Flush();
        FastImage.Image.SavePng(filename);
    }

    public byte[] GetPng() {
        FastImage.Flush();
        return FastImage.Image.SavePngToBuffer();
    }

}