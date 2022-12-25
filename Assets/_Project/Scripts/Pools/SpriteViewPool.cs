using _Project.Scripts.Config;
using _Project.Scripts.View;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Pools
{
    public class SpriteViewPool : MonoBehaviour
    {
        [SerializeField] private SpriteConfig _spriteConfig;

        public SpriteView GetFromPool(int value)
        {
            var view = Pool<SpriteView>.GetFromPool(CreateNewView);
            view.UpdateSprite(_spriteConfig.GetSpriteByValue(value));
            view.Value = value;
            return view;
        }
        
        public void SetInPool(SpriteView value)
        {
            Pool<SpriteView>.SetInPool(value);
        }

        private SpriteView CreateNewView()
            => Instantiate<SpriteView>(_spriteConfig.Prefab);

        private void OnDestroy()
        {
            Pool<SpriteView>.ForceClearAll();
        }
    }
}
