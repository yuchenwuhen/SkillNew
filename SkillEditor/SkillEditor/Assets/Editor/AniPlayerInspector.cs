using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AniPlayer))]
public class AniPlayerInspector : Editor
{
    //bool bPlay = false;

    DateTime m_last = DateTime.Now;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var con = target as AniPlayer;

        if (con == null) return;
        if (con.clips == null) con.clips = new List<AniClip>();

        {
            foreach (var c in con.clips)
            {
                if (c == null) continue;
                GUILayout.BeginHorizontal();
                if (c.frames == null) continue;
                GUILayout.Label(c.name + "(" + (c.loop ? "loop" : "") + c.frames.Count + ")");
                if (GUILayout.Button("play", GUILayout.Width(50)))
                {
                    con.Play(c);
                    con.editorPlay = true;
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.BeginVertical();
                foreach (var sub in c.subclips)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(sub.name + (sub.loop ? "[Loop]" : "") + "(" + (sub.endframe - sub.startframe + 1) + ")");
                    if (GUILayout.Button("play", GUILayout.Width(100)))
                    {

                        con.Play(c, sub);
                        con.editorPlay = true;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

            }
        }

        if (con.editorPlay)
        {
            DateTime _now = DateTime.Now;
            float delta = (float)(_now - m_last).TotalSeconds;
            m_last = _now;
            con.OnUpdate(delta);
            Repaint();
        }
    }

}
