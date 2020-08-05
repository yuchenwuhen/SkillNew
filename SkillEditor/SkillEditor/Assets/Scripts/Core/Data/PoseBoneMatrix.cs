using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
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

    public void UpdateTran(Transform trans, bool bAdd)
    {
        if (!bAdd || tag == changetag.All)
        {
            trans.localScale = s;
            trans.localPosition = t;
            trans.localRotation = r;

            //trans.localRotation = Quaternion.Euler(r);
            return;
        }
        switch (tag)
        {
            case changetag.NoChange:
                return;
            case changetag.Rotate:
                trans.localRotation = r;
                //trans.localRotation = Quaternion.Euler(r);
                break;
            case changetag.Trans:
                trans.localPosition = t;
                break;
            case changetag.Scale:
                trans.localScale = s;
                break;
            case changetag.RotateScale:
                trans.localScale = s;
                trans.localRotation = r;
                //trans.localRotation = Quaternion.Euler(r);
                break;
            case changetag.TransRotate:
                trans.localPosition = t;
                trans.localRotation = r;
                //trans.localRotation = Quaternion.Euler(r);
                break;
            case changetag.TransScale:
                trans.localScale = s;
                trans.localPosition = t;
                break;
        };

    }

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

    public static PoseBoneMatrix Lerp(PoseBoneMatrix left, PoseBoneMatrix right, float lerp)
    {
        PoseBoneMatrix m = new PoseBoneMatrix();
        m.tag = changetag.All;
        m.r = Quaternion.Lerp(left.r, right.r, lerp);
        if (float.IsNaN(m.r.x))
        {
            m.r = Quaternion.identity;
        }
        m.t = Vector3.Lerp(left.t, right.t, lerp);
        m.s = Vector3.Lerp(left.s, right.s, lerp);
        return m;

    }
}
