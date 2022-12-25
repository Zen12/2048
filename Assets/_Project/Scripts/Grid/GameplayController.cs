using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Adapters;
using _Project.Scripts.Pools;
using _Project.Scripts.View;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Grid
{
    public class GameplayController : InjectedMono
    {
        [In] private GridController _grid;
        [In] private SpriteViewPool _pool;

        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private float _cellSize = 4;

        private List<SpriteView> _currentActive = new List<SpriteView>();

        public override void OnSyncStart()
        {
            _grid.InitGrid(_gridSize.x, _gridSize.y);
        }

        public override IEnumerator OnSyncLastCallRoutine()
        {
            yield return null;
            ApplyGridChange(_grid.AddPieceToRandomPlace());
            ApplyGridChange(_grid.AddPieceToRandomPlace());
        }
        
        [Sub]
        private void OnInputUpdate(InputSignal state)
        {
            switch (state.State)
            {
                case InputState.None:
                    break;
                case InputState.Down:
                    ApplyGridChanges(_grid.ComputeUp());
                    break;
                case InputState.Up:
                    ApplyGridChanges(_grid.ComputeDown());
                    break;
                case InputState.Left:
                    ApplyGridChanges(_grid.ComputeLeft());
                    break;
                case InputState.Right:
                    ApplyGridChanges(_grid.ComputeRight());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }

        public void ApplyGridChanges(List<GridChange> changes)
        {
            foreach (var gridChange in changes)
            {
                ApplyGridChange(gridChange);
            }

            if (changes.Count > 0)
            {
                ApplyGridChange(_grid.AddPieceToRandomPlace());
            }
        }


        public void ApplyGridChange(GridChange change)
        {
            if (change.IsCreated)
            {
                var o = _pool.GetFromPool(change.Value);
                _currentActive.Add(o);
                o.Animate(GridToWorld(change.MovedTo), change.MovedTo, SpriteView.AnimationType.Create);
            }
            else if (change.IsDestroy)
            {
                var o = FindByGridPos(change.MovedFrom);
                _currentActive.Remove(o);
                _pool.SetInPool(o);
            }
            else
            {
                var o = FindByGridPos(change.MovedFrom);
                o.Animate(GridToWorld(change.MovedTo), change.MovedTo, SpriteView.AnimationType.Move);
            }
        }

        private SpriteView FindByGridPos(Vector2Int gridPos)
        {
            return _currentActive.Find(_ =>
                _.GridPos.x == gridPos.x &&
                _.GridPos.y == gridPos.y);
        }
        

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * _cellSize, gridPos.y * _cellSize);
        }
    }
}
