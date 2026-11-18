using UnityEditor;
using UnityEngine;

namespace Westhouse.DialBox
{

    public class DialBoxInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open In Editor"))
            {
                DialBoxEditor wnd = EditorWindow.GetWindow<DialBoxEditor>();
                wnd.titleContent = new GUIContent(target.name);
                wnd.Load(target as DialBox);
            }
        }
    }
}