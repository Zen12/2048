using System;
using _Project.Scripts.Adapters;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Grid
{
    public class GridInputUpdater : InjectedMono
    {
        [In] private GridController _grid;


    }
}
