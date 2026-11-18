using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Westhouse.DialBox
{

    public class DialBoxEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Tools/DialBox")]
        public static void ShowExample()
        {
            DialBoxEditor wnd = GetWindow<DialBoxEditor>();
        }

        private DiagGridManipulator gridManipulator;
        //private fileDialogue
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

            // foreach(var cell in cells)
            // {
            //     cell.AddManipulator(new DiagCellManipulator(cell));
            // }
            // Grid Manipulator
            VisualElement grid = root.Query(name: "grid");
            gridManipulator = new DiagGridManipulator(grid);
            grid.AddManipulator(gridManipulator);

            List<VisualElement> sources = root.Query<VisualElement>(className: "cell-source").ToList();

            foreach(var source in sources)
            {
                source.AddManipulator(new CellSourceManipulator(source, gridManipulator));
            }
        }

        public void Save()
        {
            Debug.Log("TODO -> save");
        }
        public void Load(DialBox dialBox)
        {
            Debug.Log("TODO -> load");
        }
    }

}
