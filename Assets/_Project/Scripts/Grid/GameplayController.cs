using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Adapters;
using _Project.Scripts.UI;
using _Project.Scripts.View;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Grid
{
    public class GameplayController : InjectedMono
    {
        [In] private GridController _grid;
        [In] private SignalBus _signal;

        [Get] private BoardAnimation _boardAnimation;

        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private float _cellSize = 4;

        private AppState _state = AppState.Init;

        public override void OnSyncStart()
        {
            _grid.InitGrid(_gridSize.x, _gridSize.y);
        }

        public override IEnumerator OnSyncLastCallRoutine()
        {
            yield return null;
            _boardAnimation.InitBoard(_gridSize.x, _gridSize.y, _cellSize);

            var changes = new List<GridChange>();
            changes.Add(_grid.AddPieceToRandomPlace());
            changes.Add(_grid.AddPieceToRandomPlace());
            var end = _grid.GetRawGrid();
            _state = AppState.Animation;
            yield return _boardAnimation.TransitionToGridAnimation(end, changes);
            _state = AppState.WaitingForInput;
        }

        [Sub]
        private async void OnInputUpdate(InputSignal state)
        {
            if (_state != AppState.WaitingForInput)
                return;
            
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
            _state = AppState.Animation;
            await _boardAnimation.TransitionToGridAnimation(_grid.GetRawGrid(), changes);
            
            //check for lose state
            if (_grid.IsPossibleToMakeMoves() == false)
            {
                _state = AppState.Lose;
                _signal.Fire(new LoseSignal());
            }
            else
            {
                _state = AppState.WaitingForInput;
            }
        }
    }

    public enum AppState
    {
        Init, WaitingForInput, Animation, Lose
    }
}
