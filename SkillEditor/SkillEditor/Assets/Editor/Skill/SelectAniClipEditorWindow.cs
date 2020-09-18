using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectAniClipEditorWindow : EditorWindow
{
    private AniPlayer m_aniPlayer;
    private List<SkillBlock> m_skillBlocks;

    public void Show(AniPlayer ani,List<SkillBlock> blocks)
    {
        m_aniPlayer = ani;
        m_skillBlocks = blocks;
        this.Show();
    }

    private void OnGUI()
    {
        //GUILayout.BeginHorizontal();
        this.ShowAniclipList();
        //this.ShowCurFrame();
        //GUILayout.EndHorizontal();
    }

    private Vector2 pos = Vector2.zero;

    private void ShowAniclipList()
    {
        pos = GUILayout.BeginScrollView(pos, GUILayout.Width(320), GUILayout.Height(300));
        GUILayout.BeginVertical();
        foreach (AniClip clip in this.m_aniPlayer.clips)
        {
            this.DrawAniClipItem(clip);
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);

    }

    private AniClip m_curClip;

    private void DrawAniClipItem(AniClip clip)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        if (this.m_curClip != null)
        {
            if (this.m_curClip.name == clip.name)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }
        }

        if (GUILayout.Button(clip.name))
        {
            this.m_curClip = clip;
        }

        GUI.color = GUI.backgroundColor;
        if (GUILayout.Button("确定", GUILayout.Width(100)))
        {
            SkillBlock sb = new SkillBlock();
            sb.AniName = clip.name;
            if (this.m_skillBlocks == null) this.m_skillBlocks = new List<SkillBlock>();
            this.m_skillBlocks.Add(sb);
            this.Close();
        }

        GUILayout.EndHorizontal();
    }

}
