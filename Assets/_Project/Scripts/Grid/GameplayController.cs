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

        [Get] private BoardAnimation _boardAnimation;

        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private float _cellSize = 4;

        public override void OnSyncStart()
        {
            _grid.InitGrid(_gridSize.x, _gridSize.y);
            _boardAnimation.InitBoard(_gridSize.x, _gridSize.y, _cellSize);
        }

        public override IEnumerator OnSyncLastCallRoutine()
        {
            yield return null;
            _boardAnimation.ResetItems();
            _boardAnimation.AddChange(_grid.AddPieceToRandomPlace());
            _boardAnimation.AddChange(_grid.AddPieceToRandomPlace());
            _boardAnimation.Execute(BoardAnimation.ExecutionType.Create);
        }

        [Sub]
        private void OnInputUpdate(InputSignal state)
        {
            if (_boardAnimation.IsAnimating)
                return;
            
            switch (state.State)
            {
                case InputState.None:
                    break;
                case InputState.Down:
                    StartCoroutine(ApplyGridChanges(_grid.ComputeUp()));
                    break;
                case InputState.Up:
                    StartCoroutine(ApplyGridChanges(_grid.ComputeDown()));
                    break;
                case InputState.Left:
                    StartCoroutine(ApplyGridChanges(_grid.ComputeLeft()));
                    break;
                case InputState.Right:
                    StartCoroutine(ApplyGridChanges(_grid.ComputeRight()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }

        private IEnumerator ApplyGridChanges(List<GridChange> changes)
        {
            if (changes.Count == 0)
                yield break;

            _boardAnimation.ResetItems();
            
            foreach (var change in changes)
            {
                _boardAnimation.AddChange(change);
            }

            _boardAnimation.Execute(BoardAnimation.ExecutionType.Move);
            yield return new WaitWhile(() => _boardAnimation.IsAnimating);
            
            _boardAnimation.Execute(BoardAnimation.ExecutionType.Destroy);
            yield return new WaitWhile(() => _boardAnimation.IsAnimating);
            
            _boardAnimation.AddChange(_grid.AddPieceToRandomPlace());
            
            _boardAnimation.Execute(BoardAnimation.ExecutionType.Create);
            yield return new WaitWhile(() => _boardAnimation.IsAnimating);
            

            
            _boardAnimation.ValidateBoardGrid(_grid.GetRawGrid());
            
            
            
        }

        
        


    }
}
