using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Grid
{
    public class GridController : MonoBehaviour
    {
        private int[,] _grid;
        private int _sizeX;
        private int _sizeY;

        public int[,] GetRawGrid() => _grid;

        public void InitGrid(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeX;
            _grid = new int[sizeX, sizeY];
        }

        public void InitGrid(int[,] grid)
        {
            _grid = grid;
            _sizeX = _grid.GetLength(0);
            _sizeY = _grid.GetLength(1);
        }

        public int GetElement(int x, int y)
        {
            return _grid[x, y];
        }

        public GridChange AddPieceToRandomPlace()
        {
            if (IsFull())
                throw new Exception("Not able to add new piece because grid is full");

            while (true)
            {
                var rX = Random.Range(0, _sizeX);
                var rY = Random.Range(0, _sizeY);
                if (_grid[rX, rY] == 0)
                {
                    _grid[rX, rY] = 2;
                    return new GridChange
                    {
                        MovedTo = new Vector2Int(rX, rY),
                        IsCreated = true,
                        Value = 2
                    };
                }
            }
        }

        public bool IsFull()
        {
            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    if (_grid[i, j] == 0)
                        return false;
                }
            }

            return true;
        }

        private List<GridChange> Compute(Vector2Int start, Vector2Int size, Vector2Int side)
        {
            var list = new List<GridChange>();
            
            ComputeMoving(start, size, side, list);
            ComputeCombine(start, size, side, list);
            ComputeMoving(start, size, side, list);

            return list;
        }

        private void ComputeMoving(Vector2Int start, Vector2Int size, Vector2Int side, List<GridChange> list)
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = start.x; i < size.x; i++)
                {
                    for (int j = start.y; j < size.y; j++)
                    {
                        if (_grid[i + side.x, j + side.y] != 0 && _grid[i, j] == 0)
                        {
                            var obj = list
                                .Find(_ => (_.MovedTo.x == i + side.x 
                                            && _.MovedTo.y == j + side.y));
                            if (obj == null)
                            {
                                obj = new GridChange
                                {
                                    MovedFrom = new Vector2Int(i + side.x, j + side.y)
                                };
                                list.Add(obj);
                            }

                            obj.MovedTo = new Vector2Int(i, j);
                            
                            _grid[i, j] = _grid[i + side.x, j + side.y];
                            _grid[i + side.x, j + side.y] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }
        }

        public void ComputeCombine(Vector2Int start, Vector2Int size, Vector2Int side, List<GridChange> list)
        {
            for (int i = start.x; i < size.x; i++)
            {
                for (int j = start.y; j < size.y; j++)
                {
                    if (_grid[i, j] == 0)
                        continue;
                    
                    if (_grid[i, j] == _grid[i + side.x, j + side.y])
                    {
                        list.Add(new GridChange
                        {
                            IsDestroy = true,
                            MovedFrom = new Vector2Int(i + side.x, j + side.y)
                        });
                        
                        list.Add(new GridChange
                        {
                            IsDestroy = true,
                            MovedFrom = new Vector2Int(i, j)
                        });

                        _grid[i, j] *= 2;
                        _grid[i + side.x, j + side.y] = 0;
                        
                        list.Add(new GridChange
                        {
                            IsCreated = true,
                            Value = _grid[i, j],
                            MovedTo = new Vector2Int(i, j)
                        });
                    }
                }
            }
        }

        public List<GridChange> ComputeDown()
        {
            return Compute(
                new Vector2Int(0, 1),
                new Vector2Int(_sizeX, _sizeY),
                new Vector2Int(0, -1));
        }

        public List<GridChange> ComputeUp()
        {
            return Compute(
                new Vector2Int(0, 0),
                new Vector2Int(_sizeX, _sizeY - 1),
                new Vector2Int(0, 1));
        }

        public List<GridChange> ComputeRight()
        {
            return Compute(
                new Vector2Int(1, 0),
                new Vector2Int(_sizeX, _sizeY),
                new Vector2Int(-1, 0));
        }

        public List<GridChange> ComputeLeft()
        {
            return Compute(
                new Vector2Int(0, 0),
                new Vector2Int(_sizeX - 1, _sizeY),
                new Vector2Int(1, 0));
        }
    }

    public class GridChange
    {
        public Vector2Int MovedFrom;
        public Vector2Int MovedTo;
        public int Value;
        public bool IsCreated;
        public bool IsDestroy;
    }
}
