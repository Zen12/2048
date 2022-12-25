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

        [Get] private AnimationQueue _queue;

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
            {
                var change = _grid.AddPieceToRandomPlace();
                var o = _pool.GetFromPool(change.Value);
                _currentActive.Add(o);
                o.Animate(GridToWorld(change.MovedTo), change.MovedTo, SpriteView.AnimationType.Create);
            }
            {
                var change = _grid.AddPieceToRandomPlace();
                var o = _pool.GetFromPool(change.Value);
                _currentActive.Add(o);
                o.Animate(GridToWorld(change.MovedTo), change.MovedTo, SpriteView.AnimationType.Create);
            }
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

        private void ApplyGridChanges(List<GridChange> changes)
        {
            if (changes.Count == 0)
                return;

            _queue.ResetItems();
            foreach (var change in changes)
            {
                if (change.IsCreated)
                {
                    var o = _pool.GetFromPool(change.Value);
                    _currentActive.Add(o);
                    _queue.AddItem(o, change, GridToWorld(change.MovedTo));
                } else
                {
                    var o = FindByGridPos(change.MovedFrom);
                    _queue.AddItem(o, change, GridToWorld(change.MovedTo));
                }
            }
            
            _queue.Execute(() =>
            {
                foreach (var change in changes)
                {
                    if (change.IsDestroy)
                    {
                        var o = FindByGridPos(change.MovedFrom);
                        _currentActive.Remove(o);
                        _pool.SetInPool(o);
                    }
                }
                
                {
                    var change = _grid.AddPieceToRandomPlace();
                    var o = _pool.GetFromPool(change.Value);
                    _currentActive.Add(o);
                    o.Animate(GridToWorld(change.MovedTo), change.MovedTo, SpriteView.AnimationType.Create);
                }
            });
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
