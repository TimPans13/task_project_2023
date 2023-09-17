using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private readonly Dictionary<ChessUnitType, IChessGridNavigator> _pathfinders;

        public ChessGridNavigator()
        {
            _pathfinders = new Dictionary<ChessUnitType, IChessGridNavigator>
        {
            { ChessUnitType.Pon, new PonPathfinder() },
            { ChessUnitType.King, new KingPathfinder() },
            { ChessUnitType.Queen, new QueenPathfinder() },
            { ChessUnitType.Rook, new RookPathfinder() },
            { ChessUnitType.Knight, new KnightPathfinder() },
            { ChessUnitType.Bishop, new BishopPathfinder() },
            
        };
        }

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (!_pathfinders.ContainsKey(unit))
            {
                throw new NotSupportedException($"No pathfinding logic for unit type {unit}");
            }

            var pathfinder = _pathfinders[unit];
            return pathfinder.FindPath(unit, from, to, grid);
        }
    }
}