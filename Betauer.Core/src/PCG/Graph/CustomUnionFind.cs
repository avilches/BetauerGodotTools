using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Graph;

public class CustomUnionFind {
    private readonly Dictionary<int, int> _parent = [];
    private readonly Dictionary<int, int> _size = [];
    private int _numSets;

    // Método para añadir una región
    public void AddRegion(int x) {
        if (_parent.TryAdd(x, x)) {
            _size[x] = 1;
            _numSets++;
        }
    }

    // Encuentra el líder del conjunto
    public int Find(int x) {
        if (_parent[x] != x) {
            _parent[x] = Find(_parent[x]);
        }
        return _parent[x];
    }

    // Une dos regiones y actualiza el tamaño y número de conjuntos
    public void Union(int x, int y) {
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX != rootY) {
            if (rootX < rootY) {
                _parent[rootY] = rootX;
                _size[rootX] += _size[rootY];
            } else {
                _parent[rootX] = rootY;
                _size[rootY] += _size[rootX];
            }
            _numSets--; // Reducimos el número de conjuntos
        }
    }

    // Devuelve el número de conjuntos independientes
    public int GetNumSets() {
        return _numSets;
    }

    // Devuelve el tamaño del conjunto de una región específica
    public int GetSetSize(int x) {
        return _size[Find(x)];
    }

    // Devuelve los conjuntos completos
    public Dictionary<int, List<int>> GetFinalRegions() {
        var regions = new Dictionary<int, List<int>>();
        foreach (var node in _parent.Keys) {
            int root = Find(node);
            if (!regions.ContainsKey(root)) {
                regions[root] = new List<int>();
            }
            regions[root].Add(node);
        }
        return regions;
    }

    // Devuelve todas las regiones conectadas a la región especificada
    public List<int> GetConnectedRegions(int x) {
        int root = Find(x);
        var connectedRegions = new List<int>();

        foreach (var node in _parent.Keys) {
            if (Find(node) == root) {
                connectedRegions.Add(node);
            }
        }
        return connectedRegions;
    }

    public bool AreAllConnected() {
        if (_parent.Count == 0) return true; // Si no hay regiones, están "conectadas"

        var root = Find(_parent.Keys.First());
        foreach (var node in _parent.Keys) {
            if (Find(node) != root) {
                return false; // Si encontramos una región con un líder diferente, no están todas conectadas
            }
        }
        return true;
    }
}