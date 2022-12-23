using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace _Project.Scripts.Grid.Editor
{
    public class ResultTest
    {
        private GridController _grid;
        
        [SetUp]
        public void Init()
        {
            _grid = new GameObject().AddComponent<GridController>();
        }


        [Test]
        public void RESULT_CHECK_TOP()
        {
            var arr = new int[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 8, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeUp();
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].MovedTo, new Vector2Int(0, 0));
            Assert.AreEqual(results[0].MovedFrom, new Vector2Int(0, 2));
        }
        
        
        [Test]
        public void RESULT_CHECK_MOVE_DOWN_ONE_PIECE()
        {
            var arr = new int[,]
            {
                { 0, 8, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeDown();
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].MovedFrom, new Vector2Int(1, 0));
            Assert.AreEqual(results[0].MovedTo, new Vector2Int(1, 2));
        }
        
                
        [Test]
        public void RESULT_CHECK_MOVE_DOWN_2_PIECE()
        {
            var arr = new int[,]
            {
                { 0, 8, 6 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeDown();
            Assert.AreEqual(results.Count, 2);
            Assert.AreEqual(results[0].MovedFrom, new Vector2Int(1, 0));
            Assert.AreEqual(results[0].MovedTo, new Vector2Int(1, 2));
            
            Assert.AreEqual(results[1].MovedFrom, new Vector2Int(2, 0));
            Assert.AreEqual(results[1].MovedTo, new Vector2Int(2, 2));
        }
        
        [Test]
        public void RESULT_CHECK_MOVE_UP_2_PIECE()
        {
            var arr = new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 8, 6, 0 },
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeUp();
            Assert.AreEqual(results.Count, 2);
        }
        
        [Test]
        public void RESULT_CHECK_LEFT()
        {
            var arr = new int[,]
            {
                { 0, 8, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeLeft();
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].MovedFrom, new Vector2Int(1, 0));
            Assert.AreEqual(results[0].MovedTo, new Vector2Int(0, 0));
        }
        
                
        [Test]
        public void RESULT_CHECK_RIGHT()
        {
            var arr = new int[,]
            {
                { 0, 8, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
            
            arr.Transpose();
            
            _grid.InitGrid(arr);
            var results = _grid.ComputeRight();
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].MovedFrom, new Vector2Int(1, 0));
            Assert.AreEqual(results[0].MovedTo, new Vector2Int(2, 0));
        }
        
        [Test]
        public void RESULT_CHECK_CORRECT_AMOUNT_OF_CHANGES()
        {
            var arr = new int[,]
            {
                { 2, 4, 4, 16 },
                { 0, 0, 0, 8 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
            };
            
            _grid.InitGrid(arr);
            var changes = _grid.ComputeRight();
            Assert.AreEqual(5, changes.Count);
            
            
        }
    }
}