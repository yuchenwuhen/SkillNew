using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoseBoneMatrix : ICloneable
{
    [SerializeField]
    public Vector3 t;
    [SerializeField]
    public Vector3 s;
    [SerializeField]
    Quaternion r = Quaternion.identity;

    public enum changetag
    {
        NoChange = 0,
        Trans = 1,
        Rotate = 2,
        Scale = 4,
        TransRotate = 3,
        TransScale = 5,
        RotateScale = 6,
        All = 7,
    }
    [SerializeField]
    public changetag tag;

    //unity默认实现的四元数相等精度太低
    static bool QuaternionEqual(Quaternion left, Quaternion right)
    {
        return left.x == right.x && left.y == right.y && left.z == right.z && left.w == right.w;

    }

    public void Record(Transform trans, PoseBoneMatrix last)
    {
        t = trans.localPosition;
        r = trans.localRotation;
        s = trans.localScale;
        tag = PoseBoneMatrix.changetag.All;

        if (last == null)
            tag = PoseBoneMatrix.changetag.All;
        else
        {
            tag = PoseBoneMatrix.changetag.NoChange;
            if (!QuaternionEqual(r, last.r))
            {
                tag |= PoseBoneMatrix.changetag.Rotate;
            }
            if (t != last.t)
            {
                tag |= PoseBoneMatrix.changetag.Trans;
            }
            if (s != last.s)
            {
                tag |= PoseBoneMatrix.changetag.Scale;
            }
        }
    }

    public void Tag(PoseBoneMatrix last)
    {
        if (last == null)
            tag = PoseBoneMatrix.changetag.All;
        else
        {
            tag = PoseBoneMatrix.changetag.NoChange;
            if (r != last.r)
            {
                tag |= PoseBoneMatrix.changetag.Rotate;
            }
            if (t != last.t)
            {
                tag |= PoseBoneMatrix.changetag.Trans;
            }
            if (s != last.s)
            {
                tag |= PoseBoneMatrix.changetag.Scale;
            }
        }
    }

    public object Clone()
    {
        PoseBoneMatrix bm = new PoseBoneMatrix();
        bm.tag = this.tag;
        bm.r = this.r;
        if (r.w == 0)
        {
            bm.r = Quaternion.identity;
        }
        bm.s = this.s;
        bm.t = this.t;
        return bm;
    }
}
