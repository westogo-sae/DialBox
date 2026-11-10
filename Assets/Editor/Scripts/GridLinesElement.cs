using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridLinesElement : VisualElement
{
    float width;
    Color color;
    Vector2 offset;
    Vector2 cellSize;
    float scale;
    public GridLinesElement(float width, Color color, Vector2 cellSize)
    {
        generateVisualContent += OnGenerateVisualContent;
        this.width = width;
        this.color = color;
        this.cellSize = cellSize;
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Painter2D painter = mgc.painter2D;
    

        painter.strokeColor = color;
        painter.lineWidth = width;


        painter.BeginPath();

        //Vertical Lines

        float scan = (offset.x%cellSize.x - cellSize.x * 0.5f) * scale; // slight offset for odd offsets

        while(scan < 0)
            scan += cellSize.x * scale; // avoid pre-scan

        while(scan < contentRect.width)
        {
            //draw line
            painter.MoveTo(new Vector2(scan, 0));
            painter.LineTo(new Vector2(scan, contentRect.height));
            scan += cellSize.x * scale;
        }

        //Horizontal Lines

        scan = (offset.y%cellSize.y - cellSize.y * 0.5f) * scale; // slight offset for odd offsets


        while(scan < 0)
            scan += cellSize.y * scale;

        while(scan < contentRect.height)
        {
            //draw line
            painter.MoveTo(new Vector2(0, scan));
            painter.LineTo(new Vector2(contentRect.width, scan));
            scan += cellSize.y * scale;
        }

        //painter.ClosePath();
        painter.Stroke();
    }

    public void Repaint(Vector2 gridOffset, float gridScale)
    {
        offset = gridOffset;
        scale = gridScale;
        MarkDirtyRepaint();
    }
}
