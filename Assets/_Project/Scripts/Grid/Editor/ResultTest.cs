using NUnit.Framework;
using UnityEngine;

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
        
        
    }
}