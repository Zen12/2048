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
        }

        public override IEnumerator OnSyncLastCallRoutine()
        {
            yield return null;
            _boardAnimation.InitBoard(_gridSize.x, _gridSize.y, _cellSize);

            var start = _grid.GetRawGrid();
            var changes = new List<GridChange>();
            changes.Add(_grid.AddPieceToRandomPlace());
            changes.Add(_grid.AddPieceToRandomPlace());
            var end = _grid.GetRawGrid();
            yield return _boardAnimation.TransitionToGridAnimation(start, end, changes);
        }

        [Sub]
        private async void OnInputUpdate(InputSignal state)
        {
            var start = _grid.GetRawGrid();
            List<GridChange> changes = null;

            switch (state.State)
            {
                case InputState.None:
                    return;
                case InputState.Down:
                    changes = _grid.ComputeUp();
                    break;
                case InputState.Up:
                    changes = _grid.ComputeDown();
                    break;
                case InputState.Left:
                    changes = _grid.ComputeLeft();
                    break;
                case InputState.Right:
                    changes = _grid.ComputeRight();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            if (changes.Count == 0)
                return;

            changes.Add(_grid.AddPieceToRandomPlace());
            await _boardAnimation.TransitionToGridAnimation(start, _grid.GetRawGrid(), changes);

        }

    }
}
