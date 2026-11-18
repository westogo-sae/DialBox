using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Westhouse.DialBox
{

    public class DialBoxEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset = default;
        [SerializeField] private VisualTreeAsset cellTreeAsset = default;

        [MenuItem("Tools/DialBox")]
        public static void ShowExample()
        {
            DialBoxEditor wnd = GetWindow<DialBoxEditor>();
        }

        private DiagGridManipulator gridManipulator;
        private DialBox activeDialBox;
        //private fileDialogue
        public void CreateGUI()
        {
            Debug.Log(activeDialBox);
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);


            AssignManipulators(root);
            gridManipulator.Repaint();

            (root.Q(name:"save") as Button).clicked += Save;


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
            List<DialNode> newNodes = new();
            foreach(var cell in gridManipulator.cells)
            {
                newNodes.Add(cell.WriteNode());
            }
            activeDialBox.nodes = newNodes.ToArray();
        }

        public void Compile()
        {
            activeDialBox.Recompile();
        }
        public void Load(DialBox dialBox)
        {
            activeDialBox = dialBox;

            foreach(var cell in gridManipulator.cells)
            {
                gridManipulator.target.Remove(cell.target); // remove all old cell;
            }
            gridManipulator.cells = new();

            if(dialBox.nodes != null)
                foreach(var node in dialBox.nodes)
                {
                    VisualElement cellElement = cellTreeAsset.Instantiate();
                    DiagCellManipulator cellManipulator = new DiagCellManipulator(cellElement, gridManipulator);
                    cellManipulator.ReadNode(node);
                    
                    gridManipulator.cells.Add(cellManipulator);
                    gridManipulator.target.Add(cellElement);
                    
                }

            gridManipulator.Repaint();
        }
    }

}
