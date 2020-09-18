using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillPlayer))]
public class SkillPlayerEditor_Inspector : Editor
{
    public SkillPlayer skillPlayer = null;

    private AniPlayer m_aniPlayer;

    private SkillEditorWindow m_window;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (skillPlayer == null)
        {
            skillPlayer = target as SkillPlayer;
        }

        if (m_aniPlayer == null)
        {
            m_aniPlayer = this.skillPlayer.transform.GetComponent<AniPlayer>();
        }

        if (this.skillPlayer.GetComponent<AniPlayer>() == null)
        {
            EditorGUILayout.HelpBox("添加AniPlayer", MessageType.Error);
            return;
        }

        if (skillPlayer.skills == null)
        {
            if (GUILayout.Button("创建skill数据"))
            {
                var skill = ScriptableObject.CreateInstance<Skills>();

                var cp = AssetDatabase.GetAssetPath(m_aniPlayer.clips[0].GetInstanceID());

                string path = System.IO.Path.GetDirectoryName(cp);

                string outPath = path + "/" + skillPlayer.name + ".Skills.asset";

                AssetDatabase.CreateAsset(skill, outPath);

                var src = AssetDatabase.LoadAssetAtPath(outPath, typeof(Skills)) as Skills;

                this.skillPlayer.skills = src;
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            if (GUILayout.Button("打开"))
            {
                this.m_window = (SkillEditorWindow)EditorWindow.GetWindow(typeof(SkillEditorWindow), false, "技能编辑器");
                m_window.Show(this.m_aniPlayer,this.skillPlayer.skills);
            }
        }
    }
}
