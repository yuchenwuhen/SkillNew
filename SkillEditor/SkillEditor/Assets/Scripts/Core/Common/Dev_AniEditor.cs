using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev_AniEditor : MonoBehaviour
{
    /// <summary>
    /// 记录pose的地方
    /// </summary>
    [Serializable]
    public class poolitem
    {
        [SerializeField]
        public AniClip cliponeframe;
        [SerializeField]
        public string name;
        [SerializeField]
        public long time;
    }

    public List<poolitem> PosePool = new List<poolitem>();
    public AniClip aniInEdit;//编辑的动画
}
