using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubClip
{
    [SerializeField]
    public String name = "noname";
    [SerializeField]
    public bool loop;
    [SerializeField]
    public uint startframe;
    [SerializeField]
    public uint endframe;
}

public class AniClip : ScriptableObject
{
    public List<string> boneinfo = new List<string>();

    public List<Frame> frames = new List<Frame>();

    public List<SubClip> subclips = new List<SubClip>();

    public float fps = 24.0f;
    public bool loop;

    public int aniFrameCount
    {
        get { return frames.Count; }
    }

    [NonSerialized]
    private int m_bonehash = -1;
    public int Bonehash
    {
        get
        {
            if (m_bonehash == -1)
            {
                string name = "";
                foreach (var s in boneinfo)
                {
                    name += s + "|";
                }
                m_bonehash = name.GetHashCode();
            }
            return m_bonehash;
        }

    }

    public void CalcLerpFrameOne(int frame)
    {
        //搜索开始与结束帧
        if (frames[frame].key) return;
        if (frame <= 0 || frame >= frames.Count - 1) return;
        int ibegin = frame;
        for (; ibegin >= 0; ibegin--)
        {
            if (frames[ibegin].key)
            {
                break;
            }
        }
        if (ibegin == frame) return;
        int iend = frame;
        for (; iend < frames.Count; iend++)
        {
            if (frames[iend].key)
            {
                break;
            }
        }
        if (iend == frame) return;

        int i = frame;
        {
            //float d1 = (i - ibegin);
            //float d2 = (iend - i);
            float lerp = frames[i].lerp;
            frames[i] = Frame.Lerp(frames[ibegin], frames[iend], lerp);
            frames[i].lerp = lerp;
            frames[i].fid = i;
        }
    }

}
