using System;
using Betauer.Core.Image;
using Godot;

namespace Betauer.TileSet.Image;

public class CellImage {
    public FastImage FastImage { get; private set; }
    public int Width { get; }
    public int Height { get; }
    public int ImageWidth { get; }
    public int ImageHeight { get; }
    public int CellSize { get; }

    /// <summary>
    /// Create an empty image with the given size
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cellSize"></param>
    public CellImage(int width, int height, int cellSize, global::Godot.Image.Format? format = null) {
        Width = width;
        Height = height;
        CellSize = cellSize;
        ImageWidth = width * cellSize;
        ImageHeight = height * cellSize;
        FastImage = new FastImage(ImageWidth, ImageHeight, false, format);
    }

    public CellImage(string resource, int cellSize, global::Godot.Image.Format? format = null) : this(new FastImage(resource, format), cellSize) {
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
        Width = fastImage.Width / cellSize;
        Height = fastImage.Height / cellSize;
        CellSize = cellSize;
        ImageWidth = fastImage.Width;
        ImageHeight = fastImage.Height;
        FastImage = fastImage;
    }
    
    public CellImage(string resource, int width, int height, global::Godot.Image.Format? format = null) : this(new FastImage(resource, format), width, height) {
    }

    /// <summary>
    /// Load and image and get the cellSize from the rows and columns (ensure that the image size is a multiple of rows and columns)
    /// </summary>
    /// <param name="fastImage"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="Exception"></exception>
    public CellImage(FastImage fastImage, int width, int height) {
        if (fastImage.Width % width != 0) {
            throw new Exception($"The image width {fastImage.Width} must be a multiple of columns {width}");
        }  
        if (fastImage.Height % height != 0) {
            throw new Exception($"The image height {fastImage.Height} must be a multiple of rows {height}");
        }
        var cellWidth = fastImage.Width / width;
        var cellHeight = fastImage.Height / height;
        if (cellWidth != cellHeight) {
            throw new Exception($"The cell size ({cellWidth},{cellHeight}) must be a square (same width and height)");
        }
        Width = width;
        Height = height;
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
        if (fastImage.Width % Width != 0) {
            throw new Exception($"The image width {fastImage.Width} must be a multiple of columns {Width}");
        }  
        if (fastImage.Height % Height != 0) {
            throw new Exception($"The image height {fastImage.Height} must be a multiple of rows {Height}");
        }
        var cellWidth = fastImage.Width / Width;
        var cellHeight = fastImage.Height / Height;
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