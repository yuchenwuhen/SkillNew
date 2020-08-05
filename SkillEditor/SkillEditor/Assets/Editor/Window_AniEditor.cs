﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Window_AniEditor : EditorWindow
{
    int m_curframe = 0;

    bool m_lockFrame = false;
    bool m_frameRecord = false;

    int m_toolsBar = 0;

    private void OnGUI()
    {
        if (editor == null || editor.aniInEdit == null)
        {
            EditorGUILayout.HelpBox("无效信息", MessageType.Warning);
            return;
        }

        string label = editor.aniInEdit.name;
        GUILayout.BeginHorizontal();
        GUILayout.Label("PosePlus编辑:" + label + ",记得多多 Ctrl + S");
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(editor.aniInEdit);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_frameRecord = GUILayout.Toggle(m_frameRecord, "自动记录动画");

        m_lockFrame = GUILayout.Toggle(m_lockFrame, "锁定帧");

        GUILayout.EndHorizontal();

        string[] toolsStr = new string[] { "特效编辑","声音编辑" };
        m_toolsBar = GUILayout.Toolbar(m_toolsBar, toolsStr);

        Layout_DrawSeparator(Color.white);

        GUI_AniPos();

    }

    /// <summary>
    /// 绘制填充框
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

    #region 动作编辑模块

    int curframe = 0;
    Vector2 anipos = Vector2.zero;

    void SetFrame(int f, bool force = false)
    {
        if (m_lockFrame) return;

        if (curframe != f || force)
        {
            aniPlayer.SetPose(editor.aniInEdit, f, true);
            curframe = f;
        }
    }

    private void GUI_AniPos()
    {
        GUILayout.Label("Animation pos:(" + curframe + "/" + editor.aniInEdit.aniFrameCount + ")");

        int nf = (int)GUILayout.HorizontalScrollbar(curframe, 1, 0, editor.aniInEdit.aniFrameCount);
        anipos = EditorGUILayout.BeginScrollView(anipos, true, false, GUILayout.Height(230));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
        {
            var obc = GUI.backgroundColor;
            if (curframe == i)
            {
                GUI.backgroundColor = Color.green;
            }
            GUILayout.BeginVertical(GUILayout.Width(1));

            var oc = GUI.color;
            string txt = "F";
            if (editor.aniInEdit.frames[i].key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                txt = "K";

            }
            GUILayout.Label(i.ToString());
            if (GUILayout.Button(txt))
            {
                nf = i;
            }
            //关键帧下需要条垂直的空格
            if (editor.aniInEdit.frames[i].key)
            {
                GUILayout.Space(123);
            }
            GUI.color = oc;
            if (editor.aniInEdit.frames[i].key == false)
            {//调整曲线
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                float lerp = GUILayout.VerticalScrollbar(editor.aniInEdit.frames[i].lerp, 0.01f, 1.0f, 0, GUILayout.Height(120));
                if (lerp != editor.aniInEdit.frames[i].lerp)
                {
                    editor.aniInEdit.frames[i].lerp = lerp;
                    editor.aniInEdit.CalcLerpFrameOne(i);
                    if (i == curframe)
                    {
                        SetFrame(curframe, true);
                        EditorUtility.SetDirty(editor.aniInEdit);
                    }
                }
                GUILayout.EndHorizontal();
            }
            //box帧按钮
            string b_txt = "F";
            if (editor.aniInEdit.frames[i].box_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                b_txt = "K";

            }
            if (GUILayout.Button(b_txt))
            {

                nf = i;
            }


            GUI.color = oc;

            string d_txt = "○";
            if (editor.aniInEdit.frames[i].dot_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                d_txt = "●";

            }
            if (GUILayout.Button(d_txt))
            {

                nf = i;
            }
            GUILayout.EndVertical();
            GUI.color = oc;
            GUI.backgroundColor = obc;


        }

        SetFrame(nf);

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    #endregion

    #region 绘画辅助函数等

    AniPlayer aniPlayer;
    public static void Show(Dev_AniEditor editor)
    {
        //获取现有的打开窗口或如果没有，创建一个新的
        var window = EditorWindow.GetWindow<Window_AniEditor>(false, "AniExEditor");
        window.SelfInit(editor);
    }

    Dev_AniEditor editor = null;
    void SelfInit(Dev_AniEditor editor)
    {
        aniPlayer = editor.GetComponent<AniPlayer>();
        this.editor = editor;
        m_curframe = 0;
        m_lockFrame = false;
        editor.aniInEdit.frames[0].key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].key = true;
        editor.aniInEdit.frames[0].box_key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].box_key = true;
        editor.aniInEdit.frames[0].dot_key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].dot_key = true;

    }

    #endregion
}
