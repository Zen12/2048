using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Adapters;
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
            yield return _boardAnimation.Execute(BoardAnimation.ExecutionType.Create);
        }

        [Sub]
        private async void OnInputUpdate(InputSignal state)
        {

            switch (state.State)
            {
                case InputState.None:
                    break;
                case InputState.Down:
                    await ApplyGridChanges(_grid.ComputeUp());
                    break;
                case InputState.Up:
                    await ApplyGridChanges(_grid.ComputeDown());
                    break;
                case InputState.Left:
                    await ApplyGridChanges(_grid.ComputeLeft());
                    break;
                case InputState.Right:
                    await ApplyGridChanges(_grid.ComputeRight());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }

        private async Task ApplyGridChanges(List<GridChange> changes)
        {
            if (changes.Count == 0)
                return;

            _boardAnimation.ResetItems();
            
            foreach (var change in changes)
            {
                _boardAnimation.AddChange(change);
            }

            await _boardAnimation.Execute(BoardAnimation.ExecutionType.Move);
            await _boardAnimation.Execute(BoardAnimation.ExecutionType.Destroy);
            _boardAnimation.AddChange(_grid.AddPieceToRandomPlace());
            await _boardAnimation.Execute(BoardAnimation.ExecutionType.Create);
            

            _boardAnimation.ValidateBoardGrid(_grid.GetRawGrid());
        }

        
        


    }
}
