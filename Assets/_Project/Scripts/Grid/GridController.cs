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

        public GridChange AddPieceAt(int x, int y, int value)
        {
            _grid[x, y] = value;
            return new GridChange(new Vector2Int(x, y), new Vector2Int(x, y),
                GridChangeType.Create, value);
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
                    return new GridChange(new Vector2Int(rX, rY), new Vector2Int(rX, rY),
                        GridChangeType.Create, 2);
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

        private List<GridChange> Compute(Vector2Int start, Vector2Int end, Vector2Int direction, Vector2Int step)
        {
            var list = new List<GridChange>();
            
            ComputeMoving(start, end, direction, step, list);
            ComputeCombine(start, end, direction, step, list);
            ComputeMoving(start, end, direction, step, list);

            return list;
        }

        private void ComputeMoving(Vector2Int start, Vector2Int end, Vector2Int direction, Vector2Int step,
            List<GridChange> list)
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = start.x + step.x; i != end.x; i += direction.x)
                {
                    for (int j = start.y + step.y; j != end.y; j += direction.y)
                    {
                        if (_grid[i - step.x, j - step.y] == 0 && _grid[i, j] != 0)
                        {
                            // to avoid duplication of move of object but to different position
                            var obj = list
                                .Find(
                                    _ => _.MovedTo.x == i && 
                                          _.MovedTo.y == j &&
                                          _.Type != GridChangeType.Destroy);
                            
                            if (obj == null)
                            {
                                obj = new GridChange(
                                    new Vector2Int(i, j),
                                    new Vector2Int(i, j), 
                                    GridChangeType.Move, _grid[i, j]);
                                list.Add(obj);
                            }

                            obj.MovedTo = new Vector2Int(i - step.x, j - step.y);
                            
                            _grid[i - step.x, j - step.y] = _grid[i, j];
                            _grid[i, j] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }
        }

        private void ComputeCombine(Vector2Int start, Vector2Int end, Vector2Int direction,
            Vector2Int step, List<GridChange> list)
        {
            for (int i = start.x + step.x; i != end.x; i += direction.x)
            {
                for (int j = start.y + step.y; j != end.y; j += direction.y)
                {
                    if (_grid[i - step.x, j - step.y] == 0)
                        continue;


                    if (_grid[i, j] == _grid[i - step.x, j - step.y])
                    {
                        _grid[i, j] = 0;
                        _grid[i - step.x, j - step.y] *= 2;

                        list.Add(new GridChange(new Vector2Int(i - step.x, j - step.y),
                            new Vector2Int(i - step.x, j - step.y), GridChangeType.Destroy, 0));

                        list.Add(new GridChange(new Vector2Int(i, j),
                            new Vector2Int(i, j), GridChangeType.Destroy, 0));
                        
                        list.Add(new GridChange(new Vector2Int(i - step.x, j - step.y),
                            new Vector2Int(i - step.x, j - step.y), GridChangeType.Create, _grid[i - step.x, j - step.y]));
                    }
                }
            }
        }

        public List<GridChange> ComputeDown()
        {
            return Compute(
                new Vector2Int(0, _sizeY - 1),
                new Vector2Int(_sizeX, -1),
                new Vector2Int(1, -1),
                new Vector2Int(0, -1));
        }

        public List<GridChange> ComputeUp()
        {
            return Compute(
                new Vector2Int(0, 0),
                new Vector2Int(_sizeX, _sizeY),
                new Vector2Int(1, 1),
                new Vector2Int(0, 1));
        }

        public List<GridChange> ComputeRight()
        {
            return Compute(
                new Vector2Int(_sizeX - 1, 0),
                new Vector2Int(-1, _sizeY),
                new Vector2Int(-1, 1),
                new Vector2Int(-1, 0));
        }

        public List<GridChange> ComputeLeft()
        {
            return Compute(
                new Vector2Int(0, 0),
                new Vector2Int(_sizeX, _sizeY),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0));
        }
    }

    public class GridChange
    {
        public Vector2Int MovedFrom;
        public Vector2Int MovedTo;
        public GridChangeType Type;
        public int Value;

        public GridChange(Vector2Int movedFrom, Vector2Int movedTo, GridChangeType type, int value)
        {
            MovedFrom = movedFrom;
            MovedTo = movedTo;
            Type = type;
            Value = value;
        }
    }

    public enum GridChangeType
    {
        Create, Destroy, Move
    }
}
