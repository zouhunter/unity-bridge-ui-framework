//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(Transform))]
//public class ArrowExampleEditor : Editor
//{
//    float size = 1f;

//    protected virtual void OnSceneGUI()
//    {
//        if (Event.current.type == EventType.Repaint)
//        {
//            Transform transform = ((Transform)target).transform;
//            Handles.color = Handles.xAxisColor;
//            Handles.ArrowHandleCap(
//                0,
//                transform.position + new Vector3(3f, 0f, 0f),
//                transform.rotation * Quaternion.LookRotation(Vector3.right),
//                size,
//                EventType.Repaint
//                );
//            Handles.color = Handles.yAxisColor;
//            Handles.ArrowHandleCap(
//                0,
//                transform.position + new Vector3(0f, 3f, 0f),
//                transform.rotation * Quaternion.LookRotation(Vector3.up),
//                size,
//                EventType.Repaint
//                );
//            Handles.color = Handles.zAxisColor;
//            Handles.ArrowHandleCap(
//                0,
//                transform.position + new Vector3(0f, 0f, 3f),
//                transform.rotation * Quaternion.LookRotation(Vector3.forward),
//                size,
//                EventType.Repaint
//                );
//        }
//    }
//}