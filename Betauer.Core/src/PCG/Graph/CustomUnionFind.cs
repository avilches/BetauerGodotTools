using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Graph;

public class CustomUnionFind {
    private Dictionary<int, int> parent = new Dictionary<int, int>();
    private Dictionary<int, int> size = new Dictionary<int, int>();
    private int numSets;

    // Método para añadir una región
    public void AddRegion(int x) {
        if (parent.TryAdd(x, x)) {
            size[x] = 1;
            numSets++;
        }
    }

    // Encuentra el líder del conjunto
    public int Find(int x) {
        if (parent[x] != x) {
            parent[x] = Find(parent[x]);
        }
        return parent[x];
    }

    // Une dos regiones y actualiza el tamaño y número de conjuntos
    public void Union(int x, int y) {
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX != rootY) {
            if (rootX < rootY) {
                parent[rootY] = rootX;
                size[rootX] += size[rootY];
            } else {
                parent[rootX] = rootY;
                size[rootY] += size[rootX];
            }
            numSets--; // Reducimos el número de conjuntos
        }
    }

    // Devuelve el número de conjuntos independientes
    public int GetNumSets() {
        return numSets;
    }

    // Devuelve el tamaño del conjunto de una región específica
    public int GetSetSize(int x) {
        return size[Find(x)];
    }

    // Devuelve los conjuntos completos
    public Dictionary<int, List<int>> GetFinalRegions() {
        var regions = new Dictionary<int, List<int>>();
        foreach (var node in parent.Keys) {
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

        foreach (var node in parent.Keys) {
            if (Find(node) == root) {
                connectedRegions.Add(node);
            }
        }
        return connectedRegions;
    }

    public bool AreAllConnected() {
        if (parent.Count == 0) return true; // Si no hay regiones, están "conectadas"

        var root = Find(parent.Keys.First());
        foreach (var node in parent.Keys) {
            if (Find(node) != root) {
                return false; // Si encontramos una región con un líder diferente, no están todas conectadas
            }
        }
        return true;
    }
}