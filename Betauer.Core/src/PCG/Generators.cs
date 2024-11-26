using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG;

public static class Generators {
    /// <summary>
    /// Eden Growth Model (DLA sin movimiento browniano)
    /// Características: Genera formas orgánicas con ramificaciones fractales
    /// Aspecto: Parece un coral o un cristal creciendo
    ///  ·························#························
    /// ·························#························
    /// ························##·#··#···················
    /// ·······················##########·················
    /// ·······················###########················
    /// ···················#·#·###########················
    /// ···················################···············
    /// ··················################················
    /// ··················##############··················
    /// ···················#############··················
    /// ·····················######···#···················
    /// ····················########······················
    /// ·····················########·····················
    /// ····················########······················
    /// ···················##########·····················
    /// ···················####··####·····················
    /// ···················###··####······················
    /// ···················##····####·····················
    /// </summary>
    public static void EdenGrowth(
        Array2D<bool> region,
        Vector2I start,
        int numPixels,
        double branchingProbability = 0.7,
        Random? random = null) {
        if (numPixels == 0) return;
        if (numPixels == 1) {
            region[start] = true;
            return;
        }
        random ??= new Random();
        var active = new List<Vector2I>();
        var emptyNeighbors = new HashSet<Vector2I>(); // Mantener track de celdas vacías adyacentes

        region[start] = true;
        active.Add(start);
        // Añadir vecinos vacíos iniciales
        emptyNeighbors.UnionWith(region.GetValidUpDownLeftRightPositions(start, b => !b));
        var pixelsAdded = 1;

        while (pixelsAdded < numPixels && (active.Count > 0 || emptyNeighbors.Count > 0)) {
            if (active.Count > 0) {
                var activeIndex = random.Next(active.Count);
                var cell = active[activeIndex];
                var addedNeighbor = false;

                // Intentar expandirse normalmente
                foreach (var pos in region.GetValidUpDownLeftRightPositions(cell, b => !b)
                             .OrderBy(_ => random.Next())) {
                    if (random.NextDouble() < branchingProbability) {
                        addedNeighbor = true;
                        region[pos] = true;
                        active.Add(pos);
                        emptyNeighbors.Remove(pos);
                        // Añadir nuevos vecinos vacíos
                        foreach (var newEmpty in region.GetValidUpDownLeftRightPositions(pos, b => !b)) {
                            emptyNeighbors.Add(newEmpty);
                        }
                        pixelsAdded++;
                        if (pixelsAdded >= numPixels) break;
                    }
                }

                if (!addedNeighbor) {
                    active.RemoveAt(activeIndex);
                }
            } else if (emptyNeighbors.Count > 0 && pixelsAdded < numPixels) {
                // Llenar huecos cuando no hay células activas
                var emptyCell = emptyNeighbors.First();
                region[emptyCell] = true;
                active.Add(emptyCell);
                emptyNeighbors.Remove(emptyCell);
                // Añadir nuevos vecinos vacíos
                foreach (var newEmpty in region.GetValidUpDownLeftRightPositions(emptyCell, b => !b)) {
                    emptyNeighbors.Add(newEmpty);
                }
                pixelsAdded++;
            }
        }

        // Fase final: llenar huecos restantes si aún no alcanzamos numPixels
        if (pixelsAdded < numPixels) {
            foreach (var pos in region.GetPositions()) {
                if (!region[pos.Position] && HasFilledNeighbor(region, pos.Position)) {
                    region[pos.Position] = true;
                    pixelsAdded++;
                    if (pixelsAdded >= numPixels) break;
                }
            }
        }
    }

    private static bool HasFilledNeighbor(Array2D<bool> region, Vector2I pos) {
        return region.GetValidUpDownLeftRightPositions(pos, b => b).Any();
    }

