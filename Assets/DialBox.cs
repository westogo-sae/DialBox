using System;
using UnityEngine;

namespace Westhouse.DialBox
{
    [CreateAssetMenu(fileName = "DialBox", menuName = "Scriptable Objects/DialBox")]
    public class DialBox : ScriptableObject
    {
        
        public DialNode[] nodes;


        public void Recompile()
        {
            throw new NotImplementedException();
        }
    }
    

    [Serializable]
    public class DialNode
    {
        public Vector2Int gridIndex;
        public string name;
    }

    // public class CompiledDialNode
    // {
        
    // }

}
