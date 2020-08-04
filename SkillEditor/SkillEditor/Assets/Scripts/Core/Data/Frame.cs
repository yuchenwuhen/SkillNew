using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Frame : ICloneable
{
    [SerializeField]
    public int fid;
    [SerializeField]
    public bool key;

    /// <summary>
    /// 骨骼
    /// </summary>
    [SerializeField]
    public List<PoseBoneMatrix> bonesinfo = new List<PoseBoneMatrix>();

    public Frame()
    {

    }
    public Frame(Frame last, int _fid, IList<Transform> trans)
    {
        //Debug.LogWarning("bones=" + trans.Length);

        this.fid = _fid;
        this.key = true;
        for (int i = 0; i < trans.Count; i++)
        {
            PoseBoneMatrix b = new PoseBoneMatrix();

            bonesinfo.Add(b);
            bonesinfo[i].Record(trans[i], last == null ? null : last.bonesinfo[i]);

        }

    }

    public void LinkLoop(Frame last)
    {
        for (int i = 0; i < bonesinfo.Count; i++)
        {
            bonesinfo[i].Tag(last.bonesinfo[i]);
        }
    }

    public object Clone()
    {
        Frame frame = new Frame();
        return frame;
    }
}
