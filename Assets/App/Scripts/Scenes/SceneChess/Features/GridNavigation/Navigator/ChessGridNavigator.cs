using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private const int MaxCellsPerStep = 7;

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            return unit switch
            {
                ChessUnitType.Pon => FindPathPawn(from, to, grid),
                ChessUnitType.King => FindPathKing(from, to, grid),
                ChessUnitType.Queen => BfsAlgorithm(from, to, grid, GetQueenMoveDirections()),
                ChessUnitType.Rook => BfsAlgorithm(from, to, grid, GetRookMoveDirections()),
                ChessUnitType.Knight => BfsAlgorithm(from, to, grid, GetKnightMoves()),
                ChessUnitType.Bishop => BfsAlgorithm(from, to, grid, GetBishopMoveDirections()),
                _ => null
            };
        }

        private List<Vector2Int> FindPathPawn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (from.x != to.x || grid.Get(to) != null) return null;

            var stepsCount = Mathf.Abs(from.y - to.y);
            var currentPos = new Vector2Int(from.x, from.y);
            var movingUp = !(from.y > to.y);

            var path = new List<Vector2Int>();

            for (var i = 0; i < stepsCount; i++)
            {
                if (movingUp) currentPos.y++;
                else currentPos.y--;
                if (grid.Get(currentPos) != null) return null;
                path.Add(currentPos);
            }

            return path;
        }

        private List<Vector2Int> FindPathKing(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (grid.Get(to) != null) return null;

            var path = new List<Vector2Int>();
            var openSet = new List<PathNode>();
            var closedSet = new HashSet<Vector2Int>();

            openSet.Add(new PathNode(from, null, 0, CalculateHCost(from, to)));

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];

                for (var i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                if (currentNode.Position == to)
                {
                    while (currentNode != null)
                    {
                        path.Insert(0, currentNode.Position);
                        currentNode = currentNode.Parent;
                    }

                    return path;
                }

                foreach (var neighbor in GetKingNeighbors(currentNode.Position))
                {
                    if (!ValidatePosition(neighbor, grid) || closedSet.Contains(neighbor))
                        continue;

                    if (grid.Get(neighbor) != null) continue;

                    var tentativeGCost = currentNode.GCost + 1;
                    var neighborNode = openSet.Find(n => n.Position == neighbor);

                    if (neighborNode == null || tentativeGCost < neighborNode.GCost)
                    {
                        if (neighborNode == null)
                        {
                            neighborNode = new PathNode(neighbor, currentNode, tentativeGCost,
                                CalculateHCost(neighbor, to));
                            openSet.Add(neighborNode);
                        }
                        else
                        {
                            neighborNode.Parent = currentNode;
                            neighborNode.GCost = tentativeGCost;
                        }
                    }
                }
            }

            return null;
        }

        private bool ValidatePosition(Vector2Int position, ChessGrid grid) =>
            position.x is >= 0 and < 8 &&
            position.y is >= 0 and < 8 &&
            grid.Get(position) == null;

        private int CalculateHCost(Vector2Int from, Vector2Int to) =>
            Mathf.Max(Mathf.Abs(from.x - to.x), Mathf.Abs(from.y - to.y));

        private List<Vector2Int> GetKingNeighbors(Vector2Int position) => new()
        {
            new Vector2Int(position.x - 1, position.y - 1),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x - 1, position.y + 1),
            new Vector2Int(position.x, position.y - 1),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x + 1, position.y - 1),
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x + 1, position.y + 1)
        };

        private List<Vector2Int> BfsAlgorithm(Vector2Int from, Vector2Int to, ChessGrid grid,
            List<Vector2Int> directions)
        {
            var queue = new Queue<Vector2Int>();
            var parentMap = new Dictionary<Vector2Int, Vector2Int>();
            var visited = new HashSet<Vector2Int>();

            queue.Enqueue(from);
            visited.Add(from);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == to)
                {
                    {
                        var reconstructPath = ReconstructPath(parentMap, from, to);
                        return reconstructPath;
                    }
                }

                foreach (var direction in directions)
                {
                    for (var i = 1; i <= MaxCellsPerStep; i++)
                    {
                        var neighbor = current + direction * i;

                        if (!ValidatePosition(neighbor, grid) || visited.Contains(neighbor))
                        {
                            break;
                        }

                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        parentMap[neighbor] = current;
                    }
                }
            }

            return null;
        }

        private List<Vector2Int> GetQueenMoveDirections() => new()
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
        };

        private List<Vector2Int> GetRookMoveDirections() => new()
            { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private List<Vector2Int> GetKnightMoves() => new()
        {
            new Vector2Int(1, 2), new Vector2Int(2, 1),
            new Vector2Int(2, -1), new Vector2Int(1, -2),
            new Vector2Int(-1, -2), new Vector2Int(-2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-1, 2)
        };

        private List<Vector2Int> GetBishopMoveDirections() => new()
            {
                Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
                Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
            };

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> parentMap, Vector2Int start,
            Vector2Int end)
        {
            var path = new List<Vector2Int>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = parentMap[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        private class PathNode
        {
            public Vector2Int Position { get; }
            public PathNode Parent { get; set; }
            public int GCost { get; set; }
            public int HCost { get; }
            public int FCost => GCost + HCost;

            public PathNode(Vector2Int position, PathNode parent, int gCost, int hCost)
            {
                Position = position;
                Parent = parent;
                GCost = gCost;
                HCost = hCost;
            }
        }
    }
}