using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        private GridHolder[,] _array;

        private float _cellSize;
        private int _sizeX;
        private int _sizeY;

        public void InitBoard(int sizeX, int sizeY, float cellSize)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            
            _array = new GridHolder[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    _array[i, j] = new GridHolder(_pool);
                }
            }
            _cellSize = cellSize;
        }

        private async Task AnimateAndSetToPool(SpriteView view, Vector3 startPos, Vector3 finishPos)
        {
            await view.MoveAnimation(_onDestroyToken.Token, startPos, finishPos);
            if (_onDestroyToken.IsCancellationRequested == false)
                _pool.SetInPool(view);
        }
        
        public async Task TransitionToGridAnimation(int[,] start, int[,] finish, List<GridChange> changes)
        {
            Assert.AreEqual(start.GetLength(0), _sizeX);
            Assert.AreEqual(start.GetLength(1), _sizeY);
            
            Assert.AreEqual(finish.GetLength(0), _sizeX);
            Assert.AreEqual(finish.GetLength(1), _sizeY);

            var tasks = new List<Task>();
            //start or reset
            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    var value = finish[i, j];
                    var (_, view) = _array[i, j].UpdateView(value);
                    if (view != null)
                        view.transform.position = GridToWorld(i, j);
                }
            }

            //move animation
            foreach (var change in changes)
            {
                if (change.Type == GridChangeType.Move)
                {
                    var holder = _array[change.MovedFrom.x, change.MovedFrom.y];
                    var startPos = GridToWorld(change.MovedFrom);
                    var finishPos = GridToWorld(change.MovedTo);
                    var view = holder.View;
                    if (view != null)
                    {
                        tasks.Add(AnimateAndSetToPool(view, startPos, finishPos));
                    }
                    holder.Clear();
                }
            }
            Debug.Log("Here!");
            
            await Task.WhenAll(tasks);
            
            //finish grid, ignore animation results
            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    var value = finish[i, j];
                    var (_, view) = _array[i, j].UpdateView(value);
                    if (view != null)
                        view.transform.position = GridToWorld(i, j);
                }
            }
            //appear animation
            foreach (var change in changes)
            {
                if (change.Type == GridChangeType.Create)
                {
                    var holder = _array[change.MovedTo.x, change.MovedTo.y];
                    var (_, v) = holder.UpdateView(change.Value);
                    _ = v.AppearAnimation(_onDestroyToken.Token, GridToWorld(change.MovedTo));
                } 
            }
        }
        
        public Vector3 GridToWorld(int x, int y)
        {
            return new Vector3(x * _cellSize, y * _cellSize);
        }
        
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * _cellSize, gridPos.y * _cellSize);
        }
        
    }

    internal class GridHolder
    {
        public SpriteView View;
        public int Value;

        private readonly SpriteViewPool _pool;

        public GridHolder(SpriteViewPool pool)
        {
            _pool = pool;
        }

        public void Clear()
        {
            View = null;
            Value = 0;
        }
        
        public (bool, SpriteView) UpdateView(int value)
        {
            var isUpdate = Value != value;

            if (isUpdate == false)
            {
                return (false, View);
            }
            Value = value;

            
            if (View != null)
            {
                _pool.SetInPool(View);
                View = null;
            }

            if (value > 0)
            {
                View = _pool.GetFromPool(value);
            }

            return (true, View);
        }
        
        
    }
}
