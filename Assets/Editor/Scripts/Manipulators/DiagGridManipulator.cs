using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Westhouse.DialBox
    {

    public class DiagGridManipulator : PointerManipulator
    {

        private Vector2 offset;
        //private Vector2 maxOffset;
        public float scale {get; private set;} = 1;
        public float cellHeight {get; private set;} = 60;
        public float cellWidth {get; private set;} = 120;
        float zoomMult = 0.05f;
        float maxZoom = 4;
        float minZoom = 0.5f;
        GridLinesElement gridLines;
        Label snapDebug;
        //Random Access reference to all cells created
        public List<DiagCellManipulator> cells;

        public DiagGridManipulator(VisualElement target)
        {
            this.target = target;
            //initialize lines
            gridLines = new GridLinesElement(2, new Color(0.8f, 0.8f, 0.8f, 0.5f), new Vector2(cellWidth, cellHeight));
            target.Add(gridLines);
            cells = new();
            Repaint();

            snapDebug = new Label();
            snapDebug.style.position = Position.Absolute;
            snapDebug.style.width = cellWidth;
            snapDebug.style.height = cellHeight;
            snapDebug.style.alignContent = Align.Center;
            snapDebug.style.justifyContent = Justify.Center;
            snapDebug.style.display = DisplayStyle.Flex;
            snapDebug.style.color = new Color(0.8f, 0.3f, 0.3f);

            target.Add(snapDebug);
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.RegisterCallback<WheelEvent>(OnWheel);
        }

        

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.UnregisterCallback<WheelEvent>(OnWheel);
        }

        

    bool dragging;

        public void OnPointerDown(PointerDownEvent evt)
        {
            if(evt.button != 2) // middle mouse
                return;
            
            dragging = true;
            target.CapturePointer(evt.pointerId);
            //evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if(evt.button != 2)
                return;

            target.ReleasePointer(evt.pointerId);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            //Debug the current grid position of the mouse
            Vector2Int mouseGridPos = LocalToGrid(evt.localPosition);
            snapDebug.text = $"{mouseGridPos.x} , {mouseGridPos.y}";
            Vector2 snappedPos = GridToLocal(mouseGridPos);
            snapDebug.style.left = snappedPos.x - cellWidth * scale * 0.5f;
            snapDebug.style.top = snappedPos.y - cellHeight * scale * 0.5f;


            if(!dragging || !target.HasPointerCapture(evt.pointerId))
                return;

            offset += (Vector2)evt.deltaPosition / scale;
            //Debug.Log(offset);

            Repaint();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            dragging = false;
        }

        private void OnWheel(WheelEvent evt)
        {
            //scale from mouse centre, by scalling offset?
            float zoom = evt.delta.y *zoomMult;
            Vector2 mouseOffset = evt.localMousePosition / scale;
            scale = Mathf.Clamp(scale - zoom, minZoom, maxZoom);
            Vector2 mouseOffset2 = evt.localMousePosition / scale;

            offset += mouseOffset2 - mouseOffset; // scale around mouse position
            snapDebug.style.width = cellWidth * scale;
            snapDebug.style.height = cellHeight * scale;
            
            Repaint();
        }

        /// <summary>
        /// convert position in local space to virtual space considering grid offset and scale
        /// </summary>
        private Vector2 LocalToVirtual(Vector2 localIn)
        {
            return localIn / scale - offset;
        }

        private Vector2 VirtualToLocal(Vector2 virtualIn)
        {
            return (virtualIn + offset) * scale;
        }

        private Vector2Int SnapVirtual(Vector2 virtualIn)
        {
            int snapX = Mathf.RoundToInt(virtualIn.x / cellWidth);
            int snapY = Mathf.RoundToInt(virtualIn.y / cellHeight);
            return new Vector2Int(snapX, snapY);
        }

        public Vector2Int LocalToGrid(Vector2 localIn)
        {
            return SnapVirtual(LocalToVirtual(localIn));
        }

        public Vector2 GridToLocal(Vector2Int gridIndex)
        {
            float virtualX = gridIndex.x * cellWidth;
            float virtualY = gridIndex.y * cellHeight;
            
            return VirtualToLocal(new Vector2(virtualX, virtualY));
        }

        public bool InBounds(Vector2 worldPos)
        {
            return target.worldBound.Contains(worldPos);
        }

        public bool IndexOccupied(Vector2Int index)
        {
            foreach(var cell in cells)
            {
                if (cell.gridIndex == index)
                    return true;
            }
            
            return false;
        }

        public void Repaint()
        {
            gridLines.Repaint(offset, scale);
            //repaint cell sizes
            foreach(var cell in cells)
            {
                cell.Repaint();
                Debug.Log(cell.gridIndex);
            }
        }
    }
}