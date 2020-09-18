using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SkillEditorData
{
    public SkillEventAttribute attr;
    public Type classData;
}

public class SkillEditorWindow : EditorWindow
{
    private Dictionary<string, SkillEditorData> m_skillEditorDict;

    private AniPlayer m_aniPlayer;

    private Skills m_skill;

    public void Show(AniPlayer ani, Skills skill)
    {
        this.m_skill = skill;
        this.m_aniPlayer = ani;

        if (this.m_skill.SkillList == null)
        {
            this.m_skill.SkillList = new List<Skill>();
        }

        m_skillEditorDict = EditorSkillTool.GetSKillEditorDict();
        this.Show();
    }

    private void OnGUI()
    {
        GUI.SetNextControlName("RefreshFocus");
        GUILayout.TextField("", GUILayout.Width(0), GUILayout.Height(0));
        GUILayout.BeginHorizontal(GUILayout.Height(800));
        {
            OnGUI_DrawSkillGroup();
            OnGUI_DrawSkillBlock();
            //
            GUILayout.BeginVertical();
            {
                OnGUI_DrawAniFrame();
                OnGUI_DrawSkillEventWindow();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    #region Skill列表渲染

    private int m_curSkillIndex = -1;
    private Skill m_curSkill = null;

    /// 当前选中技能事件的下标
    private int m_curSkillEventIndex = -1;

    //当前编辑器
    private ISkillEventEditor m_curSkillEventEditor = null;

    private void OnGUI_DrawSkillGroup()
    {
        if (m_skill == null) return;

        GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.Height(800));
        {
            var oc = GUI.backgroundColor;
            GUI.color = Color.yellow;
            if (GUILayout.Button("保存", GUILayout.Width(100), GUILayout.Height(30)))
            {
                EditorUtility.SetDirty(m_skill);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.backgroundColor = oc;
        }
        GUILayout.Label("技能列表:");
        int count = m_skill.SkillList.Count;
        for (int i = 0; i < count; i++)
        {
            if (m_curSkillIndex == i)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }

            var s = m_skill.SkillList[i];
            s.Id = i + 1;
            GUILayout.BeginHorizontal(); //每一个技能的横条
            {
                if (GUILayout.Button(s.Id.ToString()))
                {
                    m_curSkillIndex = i;
                    m_curSkill = s;
                    this.m_curSkillblockList = m_curSkill.Blocks;

                    m_curSkillblockIndex = -1;
                    m_curSkillEventIndex = -1;
                    m_curSkillblock = null;
                    m_curSkillEventList = null;
                    m_curAniClip = null;
                    m_curSkillEventEditor = null;
                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;

                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    m_skill.SkillList.Remove(s);
                    m_curSkillIndex = -1;
                    m_curSkillblockIndex = -1;
                    m_curSkillEventIndex = -1;
                    m_curSkillblock = null;
                    m_curSkillEventList = null;
                    m_curAniClip = null;
                    m_curSkillEventEditor = null;
                    m_curSkillblockList = null;
                    count = m_skill.SkillList.Count;

                    GUI.FocusControl("RefreshFocus");
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("创建skill"))
        {
            if (m_skill.SkillList == null || m_skill.SkillList.Count == 0)
            {
                m_skill.SkillList = new List<Skill>();
            }

            var s = new Skill();
            m_skill.SkillList.Add(s);
        }

        GUILayout.EndVertical();

        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    #endregion

    #region Skillblock

    private int m_curSkillblockIndex = -1;

    //当前技能块列表
    private List<SkillBlock> m_curSkillblockList = null;

    /// <summary>
    /// 当前技能块
    /// </summary>
    private SkillBlock m_curSkillblock = null;

    /// <summary>
    /// 当前技能block
    /// </summary>
    private void OnGUI_DrawSkillBlock()
    {
        GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.Height(800));

        GUILayout.Label("技能块列表:");
        if (m_curSkillblockList != null)
        {
            int count = m_curSkillblockList.Count;
            for (int i = 0; i < count; i++)
            {
                GUILayout.BeginVertical();
                if (m_curSkillblockIndex == i)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                var sb = this.m_curSkillblockList[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(string.Format("[{0}] -" + sb.AniName, i)))
                {
                    m_curSkillblockIndex = i;
                    m_curAniClip = m_aniPlayer.GetClip(sb.AniName);
                    m_curSkillblock = sb;
                    m_curSkillEventList = EditorSkillTool.GetCurFrameEventList(this.m_curframe, m_curSkillblock);
                    m_curSkillEventIndex = -1;
                    m_curSkillEventEditor = null;
                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;
                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    this.m_curSkillblockList.Remove(sb);
                    count = m_curSkillblockList.Count;
                    m_curSkillblockIndex = -1;
                    m_curSkillblock = null;
                    m_curSkillEventList = null;
                    m_curSkillEventIndex = -1;
                    m_curAniClip = null;
                    m_curSkillEventEditor = null;

                    GUI.FocusControl("RefreshFocus");
                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }

        GUILayout.Space(20);
        if (m_curSkillblockList != null)
        {
            if (GUILayout.Button("创建skillBlock"))
            {
                SelectAniClipEditorWindow window =
                    (SelectAniClipEditorWindow)EditorWindow.GetWindow(typeof(SelectAniClipEditorWindow), false, "SelectAniClip");
                window.Show(this.m_aniPlayer, this.m_curSkillblockList);
            }
        }

        GUILayout.EndVertical();

        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    #endregion

    #region 动画帧渲染

    private int m_curframe = 0;

    private Vector2 m_anipos;

    //当前选择的动画片段
    private AniClip m_curAniClip = null;

    private void OnGUI_DrawAniFrame()
    {
        GUILayout.Label("动画播放器:");
        var ani = m_curAniClip;
        m_anipos = EditorGUILayout.BeginScrollView(m_anipos, true, false, GUILayout.Height(230));
        if (m_curAniClip == null)
        {
            EditorGUILayout.EndScrollView();
            return;
        }

        GUILayout.Label("Animation pos:(" + m_curframe + "/" + ani.aniFrameCount + ")");
        int nf = (int)GUILayout.HorizontalScrollbar(m_curframe, 1, 0, ani.aniFrameCount);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < ani.aniFrameCount; i++)
        {
            var obc = GUI.backgroundColor;
            if (m_curframe == i)
            {
                GUI.backgroundColor = Color.green;
            }

            GUILayout.BeginVertical(GUILayout.Width(1));

            var oc = GUI.color;
            string txt = "F";
            if (ani.frames[i].key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                txt = "K";
            }

            GUILayout.Label(i.ToString());
            if (GUILayout.Button(txt))
            {
                nf = i;
                GUI.FocusControl("RefreshFocus");
            }

            GUI.color = oc;

            List<SkillEvent> tmp = EditorSkillTool.GetCurFrameEventList(i, m_curSkillblock);
            if (tmp.Count == 0 || tmp == null)
            {
                GUILayout.Space(60);
            }
            else
            {
                GUI.color = new Color(1.0f, 1f, 0f, 1.0f);
                GUILayout.Space(39);
                if (GUILayout.Button("e"))
                {
                    nf = i;
                    GUI.FocusControl("RefreshFocus");
                }
            }

            //关键帧下需要条垂直的空格
            if (ani.frames[i].key)
            {
                GUILayout.Space(60);
            }

            GUI.color = oc;
            if (ani.frames[i].key == false)
            {
                //调整曲线
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                float lerp = GUILayout.VerticalScrollbar(ani.frames[i].lerp, 0.01f, 1.0f, 0, GUILayout.Height(120));
                if (lerp != ani.frames[i].lerp)
                {
                    ani.frames[i].lerp = lerp;
                    ani.CalcLerpFrameOne(i);
                    if (i == m_curframe)
                    {
                        SetFrame(m_curframe, true);
                        // EditorUtility.SetDirty(ani);
                    }
                }

                GUILayout.EndHorizontal();
            }

            //box帧按钮
            string b_txt = "F";
            if (ani.frames[i].box_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                b_txt = "K";
            }

            if (GUILayout.Button(b_txt))
            {
                nf = i;
                GUI.FocusControl("RefreshFocus");
            }


            GUI.color = oc;

            string d_txt = "○";
            if (ani.frames[i].dot_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                d_txt = "●";
            }

            if (GUILayout.Button(d_txt))
            {
                nf = i;
                GUI.FocusControl("RefreshFocus");
            }

            GUILayout.EndVertical();
            GUI.color = oc;
            GUI.backgroundColor = obc;
        }

        SetFrame(nf);

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    void SetFrame(int f, bool force = false)
    {
        if (m_curframe != f || force)
        {
            m_aniPlayer.SetPose(m_curAniClip, f, true);
            m_curframe = f;
            //动画编辑器 切换帧刷新curSkillEventList
            m_curSkillEventList = EditorSkillTool.GetCurFrameEventList(this.m_curframe, m_curSkillblock);
        }
    }

    #endregion

    #region SkillEvent 渲染

    // 当前选中的动画帧率
    private int m_curSelectClipFrame = 0;

    // 当前技能事件
    private List<SkillEvent> m_curSkillEventList;

    private void OnGUI_DrawSkillEventWindow()
    {
        GUILayout.BeginHorizontal();
        this.OnGUI_DrawSkillEvent();
        this.OnGUI_DrawSkillEventEditor();
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 当前技能帧事件
    /// </summary>
    private void OnGUI_DrawSkillEvent()
    {
        GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.Height(550));
        GUILayout.Label("技能事件列表:");
        if (m_curSkillEventList != null)
        {
            int count = m_curSkillEventList.Count;
            for (int i = 0; i < m_curSkillEventList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (m_curSkillEventIndex == i)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                var e = m_curSkillEventList[i];
                if (GUILayout.Button(string.Format("[{0}] " + e.EventName, i)))
                {
                    this.m_curSkillEventIndex = i;
                    //TODO 需要给Editor进行赋值
                    SkillEditorData data = m_skillEditorDict[e.EventName];
                    m_curSkillEventEditor =
                        (ISkillEventEditor)data.classData.Assembly.CreateInstance(data.classData.FullName);

                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;
                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    m_curSkillEventList.Remove(e);
                    m_curSkillblock.Events.Remove(e);
                    this.m_curSkillEventIndex = -1;
                    m_curSkillEventEditor = null;
                    count = m_curSkillEventList.Count;

                    GUI.FocusControl("RefreshFocus");
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(20);
        if (m_curSkillblock != null)
        {

            if (GUILayout.Button("添加事件"))
            {
                SelectSkillEventEditorWindow window =
                    (SelectSkillEventEditorWindow)EditorWindow.GetWindow(typeof(SelectSkillEventEditorWindow), false,
                        "选择创建skillevent");
                window.Show(this.m_curframe, this.m_curSkillblock, this.m_curSkillEventList);
            }
        }

        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    /// <summary>
    /// 当前技能帧事件编辑
    /// </summary>
    private void OnGUI_DrawSkillEventEditor()
    {
        if (m_curSkillEventEditor == null || m_curSkillEventList == null || m_curSkillEventIndex == -1 ||
            m_curSkillEventList.Count == 0) return;
        GUILayout.BeginVertical();
        var se = m_curSkillEventEditor.OnGuiEditor(this.m_curSkillEventList[m_curSkillEventIndex]);
        GUILayout.EndVertical();

        this.m_curSkillEventList[m_curSkillEventIndex] = se;
    }

    #endregion
}
