using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Grid;
using UnityEngine;

namespace _Project.Scripts.View
{
    public class AnimationQueue : MonoBehaviour
    {
        private List<AnimationItem> _items = new List<AnimationItem>();

        public void ResetItems()
        {
            _items.Clear();
        }
        
        public void AddItem(SpriteView view, GridChange change, Vector3 worldPos)
        {
            _items.Add(new AnimationItem
            {
                Item = view,
                Change = change,
                WorldPos = worldPos
            });
        }

        public void Execute(System.Action callBack)
        {
            StartCoroutine(ExecuteRoutine(callBack));
        }

        private IEnumerator ExecuteRoutine(System.Action callBack)
        {
            foreach (var item in _items)
            {
                if (item.Change.IsCreated == false && item.Change.IsDestroy == false)
                {
                    var r = item;
                    item.Item.Animate(item.WorldPos, item.Change.MovedTo, SpriteView.AnimationType.Move,
                        () => _items.Remove(r));
                }
            }
            
            yield return new WaitWhile(() => _items
                .Exists(_ => _.Change.IsCreated == false && _.Change.IsDestroy == false));
            
            foreach (var item in _items)
            {
                if (item.Change.IsCreated)
                {
                    item.Item.Animate(item.WorldPos, item.Change.MovedTo, SpriteView.AnimationType.Create);
                }
            }
            
            ResetItems();
            callBack?.Invoke();
        }
    }

    internal class AnimationItem
    {
        public SpriteView Item;
        public GridChange Change;
        public Vector3 WorldPos;
    }
}
