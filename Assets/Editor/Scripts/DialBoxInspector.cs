using UnityEditor;
using UnityEngine;

namespace Westhouse.DialBox
{
    [CustomEditor(typeof(DialBox))]
    public class DialBoxInspector : Editor
    {
        public override void OnInspectorGUI()
        {   
            if(GUILayout.Button("Open In Editor", GUILayout.Height(200)))
            {
                DialBoxEditor wnd = EditorWindow.GetWindow<DialBoxEditor>();
                wnd.titleContent = new GUIContent(target.name);
                wnd.Load(target as DialBox);
            }
        }
    }
}