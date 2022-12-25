using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Grid;
using _Project.Scripts.Pools;
using MonoDI.Scripts.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace _Project.Scripts.View
{
    public class BoardAnimation : InjectedMono
    {
        [In] private SpriteViewPool _pool;

        private readonly List<GridChange> _items = new List<GridChange>();

        private SpriteView[,] _array;

        private float _cellSize;
        
        public bool IsAnimating
        {
            get;
            private set;
        }

        public void InitBoard(int sizeX, int sizeY, float cellSize)
        {
            _array = new SpriteView[sizeX, sizeY];
            _cellSize = cellSize;
        }

        public void ResetItems()
        {
            _items.Clear();
        }
        
        public void AddChange(GridChange change)
        {
            _items.Add(change);
        }

        public void ValidateBoardGrid(int[,] grid)
        {
            Assert.AreEqual(grid.GetLength(0), _array.GetLength(0));
            Assert.AreEqual(grid.GetLength(1), _array.GetLength(1));
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == 0)
                    {
                        if (_array[i, j] != null)
                        {
                            Debug.LogError("Expected to be NULL");
                        }
                    }
                    else
                    {
                        if (_array[i, j] == null)
                        {
                            Debug.LogError("Expected to NOT be NULL");
                        }
                        if (grid[i, j] != _array[i, j].Value)
                        {
                            Debug.LogError("Expected Same Value");
                        }
                    }
                    
                    
                }
            }
        }

        public void Execute(ExecutionType executionType)
        {
            IsAnimating = true;
            StartCoroutine(ExecuteRoutine(executionType));
        }

        private IEnumerator ExecuteRoutine(ExecutionType executionType)
        {
            var create = executionType == ExecutionType.Create;
            var destroy = executionType == ExecutionType.Destroy;

            var exeList = _items.FindAll(_ => _.IsCreated == create && _.IsDestroy == destroy);

            foreach (var change in exeList)
            {
                var c = change;

                if (change.IsCreated)
                {
                    var item = _array[change.MovedTo.x, change.MovedTo.y];
                    if (item != null)
                        throw new System.Exception("Expected to NOT exist");

                    var o = _pool.GetFromPool(change.Value);
                    
                    _array[change.MovedTo.x, change.MovedTo.y] = o;
                    o.name = $"{change.MovedTo.x} { change.MovedTo.y}";

                    o.Animate(GridToWorld(change.MovedTo), SpriteView.AnimationType.Create,
                        () =>
                        {
                            exeList.Remove(c);
                        });
                } else if (change.IsDestroy)
                {
                    var item = _array[change.MovedFrom.x, change.MovedFrom.y];
                    if (item == null)
                        throw new System.Exception("Expected to exist");
                    
                    _array[change.MovedFrom.x, change.MovedFrom.y] = null;
                    item.name = $"----";


                    item.Animate(GridToWorld(change.MovedFrom), SpriteView.AnimationType.Disappear,
                        () =>
                        {
                            exeList.Remove(c);
                        });
                }
                else //move 
                {
                    var item = _array[change.MovedFrom.x, change.MovedFrom.y];
                    if (item == null)
                        throw new System.Exception("Expected to exist");
                    
                    _array[change.MovedFrom.x, change.MovedFrom.y] = null;
                    _array[change.MovedTo.x, change.MovedTo.y] = item;
                    
                    item.name = $"{change.MovedTo.x} {change.MovedTo.y}";

                    
                    item.Animate(GridToWorld(change.MovedTo), SpriteView.AnimationType.Move,
                        () =>
                        {
                            exeList.Remove(c);
                        });
                }
            }

            yield return new WaitWhile(() => exeList.Count > 0);
            
            IsAnimating = false;
        }
        
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * _cellSize, gridPos.y * _cellSize);
        }
        
        public enum ExecutionType
        {
            Create, Destroy, Move
        }
    }

    internal class AnimationItem
    {
        public SpriteView Item;
        public GridChange Change;
        public Vector3 WorldPos;
    }
}
