using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DiagGridManipulator : PointerManipulator
{

    private Vector2 offset;
    //private Vector2 maxOffset;
    private float scale = 1;
    int cellHeight = 50;
    int cellWidth = 75;
    float zoomMult = 0.05f;
    float maxZoom = 4;
    float minZoom = 0.5f;
    GridLinesElement gridLines;

    public DiagGridManipulator(VisualElement target)
    {
        this.target = target;
        //initialize lines
        gridLines = new GridLinesElement(2, new Color(0.8f, 0.8f, 0.8f, 0.5f), new Vector2(cellWidth, cellHeight));
        target.Add(gridLines);
        Debug.Log(gridLines);
        Repaint();
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
        
        Repaint();
    }

    public void Repaint()
    {
        gridLines.Repaint(offset, scale);
    }
}
