using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniClip : ScriptableObject
{
    public List<string> boneinfo = new List<string>();

    public List<Frame> frames = new List<Frame>();

    public float fps = 24.0f;
    public bool loop;

}
