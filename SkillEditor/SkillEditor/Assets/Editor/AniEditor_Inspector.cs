using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dev_AniEditor))]
public class AniEditor_Inspector : Editor
{

    public override void OnInspectorGUI()
    {
        if (this.target == null) return;
        //base.OnInspectorGUI();
        //if (Application.isPlaying) return;
        var con = target as Dev_AniEditor;
        con.aniInEdit = EditorGUILayout.ObjectField("EditAni", con.aniInEdit, typeof(AniClip), true) as AniClip;
        var player = con.GetComponent<AniPlayer>();
        if (player != null)
            if (player.clips.Contains(con.aniInEdit) == false)
            {
                con.aniInEdit = null;
            }
        if (con.aniInEdit == null)
        {
            EditorGUILayout.HelpBox("选择一个动画,必须来自AniPlayer的Clip中的动画", MessageType.Info);
        }
        else
        {
            if (GUILayout.Button("打开编辑动画Window"))
            {
                Window_AniEditor.Show(con);
            }
        }
    }
}
