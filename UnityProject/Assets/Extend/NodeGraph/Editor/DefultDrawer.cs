using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
namespace NodeGraph
{ 
    public class DefultDrawer
{
    protected Dictionary<object, List<FieldInfo>> fieldDic;
    protected Dictionary<object, bool> toggleDic;
    protected void OnInspectorGUI(object target)
    {
        if (fieldDic == null)
        {
            fieldDic = new Dictionary<object, List<FieldInfo>>();
            toggleDic = new Dictionary<object, bool>();
            UserDefineUtility.GetNeedSerializeField(target, toggleDic, fieldDic);
        }
        UserDefineUtility.DrawClassObject(target, toggleDic, fieldDic);
    }
}
}