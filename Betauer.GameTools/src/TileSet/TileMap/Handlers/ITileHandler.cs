using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;

namespace Betauer.TileSet.TileMap.Handlers;

public interface ITileHandler {
    public void Apply(TileMap tileMap, int x, int y);
}

/*
public class FilterHandler : ITileHandler  {
    private readonly ITileHandler _handler;
    private readonly int _type;

    public FilterHandler(ITileHandler handler) {
        _handler = handler;
    }

    public void Apply<T>(TileMap<T> tileMap, int x, int y) where T : Enum {
        if (_filter.All((f) => f(tileMap, x, y))
        _handler.Apply(tileMap, x, y);
    }

    public FilterHandler<TType> Filter(TType type) {
        var 
        _filter.Add((tileMap, x, y) => tileMap.GetType(x, y).ToInt() == type.ToInt());
        return this;
    }
}                        */