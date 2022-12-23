using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.View;
using UnityEngine;

namespace _Project.Scripts.Config
{
    [CreateAssetMenu(fileName = "SpriteConfig", menuName = "_Config/SpriteConfig", order = 1)]
    public class SpriteConfig : ScriptableObject
    {
        [SerializeField] private SpriteView _prefab;
        [SerializeField] private List<SpriteData> _data;

        public SpriteView Prefab => _prefab;

        public Sprite GetSpriteByValue(int value) 
            => _data.Find(_ => _.Value == value)?.Image;


    }

    [System.Serializable]
    public class SpriteData
    {
        public int Value;
        public Sprite Image;
    }
}