    /// <summary>
    /// Percolation Cluster
    /// Características: Genera formas con huecos internos y bordes irregulares
    /// Aspecto: Similar a una mancha de tinta o líquido percolando
    /// ··················································
    /// ··················································
    /// ·····················#····#·······················
    /// ·····················#·####·······················
    /// ····················######························
    /// ···············####·######························
    /// ·········#······#######·##························
    /// ·······###·······####·####·###····················
    /// ········##·······####··#######····················
    /// ········#####······###########····················
    /// ··········####·#####··####·######·················
    /// ···········##########·##########··················
    /// ············####################··················
    /// ············##······##########····················
    /// ····················########······················
    /// ·····················#··####······················
    /// ····················###····#······················
    /// ··················································
    /// </summary>
    public static void PercolationCluster(
        Array2D<bool> region,
        Vector2I start,
        int numPixels,
        double stickingProbability = 0.3,
        double smoothingFactor = 0.5,
        Random? random = null) {
        if (numPixels == 0) return;
        if (numPixels == 1) {
            region[start] = true;
            return;
        }
        random ??= new Random();
        var candidates = new HashSet<Vector2I>();
        var filled = new HashSet<Vector2I>();
        var active = new HashSet<Vector2I>(); // Puntos de crecimiento activos

        // Punto semilla
        region[start] = true;
        filled.Add(start);
        active.Add(start); // El punto inicial es un punto activo

        candidates.UnionWith(region.GetValidUpDownLeftRightPositions(start));

        // This is to ensure the algorithm doesn't run out of cells until reach to the desired number of pixels
        var min = numPixels / 4 + 1;
        var pixelsAdded = 1;
        while (pixelsAdded < numPixels && (candidates.Count > 0 || active.Count >= min)) {
            if (candidates.Count == 0) {
                // Si no hay candidatos pero tenemos puntos activos, generamos nuevos candidatos
                candidates.UnionWith(active
                    .SelectMany(p => region.GetValidUpDownLeftRightPositions(p, b => !b))
                    .Where(pos => !filled.Contains(pos)));
                if (candidates.Count == 0) break; // Si aún no hay candidatos, terminamos
            }

            var candidateArray = candidates.ToArray();
            var idx = random.Next(candidateArray.Length);
            var cell = candidateArray[idx];
            candidates.Remove(cell);
            var neighbors = region.GetValidUpDownLeftRightPositions(cell, b => b).Count();
            var prob = stickingProbability + (neighbors * smoothingFactor / 8.0);
            if (random.NextDouble() < prob) {
                region[cell] = true;
                pixelsAdded++;
                filled.Add(cell);
                active.Add(cell);

                candidates.UnionWith(region
                    .GetValidUpDownLeftRightPositions(cell, b => !b)
                    .Where(pos => !filled.Contains(pos)));
            } else if (active.Count < min) {
                // Si tenemos pocos puntos activos, devolvemos el candidato a la lista
                candidates.Add(cell);
            }
        }
    }

