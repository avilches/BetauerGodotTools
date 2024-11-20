using System.Collections.Generic;

namespace Betauer.Core.PCG.Graph;

public class UnionFind {
    private int[] parent;
    private int[] size;
    private int numSets;

    public UnionFind(int n) {
        parent = new int[n];
        size = new int[n];
        numSets = n;

        for (int i = 0; i < n; i++) {
            parent[i] = i; // Inicialmente, cada región es su propio padre
            size[i] = 1; // Cada conjunto comienza con tamaño 1
        }
    }

    // Encuentra el líder del conjunto aplicando compresión de camino
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

    // Devuelve todas las regiones conectadas a la región especificada
    public List<int> GetConnectedRegions(int x) {
        int root = Find(x);
        var connectedRegions = new List<int>();

        for (int i = 0; i < parent.Length; i++) {
            if (Find(i) == root) {
                connectedRegions.Add(i);
            }
        }
        return connectedRegions;
    }

    // Devuelve todos los conjuntos completos
    public List<List<int>> GetFinalRegions() {
        var regions = new Dictionary<int, List<int>>();

        for (int i = 0; i < parent.Length; i++) {
            int root = Find(i);
            if (!regions.ContainsKey(root)) {
                regions[root] = new List<int>();
            }
            regions[root].Add(i);
        }

        // Convertir a una lista de listas para mantener la estructura de array
        var finalRegions = new List<List<int>>();
        foreach (var region in regions.Values) {
            finalRegions.Add(region);
        }

        return finalRegions;
    }

    // Verifica si todas las regiones están conectadas en un único conjunto
    public bool AreAllConnected() {
        if (parent.Length == 0) return true; // Si no hay regiones, están "conectadas"

        int root = Find(0); // Tomamos el líder de la primera región
        for (int i = 1; i < parent.Length; i++) {
            if (Find(i) != root) {
                return false; // Si encontramos una región con un líder diferente, no están todas conectadas
            }
        }
        return true;
    }
}

