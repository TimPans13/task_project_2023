using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class BishopPathfinder : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (unit != ChessUnitType.Bishop)
            {
                throw new NotSupportedException($"BishopPathfinder doesn't support unit type {unit}");
            }

            var openSet = new List<Vector2Int> { from };
            var distance = new Dictionary<Vector2Int, int>();
            distance[from] = 0;
            var previous = new Dictionary<Vector2Int, Vector2Int>();

            while (openSet.Count > 0)
            {
                var current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (distance.ContainsKey(openSet[i]) && distance[openSet[i]] < distance[current])
                    {
                        current = openSet[i];
                    }
                }

                openSet.Remove(current);

                if (current == to)
                {
                    var path = new List<Vector2Int>();
                    while (previous.ContainsKey(current))
                    {
                        path.Insert(0, current);
                        current = previous[current];
                    }
                    path.Insert(0, from);
                    return path;
                }

                foreach (var neighbor in GetDiagonalNeighbors(current))
                {
                    if (!IsValidMove(grid, neighbor))
                    {
                        continue;
                    }

                    int stepCost = 5;

                    if (!(previous.ContainsKey(current) && GetDirection(current, neighbor) != GetDirection(current, previous[current])))
                    {
                        stepCost = 0;
                    }

                    int newDistance = distance[current] + stepCost;

                    if (!distance.ContainsKey(neighbor) || newDistance < distance[neighbor])
                    {
                        distance[neighbor] = newDistance;
                        previous[neighbor] = current;
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private bool IsValidMove(ChessGrid grid, Vector2Int position)
        {
            return position.x >= 0 && position.x < grid.Size.x && position.y >= 0 && position.y < grid.Size.y && grid.Get(position.y, position.x) == null;
        }

        private List<Vector2Int> GetDiagonalNeighbors(Vector2Int position)
        {
            var neighbors = new List<Vector2Int>();
            neighbors.Add(new Vector2Int(position.x + 1, position.y + 1));
            neighbors.Add(new Vector2Int(position.x - 1, position.y - 1));
            neighbors.Add(new Vector2Int(position.x - 1, position.y + 1));
            neighbors.Add(new Vector2Int(position.x + 1, position.y - 1));
            return neighbors;
        }

        private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
        {
            return new Vector2Int(Math.Sign(to.x - from.x), Math.Sign(to.y - from.y));
        }
    }
}