    /// <summary>
    /// Diffusion Limited Aggregation (DLA)
    /// Características: Genera estructuras fractales ramificadas
    /// Aspecto: Similar a un rayo o descarga eléctrica
    /// ···························#······················
    /// ·······················#··#·······················
    /// ························##························
    /// ··························#·······················
    /// ··························#·#·····················
    /// ·························#·#······················
    /// ·····················#··#···##····················
    /// ····················#·#·##··####··················
    /// ·······················#······#···················
    /// ······················#·#······#·#················
    /// ····················#··#·#····#·#·#···············
    /// ····················#·#··#····#··#················
    /// ·····················###··#·······················
    /// ····················#··##··#······················
    /// ·························#··#·····················
    /// ························#·························
    /// ·······················##·························
    /// ····················#·#··#························
    /// ·····················###··#·······················
    /// Incrementar el numero de pixeles genera una figura igual, pero con una celda mas
    /// </summary>
    public static void DiffusionLimitedAggregation(
        Array2D<bool> region,
        Vector2I start,
        int numPixels,
        int particleSpeed = 1,
        double stickinessRadius = 1.5,
        Random? random = null) {
        if (numPixels == 0) return;
        if (numPixels == 1) {
            region[start] = true;
            return;
        }

        random ??= new Random();
        var active = new HashSet<Vector2I>();

        // Punto semilla
        region[start] = true;
        active.Add(start);
        var pixelsAdded = 1;

        // Esto asegura que el algoritmo no se quede sin células hasta llegar al número deseado de píxeles
        var min = numPixels / 4 + 1;

        // Pre-calcular el radio al cuadrado para evitar raíz cuadrada en las comparaciones
        var radiusSquared = stickinessRadius * stickinessRadius;
        // Pre-calcular el rango de búsqueda
        var radiusCeil = (int)Math.Ceiling(stickinessRadius);
        var radiusRange = Enumerable.Range(-radiusCeil, 2 * radiusCeil + 1).ToArray();

        while (pixelsAdded < numPixels && active.Count > 0) {
            // Generar partícula en el borde usando random.Next una sola vez
            Vector2I particle;
            var height = region.Height;
            var width = region.Width;
            if (random.Next(2) == 0) {
                // Partícula en borde superior o inferior
                particle = new Vector2I(
                    random.Next(width), // X
                    random.Next(2) * (height - 1) // Y
                );
            } else {
                // Partícula en borde izquierdo o derecho
                particle = new Vector2I(
                    random.Next(2) * (width - 1), // X
                    random.Next(height) // Y
                );
            }

            bool stuck = false;
            while (!stuck) {
                // Mover la partícula varios pasos
                for (int step = 0; step < particleSpeed; step++) {
                    particle = new Vector2I(
                        Math.Clamp(particle.X + random.Next(3) - 1, 0, width - 1), // X
                        Math.Clamp(particle.Y + random.Next(3) - 1, 0, height - 1) // Y
                    );
                }

                var hasNeighbor = false;
                foreach (var dy in radiusRange) {
                    if (hasNeighbor) break;
                    var newY = particle.Y + dy;
                    if (newY < 0 || newY >= height) continue;

                    foreach (var dx in radiusRange) {
                        var newX = particle.X + dx;
                        if (newX < 0 || newX >= width) continue;

                        var offset = new Vector2I(dx, dy);
                        if (offset.LengthSquared() > radiusSquared) continue;

                        if (region[newY, newX]) {
                            hasNeighbor = true;
                            break;
                        }
                    }
                }

                if (hasNeighbor) {
                    region[particle.Y, particle.X] = true; // Acceso al array con [Y,X]
                    active.Add(particle);
                    pixelsAdded++;
                    stuck = true;

                    // Verificar si algún punto activo ya no tiene espacio para crecer
                    if (active.Count > min) {
                        var pointsToRemove = new List<Vector2I>();
                        foreach (var point in active) {
                            if (region.GetValidUpDownLeftRightPositions(point, b => !b).Count() == 0) {
                                pointsToRemove.Add(point);
                            }
                        }
                        foreach (var point in pointsToRemove) {
                            active.Remove(point);
                        }
                    }
                }

                // Si la partícula llega al borde, terminamos con ella
                if (particle.X == 0 || particle.X == width - 1 ||
                    particle.Y == 0 || particle.Y == height - 1) {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Metaball/Blob
    /// Características: Genera formas orgánicas suaves y redondeadas
    /// Aspecto: Como gotas de líquido o células fusionándose
    /// </summary>
    public static void Metaball(
        Array2D<bool> region,
        Vector2I start,
        int numPixels,
        int numBlobs = 5,
        double threshold = 1.0,
        double minRadius = 10,
        double maxRadius = 25,
        Random? random = null) {
        if (numPixels == 0) return;
        if (numPixels == 1) {
            region[start] = true;
            return;
        }

        random ??= new Random();
        var blobs = new List<(Vector2I pos, double radius)>();

        // Genera 3 puntos aleatorios dentro del radio pr (pr = numPixels / 3 como radio inicial sugerido)
        var pr = numPixels / 3.0;
        for (int i = 0; i < numBlobs; i++) {
            var angle = random.NextDouble() * Math.PI * 2;
            var distance = random.NextDouble() * pr;
            var x = (int)(start.X + Math.Cos(angle) * distance);
            var y = (int)(start.Y + Math.Sin(angle) * distance);
            var pos = new Vector2I(
                Math.Clamp(x, 0, region.Width - 1),
                Math.Clamp(y, 0, region.Height - 1)
            );

            double radius = random.NextDouble() * (maxRadius - minRadius) + minRadius;
            blobs.Add((pos, radius));
        }

        // Calcular campo de influencia usando LINQ
        var positions = region.GetPositions().Select(cell => cell.Position)
            .Where(pos => blobs.Sum(blob => {
                var distance = (pos - blob.pos).Length();
                return Math.Pow(blob.radius / distance, 2);
            }) >= threshold);

        foreach (var pos in positions) {
            region[pos] = true;
        }
    }
}