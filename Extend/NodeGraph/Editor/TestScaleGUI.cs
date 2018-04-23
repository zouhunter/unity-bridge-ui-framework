using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class TestScaleGUI : EditorWindow
{
    private MethodInfo _getTopRect = null;
    private PropertyInfo _getVisibleRect = null;
    private MethodInfo _setTransform = null;
    private Vector2 _scrPos = Vector2.zero;

    //Windowメニュー
    [MenuItem("Test/TestScaleGUI")]
    public static void Create()
    {
        /*var win = */GetWindow<TestScaleGUI>("TestScaleGUI");
    }

    private void OnEnable()
    {
        Assembly UnityEngine = System.Reflection.Assembly.Load("UnityEngine.dll");
        Type GUIClipType = UnityEngine.GetType("UnityEngine.GUIClip");
        _getTopRect = GUIClipType.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
        _getVisibleRect = GUIClipType.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
        _setTransform = GUIClipType.GetMethod("SetTransform", BindingFlags.Static | BindingFlags.NonPublic);
    }

    private void OnDisable()
    {
    }

    private Rect GUIClipGetTopRect()
    {
        if (_getTopRect == null)
            return new Rect(0, 0, position.width, position.height);
        var rect = (Rect)_getTopRect.Invoke(null, null);
        return rect;
    }

    private Rect GUIClipGetVisibleRect()
    {
        if (_getVisibleRect == null)
            return new Rect(0, 0, position.width, position.height);
        var rect = (Rect)_getVisibleRect.GetValue(null, null);
        return rect;
    }

    private void GUIClipSetTransform(Matrix4x4 clipTransform, Matrix4x4 objectTransform, Rect clipRect)
    {
        if (_setTransform == null)
            return;
        _setTransform.Invoke(null, new object[] { clipTransform, objectTransform, clipRect });
    }

    private void OnGUI()
    {
       /* var topRect =*/ GUIClipGetTopRect();

        var pivotPoint = Vector2.zero;
        //var rect = new Rect(0, 0, topRect.width, topRect.height);

        var zoom = 2.0f; // 1.0f～ 値が大きいほど、小さく表示される
        var invZoom = 1.0f / zoom;

        {
            using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scrPos))
            {
                _scrPos = scrollScope.scrollPosition;

                /*var area =*/ GUILayoutUtility.GetRect(new GUIContent(string.Empty), GUIStyle.none, GUILayout.Width(3000), GUILayout.Height(3000));
                {
                    GUIUtility.ScaleAroundPivot(Vector2.one * invZoom, pivotPoint);
                    var visibleRect = GUIClipGetVisibleRect();
                    visibleRect.position *= zoom;
                    visibleRect.size *= zoom;
                    GUIClipSetTransform(Matrix4x4.identity, Matrix4x4.identity, visibleRect);

                    //内部のGUI　適当！
                    int x, y;
                    x = 3000 - 25;
                    for (y = 0; y <= 6000; y += 100)
                        GUI.Button(new Rect(x, y, 50, 50), y.ToString());

                    x = 6000 - 25;
                    for (y = 0; y <= 6000; y += 100)
                        GUI.Button(new Rect(x, y, 50, 50), y.ToString());

                    y = 3000 - 25;
                    for (x = -25; x <= 6000; x += 100)
                        GUI.Button(new Rect(x, y, 50, 50), x.ToString());

                    y = 6000 - 25;
                    for (x = 0; x <= 6000; x += 100)
                        GUI.Button(new Rect(x, y, 50, 50), x.ToString());


                    GUI.matrix = Matrix4x4.identity;
                }
            }
        }
    }
}