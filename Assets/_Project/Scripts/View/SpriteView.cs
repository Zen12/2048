using System;
using System.Collections;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.View
{
    public class SpriteView : InjectedMono, IPoolObject
    {
        [Get] private SpriteRenderer _sprite;
        [Get] private Transform _tr;

        private Vector3 _targetPos;
        private Coroutine _lastRoutine;
        private System.Action _onFinishMove;

        public Vector2Int GridPos
        {
            get;
            private set;
        }

        private float _speed = 12f;

        public void UpdateOnFinish(System.Action onFinish)
        {
            _onFinishMove = onFinish;
        }


        public void Animate(Vector3 pos, Vector2Int gridPos, AnimationType animationType,
            System.Action onFinish = null)
        {
            gameObject.SetActive(true);
            _onFinishMove = onFinish;
            GridPos = gridPos;
            if (_lastRoutine != null)
                StopCoroutine(_lastRoutine);

            switch (animationType)
            {
                case AnimationType.Move:
                    _targetPos = pos;
                    _lastRoutine = StartCoroutine(MoveToPosRoutine());
                    break;
                case AnimationType.Create:
                    _targetPos = pos;
                    _lastRoutine = StartCoroutine(CreateRoutine());
                    break;
                case AnimationType.Disappear:
                    _lastRoutine = StartCoroutine(DisappearRoutine());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
            }
        }
        public void UpdateSprite(Sprite sprite)
        {
            _sprite.sprite = sprite;
        }
        
        public void OnEnterPool()
        {
        }

        public void OnExitPool()
        {
        }
        
        #region Animation

        private IEnumerator DisappearRoutine()
        {
            var delta = Time.deltaTime * _speed;
            var dir = _targetPos - _tr.position;

            while(dir.magnitude > delta + float.Epsilon)
            {
                _tr.position += dir.normalized * delta;
                yield return null;
                delta = Time.deltaTime * _speed;
                dir = _targetPos - _tr.position;
            }
            
            _tr.position = _targetPos;
            
            var currentLerp = 0f;

            //grow
            while (currentLerp < 1f)
            {
                currentLerp += Time.deltaTime * 4f;
                _tr.localScale = Vector3.Lerp(Vector3.zero, Vector3.zero, currentLerp);
                yield return null;
            }
            
            _lastRoutine = null;
            _onFinishMove?.Invoke();
            gameObject.SetActive(false);
        }

        private IEnumerator MoveToPosRoutine()
        {
            yield return null;
            var delta = Time.deltaTime * _speed;
            var dir = _targetPos - _tr.position;

            while(dir.magnitude > delta + float.Epsilon)
            {
                _tr.position += dir.normalized * delta;
                yield return null;
                delta = Time.deltaTime * _speed;
                dir = _targetPos - _tr.position;
            }

            _tr.position = _targetPos;
            _lastRoutine = null;
            _onFinishMove?.Invoke();
        }
        
        private IEnumerator CreateRoutine()
        {
            _tr.position = _targetPos;
            _tr.localScale = Vector3.zero;

            var currentLerp = 0f;

            var big = Vector3.one * 1.1f;
            var small = Vector3.one * 0.9f;
            var speed = 4f;

            //grow
            while (currentLerp < 1f)
            {
                currentLerp += Time.deltaTime * speed * 4f;
                _tr.localScale = Vector3.Lerp(Vector3.zero, big, currentLerp);
                yield return null;
            }

            currentLerp = 0f;
            //small 
            while (currentLerp < 1f)
            {
                currentLerp += Time.deltaTime * speed;
                _tr.localScale = Vector3.Lerp(big, small, currentLerp);
                yield return null;
            }

            currentLerp = 0f;

            //normal 
            while (currentLerp < 1f)
            {
                currentLerp += Time.deltaTime * speed;
                _tr.localScale = Vector3.Lerp(small, Vector3.one, currentLerp);
                yield return null;
            }
            
            _onFinishMove?.Invoke();
        }
        
        #endregion
        
        public enum AnimationType
        {
            Move, Create, Disappear
        }
    }
}
