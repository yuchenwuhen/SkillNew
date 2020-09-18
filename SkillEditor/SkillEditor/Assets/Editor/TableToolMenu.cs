using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TableToolMenu
{

    /// <summary>
    /// 绘制横填充框
    /// —
    /// </summary>
    /// <param name="color"></param>
    /// <param name="height"></param>
    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }

    /// <summary>
    /// 绘制垂直填充框
    /// |
    /// </summary>
    /// <param name="color"></param>
    /// <param name="width"></param>
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }
}
