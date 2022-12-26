using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.View
{
    public class SpriteView : InjectedMono, IPoolObject
    {
        [Get] private SpriteRenderer _sprite;
        [Get] private Transform _tr;
        
        private float _speed = 12f;

        public async Task MoveAnimation(CancellationToken token, Vector3 start, Vector3 finish)
        {
            gameObject.SetActive(true);
            var delta = 1f / Vector3.Distance(start, finish) * _speed;
            var time = 0f;
            while (time <= 1f && token.IsCancellationRequested == false)
            {
                time += delta * Time.deltaTime;
                _tr.position = Vector3.Lerp(start, finish, time);
                await Task.Yield();
            }
        }

        public async Task AppearAnimation(CancellationToken token, Vector3 pos)
        {
            _tr.position = pos;
            var currentLerp = 0f;

            var big = Vector3.one * 1.1f;
            var small = Vector3.one * 0.9f;
            var speed = 4f;

            //grow
            while (currentLerp <= 1f && token.IsCancellationRequested == false)
            {
                currentLerp += Time.deltaTime * speed * 4f;
                _tr.localScale = Vector3.Lerp(Vector3.zero, big, currentLerp);
                await Task.Yield();
            }

            currentLerp = 0f;
            //small 
            while (currentLerp <= 1f && token.IsCancellationRequested == false)
            {
                currentLerp += Time.deltaTime * speed;
                _tr.localScale = Vector3.Lerp(big, small, currentLerp);
                await Task.Yield();
            }

            currentLerp = 0f;

            //normal 
            while (currentLerp <= 1f && token.IsCancellationRequested == false)
            {
                currentLerp += Time.deltaTime * speed;
                _tr.localScale = Vector3.Lerp(small, Vector3.one, currentLerp);
                await Task.Yield();
            }

        }
        public void UpdateSprite(Sprite sprite)
        {
            _sprite.sprite = sprite;
        }
        
        public void OnEnterPool()
        {
            gameObject.SetActive(false);
        }

        public void OnExitPool()
        {
            gameObject.SetActive(true);
        }

    }
}
