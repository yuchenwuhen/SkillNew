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

    public float fps = 24.0f;
    public bool loop;

    public int aniFrameCount
    {
        get { return frames.Count; }
    }

}
