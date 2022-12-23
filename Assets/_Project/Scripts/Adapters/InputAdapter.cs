using System;
using MonoDI.Sample;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Adapters
{
    public class InputAdapter : InjectedMono
    {
        [In] private SignalBus _signal;

        private IInputController _input;

        public override void OnSyncStart()
        {
            _input = 
#if UNITY_EDITOR || UNITY_STANDALONE
                new KeyBoardInputController();
#elif UNITY_IOS || UNITY_ANDRIOD
                new TouchScreenInputController();
#else
                new KeyBoardInputController();
#endif
        }

        private void Update()
        {
            _input.UpdateState();
            var state = _input.GetState();

            if (state != InputState.None)
            {
                _signal.Fire(new InputSignal
                {
                    State = state
                });
            }
        }
    }


    internal interface IInputController
    {
        void UpdateState();

        InputState GetState();
    }


    internal class KeyBoardInputController : IInputController
    {
        public void UpdateState()
        {
            
        }

        public InputState GetState()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                return InputState.Up;
            }
            
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                return InputState.Down;
            }
            
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                return InputState.Left;
            }
                        
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                return InputState.Right;
            }

            return InputState.None;
        }
    }
    
    internal class TouchScreenInputController : IInputController
    {
        private Vector3 _pressedPosition;

        private InputState _input;
        private bool _isDown;
        private bool _isLeft;
        private bool _isRight;
        
        
        public void UpdateState()
        {
            _input = InputState.None;
            if (Input.GetMouseButtonDown(0))
            {
                _pressedPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                var dif = _pressedPosition - Input.mousePosition;

                if (Mathf.Abs(dif.x) > Mathf.Abs(dif.y))
                {
                    //horizontal is bigger
                    if (dif.x > 0)
                    {
                        _input = InputState.Left;
                    }
                    else
                    {
                        _input = InputState.Right;
                    }
                }
                else
                {
                    //horizontal is bigger
                    if (dif.y > 0)
                    {
                        _input = InputState.Up;
                    }
                    else
                    {
                        _input = InputState.Down;
                    }
                }
            }
            
            
        }

        public InputState GetState()
        {
            return _input;
        }
    }

    public enum InputState
    {
        None, Up, Down, Left, Right, 
    }

    public struct InputSignal : ISignal
    {
        public InputState State;
    }
}
