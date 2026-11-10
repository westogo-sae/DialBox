using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialBoxEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tools/DialBox")]
    public static void ShowExample()
    {
        DialBoxEditor wnd = GetWindow<DialBoxEditor>();
        wnd.titleContent = new GUIContent("DialBoxEditor");
    }

    private DiagGridManipulator gridManipulator;
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        AssignManipulators(root);

        gridManipulator.Repaint();


    }

    



    private void AssignManipulators(VisualElement root)
    {
        //Cell Manipulators
        List<VisualElement> cells = root.Query<VisualElement>(className: "cell").ToList();

        foreach(var cell in cells)
        {
            cell.AddManipulator(new DiagCellManipulator(cell));
        }

        // Grid Manipulator
        VisualElement grid = root.Query(name: "grid");
        gridManipulator = new DiagGridManipulator(grid);
        grid.AddManipulator(gridManipulator);
    }
}
