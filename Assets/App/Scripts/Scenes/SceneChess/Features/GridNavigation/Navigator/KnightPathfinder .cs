using System;
using System.Collections.Generic;
using System.Diagnostics;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class KnightPathfinder : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (unit != ChessUnitType.Knight)
            {
                throw new NotSupportedException($"KnightPathfinder doesn't support unit type {unit}");
            }

            var openSet = new List<Vector2Int> { from };
            var distance = new Dictionary<Vector2Int, int>();
            distance[from] = 0;
            var previous = new Dictionary<Vector2Int, Vector2Int>();
            var allPaths = new List<List<Vector2Int>>();

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
                    allPaths.Add(path);
                }

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!IsValidMove(grid, neighbor))
                    {
                        continue;
                    }

                    int stepCost = 3;

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

            if (allPaths.Count > 0)
            {
                List<Vector2Int> shortestPath = allPaths[0];
                for (int i = 1; i < allPaths.Count; i++)
                {
                    if (allPaths[i].Count < shortestPath.Count)
                    {
                        shortestPath = allPaths[i];
                    }
                }
                return shortestPath;
            }

            return null;
        }

        private bool IsValidMove(ChessGrid grid, Vector2Int position)
        {
            if (position.x < 0 || position.x > 7 || position.y < 0 || position.y > 7)
            {
                return false;
            }

            return grid.Get(position.y, position.x) == null;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            var neighbors = new List<Vector2Int>();
            int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
            int[] dy = { -2, -1, 1, 2, 2, 1, -1, -2 };

            for (int i = 0; i < 8; i++)
            {
                int newX = position.x + dx[i];
                int newY = position.y + dy[i];
                neighbors.Add(new Vector2Int(newX, newY));
            }

            return neighbors;
        }
    }
}
