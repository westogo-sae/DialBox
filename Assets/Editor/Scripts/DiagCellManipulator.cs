using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DiagCellManipulator : PointerManipulator
{

    public DiagCellManipulator(VisualElement target)
    {
        this.target = target;
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
    Vector2 pointerStartPos;
    Vector2 elementStartPos;
    [SerializeField] int buttonID = 1;

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
    }
    
}
