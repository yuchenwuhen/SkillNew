using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Frame : ICloneable
{
    public object Clone()
    {
        Frame frame = new Frame();
        return frame;
    }
}
