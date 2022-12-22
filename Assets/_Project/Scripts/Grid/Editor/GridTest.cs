using NUnit.Framework;
using UnityEngine;

namespace _Project.Scripts.Grid.Editor
{
    public class GridTest
    {
        private GridController _grid;
        
        [SetUp]
        public void Init()
        {
            _grid = new GameObject().AddComponent<GridController>();
        }
        
        
        [Test]
        public void GRID_INIT_1_PIECES_HECK_FOR_VALID_PIECE()
        {
            _grid.InitGrid(3, 3);
            var p = _grid.AddPieceToRandomPlace();
            Assert.IsTrue(p.IsCreated);
        }
        
        [Test]
        public void GRID_INIT_2_PIECES_HECK_FOR_VALID_GRID()
        {
            _grid.InitGrid(3, 3);
            _grid.AddPieceToRandomPlace();
            _grid.AddPieceToRandomPlace();
        }
        
        [Test]
        public void GRID_INIT_UTIL_IS_FULL_CHECK_FOR_FULL()
        {
            _grid.InitGrid(2, 2);
            _grid.AddPieceToRandomPlace();
            _grid.AddPieceToRandomPlace();
            _grid.AddPieceToRandomPlace();
            _grid.AddPieceToRandomPlace();
            Assert.IsTrue(_grid.IsFull());
        }
        
        [Test]
        public void GRID_INIT_ADD_2_PIECES_DOWN()
        {
            var arr = new int[,]
            {
                { 0, 0, 0 },
                { 2, 0, 0 },
                { 2, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeDown();
            
            Assert.AreEqual(4, _grid.GetElement(0, 2));
        }
        
        [Test]
        public void GRID_MOVE_ALL_PIECES_DOWN()
        {
            var arr = new int[,]
            {
                { 2, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 6 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeDown();
            
            Assert.AreEqual(2, _grid.GetElement(0, 2));
            Assert.AreEqual(4, _grid.GetElement(1, 2));
            Assert.AreEqual(6, _grid.GetElement(2, 2));
        }
        
        
        [Test]
        public void GRID_INIT_ADD_2_PIECES_UP()
        {
            var arr = new int[,]
            {
                { 0, 0, 0 },
                { 2, 0, 0 },
                { 2, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeUp();
            
            Assert.AreEqual(4, _grid.GetElement(0, 0));
        }
        
        [Test]
        public void GRID_MOVE_ALL_PIECES_UP()
        {
            var arr = new int[,]
            {
                { 2, 0, 0 },
                { 0, 4, 0 },
                { 8, 0, 6 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeUp();
            
            Assert.AreEqual(2, _grid.GetElement(0, 0));
            Assert.AreEqual(4, _grid.GetElement(1, 0));
            Assert.AreEqual(6, _grid.GetElement(2, 0));
            Assert.AreEqual(8, _grid.GetElement(0, 1));
        }
        
        [Test]
        public void GRID_INIT_ADD_2_PIECES_RIGHT()
        {
            var arr = new int[,]
            {
                { 0, 0, 0 },
                { 0, 2, 2 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeRight();
            
            Assert.AreEqual(4, _grid.GetElement(2, 1));
        }
        
        [Test]
        public void GRID_MOVE_ALL_PIECES_RIGHT()
        {
            var arr = new int[,]
            {
                { 2, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 6 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeRight();
            
            Assert.AreEqual(2, _grid.GetElement(2, 0));
            Assert.AreEqual(4, _grid.GetElement(2, 1));
            Assert.AreEqual(6, _grid.GetElement(2, 2));
        }
        
        [Test]
        public void GRID_INIT_ADD_2_PIECES_LEFT()
        {
            var arr = new int[,]
            {
                { 0, 0, 0 },
                { 2, 2, 0 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeLeft();
            
            Assert.AreEqual(4, _grid.GetElement(0, 1));
        }
        
        [Test]
        public void GRID_MOVE_ALL_PIECES_LEFT()
        {
            var arr = new int[,]
            {
                { 2, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 6 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            _grid.ComputeLeft();
            
            Assert.AreEqual(2, _grid.GetElement(0, 0));
            Assert.AreEqual(4, _grid.GetElement(0, 1));
            Assert.AreEqual(6, _grid.GetElement(0, 2));
        }
    }
}
