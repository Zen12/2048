using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.View
{
    public class SpriteView : InjectedMono, IPoolObject
    {
        [Get] private SpriteRenderer _sprite;
        [Get] private Transform _tr;

        public Vector2Int GridPos
        {
            get;
            private set;
        }


        public void UpdatePosition(Vector3 pos, Vector2Int gridPos)
        {
            _tr.position = pos;
            GridPos = gridPos;
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
