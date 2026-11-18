using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Westhouse.DialBox
    {

    public class DiagCellManipulator : PointerManipulator
    {

        public CellType cellType;
        DiagGridManipulator grid;
        public Vector2Int gridIndex;
        bool placed;
        public DiagCellManipulator(VisualElement target, DiagGridManipulator grid)
        {
            this.target = target;
            this.grid = grid;
            target.AddToClassList("cell");
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);

        }


        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        bool dragging;
        public void forceDragFrom(PointerDownEvent evt, Vector2 origin) 
        { 
            OnPointerDown(evt);
            elementStartPos = origin;

            //update Position
            Vector2 newPos = target.parent.WorldToLocal(origin);

            target.style.left = newPos.x;
            target.style.top = newPos.y;
        }
        Vector2 pointerStartPos;
        Vector2 elementStartPos;
        [SerializeField] int buttonID = 0;

        private void OnPointerDown(PointerDownEvent evt)
        {
            { // Drag and Drop func

                if(evt.button != buttonID)
                return;

                //switch to absolute
                target.style.position = Position.Absolute;
                dragging = true;

                pointerStartPos = evt.position;
                elementStartPos = target.worldBound.position;

                target.BringToFront();

                //ensures callbacks still occur when pointer is out of bounds;
                target.CapturePointer(evt.pointerId);

                //prevents event trickle down
                evt.StopPropagation();
            }
        }
        private void OnPointerUp(PointerUpEvent evt)
        {
            { // Drag and Drop func
            
                if(!dragging || !target.HasPointerCapture(evt.pointerId)) return;

                target.ReleasePointer(evt.pointerId);

                //prevents event trickle down
                evt.StopPropagation();
            }

        }
        private void OnPointerMove(PointerMoveEvent evt)
        {
            { // Drag and Drop func
            
                if(!dragging || !target.HasPointerCapture(evt.pointerId)) return;

                VisualElement parent = target.parent;
                if(parent == null) return;

                Vector2 pointerDelta = (Vector2)evt.position - pointerStartPos;

                Vector2 newPos = parent.WorldToLocal(elementStartPos + pointerDelta);

                target.style.left = newPos.x;
                target.style.top = newPos.y;

                //prevents event trickle down
                evt.StopPropagation();
            }
        }

        //called when pointer is released or lost
        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            dragging = false;
            Vector2 centerPos = target.worldBound.position + new Vector2(grid.cellWidth, grid.cellHeight) * 0.5f * grid.scale;

            if(grid.InBounds(centerPos))
            {
                //snap to grid
                Vector2Int newIndex = grid.LocalToGrid(target.parent.WorldToLocal(centerPos));

                if(grid.IndexOccupied(newIndex))
                {
                    if(!placed)
                        DeleteCell();
                        
                }else
                {
                    placed = true;
                    gridIndex = newIndex;
                }

                Repaint();
            }else
                DeleteCell();
            
        }
        public void DeleteCell()
        {
            grid.cells.Remove(this);
            target.RemoveFromHierarchy();
        }

        public void Repaint()
        {

            Vector2 newPos = grid.GridToLocal(gridIndex);
            
            target.style.left = newPos.x - grid.cellWidth * 0.5f * grid.scale;
            target.style.top = newPos.y - grid.cellHeight * 0.5f * grid.scale;

            target.style.width = grid.cellWidth * grid.scale;
            target.style.height = grid.cellHeight * grid.scale;
        }
        
    }
}