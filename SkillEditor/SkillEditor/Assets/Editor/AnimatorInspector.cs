using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Animator))]
public class AnimatorInspector : Editor
{

    Dictionary<string, float> m_aniDic = new Dictionary<string, float>();

    public override void OnInspectorGUI()
    {
        if (target == null)
        {
            return;
        }

        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            return;
        }

        EditorGUILayout.HelpBox("这里增加一个Inspector,用来检查动画和产生CleanData.Ani", MessageType.Info);

        var ani = target as Animator;
        AniPlayer con = ani.GetComponent<AniPlayer>();

        if (con == null)
        {
            if (GUILayout.Button("添加CleanData.Ani组件"))
            {
                con = ani.gameObject.AddComponent<AniPlayer>();
                ani.enabled = false;
            }
        }
        else
        {
            GUILayout.Label("已经有CleanData.Ani");

            List<AnimationClip> clips = new List<AnimationClip>();
            //if (GUILayout.Button("Update Clips"))
            //{
            UnityEditor.Animations.AnimatorController cc = ani.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (cc != null)
            {
                FindAllAniInControl(cc, clips);
                GUILayout.Label("拥有动画:" + clips.Count);
                //}
                foreach (var c in clips)
                {
                    if (c == null) continue;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(c.name + "(" + c.length * c.frameRate + ")");
                    if (GUILayout.Button("Create FBAniClip", GUILayout.Width(150)))
                    {
                        CloneAni(c, c.frameRate);
                    }
                    GUILayout.EndHorizontal();
                    if (m_aniDic.ContainsKey(c.name) == false)
                    {
                        m_aniDic[c.name] = 0;
                    }
                    float v = m_aniDic[c.name];
                    v = GUILayout.HorizontalSlider(v, 0, c.length);
                    if (v != m_aniDic[c.name])
                    {
                        m_aniDic[c.name] = v;
                        ani.Play(c.name, 0, v / c.length);
                        ani.updateMode = AnimatorUpdateMode.UnscaledTime;
                        ani.Update(0);
                    }
                }
            }
        }
    }

    void CloneAni(AnimationClip clip, float fps)
    {

        var ani = target as Animator;

        //创建CleanData.Ani
        AniClip _clip = ScriptableObject.CreateInstance<AniClip>();
        _clip.boneinfo = new List<string>();//也增加了每个动画中的boneinfo信息.

        //这里重新检查动画曲线，找出动画中涉及的Transform部分，更精确
        List<Transform> cdpath = new List<Transform>();
        EditorCurveBinding[] curveDatas = AnimationUtility.GetCurveBindings(clip);
        foreach (var dd in curveDatas)
        {
            Transform tran = ani.transform.Find(dd.path);
            if (cdpath.Contains(tran) == false)
            {
                _clip.boneinfo.Add(dd.path);
                cdpath.Add(tran);
            }
        }
        Debug.LogWarning("curve got path =" + cdpath.Count);


        string path = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(clip.GetInstanceID()));
        _clip.name = clip.name;
        _clip.frames = new List<Frame>();
        _clip.fps = fps;
        _clip.loop = clip.isLooping;
        float flen = (clip.length * fps);
        int framecount = (int)flen;
        if (flen - framecount > 0.0001) framecount++;
        //if (framecount < 1) framecount = 1;

        framecount += 1;
        Frame last = null;

        //ani.StartPlayback();
        //逐帧复制
        //ani.Play(_clip.name, 0, 0);
        for (int i = 0; i < framecount; i++)
        {
            ani.Play(_clip.name, 0, (i * 1.0f / fps) / clip.length);
            ani.Update(0);

            last = new Frame(last, i, cdpath);
            _clip.frames.Add(last);
        }
        if (_clip.loop)
        {
            _clip.frames[0].LinkLoop(last);
        }
        Debug.Log("FrameCount." + framecount);

        AniPlayer con = ani.GetComponent<AniPlayer>();

        List<AniClip> clips = null;
        if (con.clips != null)
        {
            clips = new List<AniClip>(con.clips);
        }
        else
        {
            clips = new List<AniClip>();
        }
        foreach (var c in clips)
        {
            if (c.name == _clip.name + ".FBAni")
            {
                clips.Remove(c);
                break;
            }
        }

        //ani.StopPlayback();
        string outpath = path + "/" + clip.name + ".FBAni.asset";
        AssetDatabase.CreateAsset(_clip, outpath);
        var src = AssetDatabase.LoadAssetAtPath(outpath, typeof(AniClip)) as AniClip;

        //设置clip

        //FB.CleanData.AniController con = ani.GetComponent<FB.CleanData.AniController>();

        clips.Add(src);
        con.clips = clips;
    }

    //从一个Animator中获取所有的Animation
    public static void FindAllAniInControl(UnityEditor.Animations.AnimatorController control, List<AnimationClip> list)
    {
        for (int i = 0; i < control.layers.Length; i++)
        {
            var layer = control.layers[i];
            FindAllAniInControl(layer.stateMachine, list);
        }
    }

    static void FindAllAniInControl(UnityEditor.Animations.AnimatorStateMachine machine, List<AnimationClip> list)
    {
        for (int i = 0; i < machine.states.Length; i++)
        {
            var m = machine.states[i].state.motion;
            if (list.Contains(m as AnimationClip) == false)
            {
                list.Add(m as AnimationClip);
            }
        }
        for (int i = 0; i < machine.stateMachines.Length; i++)
        {
            var m = machine.stateMachines[i].stateMachine;
            FindAllAniInControl(m, list);
        }
    }

}
