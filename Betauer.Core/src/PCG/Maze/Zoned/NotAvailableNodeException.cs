using System;

namespace Betauer.Core.PCG.Maze.Zoned;

public class NotAvailableNodeException(string message) : Exception(message);