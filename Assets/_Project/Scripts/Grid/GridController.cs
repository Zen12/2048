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

        public void AddPieceToRandomPlace()
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
                    return;
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

        public void ComputeDown()
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = 0; i < _sizeX; i++)
                {
                    for (int j = 1; j < _sizeY; j++)
                    {
                        if (_grid[i, j - 1] != 0 && _grid[i, j] == 0)
                        {
                            _grid[i, j] = _grid[i, j - 1];
                            _grid[i, j - 1] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }

            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 1; j < _sizeY; j++)
                {
                    if (_grid[i, j] == _grid[i, j - 1])
                    {
                        _grid[i, j] *= 2;
                        _grid[i, j - 1] = 0;
                    }
                }
            }

        }

        public void ComputeUp()
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = 0; i < _sizeX; i++)
                {
                    for (int j = 0; j < _sizeY - 1; j++)
                    {
                        if (_grid[i, j + 1] != 0 && _grid[i, j] == 0)
                        {
                            _grid[i, j] = _grid[i, j + 1];
                            _grid[i, j + 1] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }

            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY - 1; j++)
                {
                    if (_grid[i, j] == 0)
                        continue;
                    if (_grid[i, j] == _grid[i, j + 1])
                    {
                        _grid[i, j] *= 2;
                        _grid[i, j + 1] = 0;
                    }
                }
            }

        }

        public void ComputeRight()
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = 1; i < _sizeX; i++)
                {
                    for (int j = 0; j < _sizeY; j++)
                    {
                        if (_grid[i - 1, j] != 0 && _grid[i, j] == 0)
                        {
                            _grid[i, j] = _grid[i - 1, j];
                            _grid[i - 1, j] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }


            for (int i = 1; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    if (_grid[i, j] == _grid[i - 1, j])
                    {
                        if (_grid[i, j] == 0)
                            continue;
                        _grid[i, j] *= 2;
                        _grid[i - 1, j] = 0;
                    }
                }
            }
        }

        public void ComputeLeft()
        {
            bool wasMoved = true;
            while (wasMoved)
            {
                wasMoved = false;
                for (int i = 0; i < _sizeX - 1; i++)
                {
                    for (int j = 0; j < _sizeY; j++)
                    {
                        if (_grid[i + 1, j] != 0 && _grid[i, j] == 0)
                        {
                            _grid[i, j] = _grid[i + 1, j];
                            _grid[i + 1, j] = 0;
                            wasMoved = true;
                        }
                    }
                }
            }

            for (int i = 0; i < _sizeX - 1; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    if (_grid[i, j] == _grid[i + 1, j])
                    {
                        if (_grid[i, j] == 0)
                            continue;
                        _grid[i, j] *= 2;
                        _grid[i + 1, j] = 0;
                    }
                }
            }
        }

    }

    public class GridResult
    {
        public Vector2Int MovedFrom;
        public Vector2Int MovedTo;
        public int StartValue;
        public int FinalValue;
        public bool IsMeshed;
    }
}
