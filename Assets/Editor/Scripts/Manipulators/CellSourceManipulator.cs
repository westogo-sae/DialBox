using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

namespace Westhouse.DialBox
{

    public class CellSourceManipulator : PointerManipulator
    {
        public CellType cellType {get; private set;}
        private DiagGridManipulator grid;
        public CellSourceManipulator(VisualElement target, DiagGridManipulator grid)
        {
            cellType = (CellType)GetCellFromName(target.name);
            this.grid = grid;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }


        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }



        private void OnPointerDown(PointerDownEvent evt)
        {
            //Create new cell of cellType
            DiagCellManipulator newCell = SpawnNewCellofType();

            Vector2 localPos = target.localBound.position;
            Vector2 worldPos = target.parent.LocalToWorld(localPos);
        
            newCell.forceDragFrom(evt, worldPos);

            VisualElement nestedCell = newCell.target.Children().First();
            // nestedCell.style.width = Length.Percent(100);
            // nestedCell.style.height = Length.Percent(100);
            grid.cells.Add(newCell);

            evt.StopPropagation();
        }

        private DiagCellManipulator SpawnNewCellofType()
        {
            VisualElement newCell = target.Children().First().visualTreeAssetSource.Instantiate();

            DiagCellManipulator manipulator = new DiagCellManipulator(newCell, grid);
            newCell.AddManipulator(manipulator);
            grid.target.Add(newCell);

            manipulator.cellType = cellType;
            
            manipulator.Repaint();

            return manipulator;
        }

        private int GetCellFromName(string cellName)
        {
            cellName = cellName.ToLower();
            switch(cellName)
            {
                case "dialogue": return 0;
                case "group": return 1;
                case "style": return 2;
                case "reroute": return 3;
                case "spacer": return 4;
                default: return -1;
            }
        }
    }

    


    public enum CellType
    {
        Dialogue,
        Group,
        Style,
        Reroute,
        Spacer,
    }
}