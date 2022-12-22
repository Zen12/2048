using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Grid
{
    public class GridView : InjectedMono
    {
        [In] private GridController _grid;

        [SerializeField] private Vector2Int _gridSize;

        public override void OnSyncStart()
        {
            _grid.InitGrid(_gridSize.x, _gridSize.y);
            ApplyGridChange(_grid.AddPieceToRandomPlace());
            ApplyGridChange(_grid.AddPieceToRandomPlace());
        }

        public void ApplyGridChange(GridChange change)
        {
            
        }
    }
}
