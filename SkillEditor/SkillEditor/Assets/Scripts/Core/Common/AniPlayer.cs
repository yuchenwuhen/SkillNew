using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AniPlayer : MonoBehaviour
{
    public List<AniClip> clips;

    public bool isShowBoxLine = true;

    Dictionary<string, int> clipcache = null;

    private bool m_loop = false;
    int m_startframe;
    int m_endframe;
    float m_fps = -1;

    float m_crossTimer = -1;
    float _crossTimerTotal = 0;
    Frame m_crossFrame = null;

    public Frame frameNow { get; private set; }

    private AniClip m_lastClip;     //当前动画
    private int m_lastframe = -1; //当前帧

    float m_timer = 0;

    /// <summary>
    /// 是否在运行模式
    /// </summary>
    bool m_playRunTime = false;

    /// <summary>
    /// 当前帧的总计数
    /// </summary>
    int frameCounter = -1;

    GameObject m_boxes = null;
    List<GameObject> mBoxList = new List<GameObject>();

    List<GameObject> mDotList = new List<GameObject>();
    GameObject dot;

    public class BoxColor
    {
        public BoxColor(Color line, Color box)
        {
            linecolor = line;
            boxcolor = box;
        }

        public Color linecolor;
        public Color boxcolor;
    }

    public Dictionary<string, BoxColor> boxcolor = new Dictionary<string, BoxColor>()
        {
            {"box_attack", new BoxColor(Color.black, new Color(0f, 0f, 0f, 0.3f))},
            {"box_area", new BoxColor(Color.green, new Color(0f, 1f, 0f, 0.3f))},
            {"box_behurt", new BoxColor(Color.red, new Color(1f, 0f, 0f, 0.3f))}
        };

    private float m_playTime = 0;

    /// <summary>
    /// Editor控制Play变量，防止因为调用DestroyImmediate删除音频组件（AniPlayerInspector会重新初始化一遍，变量值就改变了）
    /// </summary>
    public bool editorPlay
    {
        get;
        set;
    }

    private void Start()
    {
        m_playRunTime = true;
    }

    public void OnUpdate(float delta)
    {
        //帧推行
        if (m_lastClip == null)
            return;

        m_timer += delta;

        bool crossend = false;
        if (m_crossTimer >= 0)
        {
            m_crossTimer -= delta;
            if (m_crossTimer <= 0)
                crossend = true;
        }

        int _frameCount = (int)(m_timer * m_fps); //这里要用一个稳定的fps，就用播放的第一个动画的fps作为稳定fps


        //
        if (_frameCount == frameCounter)
            return;

        if (_frameCount > frameCounter + 1) //增加一个限制，不准动画跳帧
        {
            _frameCount = frameCounter + 1;
            m_timer = (float)_frameCount / m_fps;
        }

        frameCounter = _frameCount;


        //帧前行
        int frame = m_lastframe + 1;
        if (frame > m_endframe)
        {
            if (m_loop)
            {
                frame = m_startframe;
            }
            else
            {
                frame = m_endframe;
            }
        }

        //设置动作或者插值
        //if (crossend)
        //{
        //    crossFrame = null;
        //    SetPose(lastClip, frame, true);
        //    return;
        //}

        if (m_crossTimer >= 0)
        {
            //if (m_crossFrame != null)
            //{
            //    float l = 1.0f - m_crossTimer / _crossTimerTotal;
            //    SetPoseLerp(crossFrame, lastClip.frames[frame], l);

            //    lastframe = frame;
            //    frameNow = lastClip.frames[frame];
            //}
        }
        else
        {
            if (frame != m_lastframe)
            {
                SetPose(m_lastClip, frame);
                frameNow = m_lastClip.frames[frame];
            }
        }

    }

    #region 动画处理

    public void Play(string clip)
    {
        var c = this.GetClip(clip);
        this.Play(c);
    }

    /// <summary>
    /// 获取一个动画片段
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AniClip GetClip(string name)
    {
        if (clips == null || clips.Count == 0) return null;
        if (clipcache == null || clipcache.Count != clips.Count)
        {
            clipcache = new Dictionary<string, int>();
            for (int i = 0; i < clips.Count; i++)
            {
                clipcache[clips[i].name] = i;
            }
        }

        int igot = 0;

        if (name.EndsWith(".FBAni") == false)
        {
            name += ".FBAni";
        }
        if (clipcache.TryGetValue(name, out igot))
        {
            return clips[igot];
        }

        return null;
    }

    /// <summary>
    /// 播放一个动画片段
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="clipsub"></param>
    /// <param name="crosstimer"></param>
    public void Play(AniClip clip, SubClip clipsub = null, bool? clipLoop = null, bool? clipsubLoop = null, float crosstimer = 0)
    {
        onece = false;
        if (clipsub != null)
        {
            m_loop = clipsub.loop;
            m_startframe = (int)clipsub.startframe;
            m_endframe = (int)clipsub.endframe;
            if (m_fps < 0)
            {
                m_fps = clip.fps;
            }
        }
        else if (clip != null)
        {
            if (clipLoop != null)
            {
                m_loop = (bool)clipLoop;
            }
            else
            {
                m_loop = clip.loop;
            }
            m_startframe = 0;
            m_endframe = (clip.aniFrameCount - 1);
            if (m_fps < 0)
            {
                m_fps = clip.fps;
            }
        }

        if (crosstimer <= 0)
        {
            this.m_crossTimer = -1;
            m_crossFrame = null;

            m_lastClip = clip;
            m_lastframe = m_startframe;

            SetPose(clip, m_startframe, true);
            frameNow = m_lastClip.frames[m_lastframe];
            // TODO 修复设置后立马推帧问题
            m_timer = frameCounter / m_fps;
        }
        else
        {
            //if (lastClip != null && lastframe >= 0 && lastframe < lastClip.frames.Count)
            //{
            //    RecCrossFrame();
            //    lastClip = clip;
            //    lastframe = startframe;
            //    // TODO 修复设置后立马推帧问题
            //    timer = frameCounter / _fps;
            //}
            //else
            //{
            //    lastClip = clip;
            //    lastframe = startframe;

            //    SetPose(clip, startframe, true);
            //    frameNow = lastClip.frames[lastframe];
            //    // TODO 修复设置后立马推帧问题
            //    timer = frameCounter / _fps;
            //}

            //this._crossTimerTotal = this._crossTimer = crosstimer;
        }
    }

    int m_transcode = -1;
    Transform[] m_trans = null;

    public void SetPose(AniClip clip, int frame, bool reset = false, Transform parent = null)
    {
        if (clip.Bonehash != m_transcode)
        {
            m_trans = new Transform[clip.boneinfo.Count];
            for (int i = 0; i < clip.boneinfo.Count; i++)
            {
                m_trans[i] = this.transform.Find(clip.boneinfo[i]);
            }

            m_transcode = clip.Bonehash;
        }

        bool badd = false;

        if (m_lastClip == clip && !reset)
        {
            if (m_lastframe + 1 == frame) badd = transform;
            if (clip.loop && m_lastframe == clip.frames.Count - 1 && frame == 0)
                badd = true;
        }

        for (int i = 0; i < m_trans.Length; i++)
        {
            if (m_trans[i] == null) continue;
            if (parent != null && parent != m_trans[i])
            {
                if (m_trans[i].IsChildOf(parent) == false) continue;
            }

            clip.frames[frame].bonesinfo[i].UpdateTran(m_trans[i], badd);
        }

        if (clip.frames.Count > 0 && frame >= 0)
        {
            SetBoxColiderAttribute(clip.frames[frame]); //设置碰撞盒
            if (isShowBoxLine)
            {
                SetDebugDot(clip.frames[frame]); //设置触发点
            }

            SetAudio(clip.frames[frame]);
            if (m_playRunTime)
            {
                SetEffect(clip.frames[frame]); //设置/检测特效 
            }

        }

        m_lastClip = clip;
        m_lastframe = frame;
    }

    void SetBoxColiderAttribute(Frame src)
    {
        if (m_boxes != null)
        {
            m_boxes.transform.localPosition = Vector3.zero;
            m_boxes.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        if (src.boxesinfo != null)
        {
            //剔除null
            for (int i = mBoxList.Count - 1; i >= 0; i--)
            {
                if (mBoxList[i] == null)
                {
                    mBoxList.RemoveAt(i);
                }
            }

            for (int i = 0; i < src.boxesinfo.Count; i++)
            {
                if (mBoxList.Count - 1 < i)
                {
                    CreateBox(1);
                }

                SetBoxAttribute(src.boxesinfo[i], mBoxList[i]);
            }

            if (mBoxList.Count > src.boxesinfo.Count)
            {
                for (int i = src.boxesinfo.Count; i < mBoxList.Count; i++)
                {
                    if (mBoxList[i].activeSelf)
                    {
                        mBoxList[i].SetActive(false);
                    }
                }
            }
        }
    }

    void SetDebugDot(Frame f)
    {
        ReturnDot(); //每一帧调用，先重置box
        if (f.boxesinfo != null)
        {
            foreach (var b in f.dotesinfo)
            {
                SetDotAttribute(b);
            }
        }
    }

    public void SetDotAttribute(Dot d)
    {
        for (int i = mDotList.Count - 1; i >= 0; i--)
        {
            if (mDotList[i] == null)
            {
                mDotList.RemoveAt(i);
            }
        }

        if (dot != null)
            dot.transform.localRotation = new Quaternion(0, 0, 0, 0);
        GameObject _curdot = mDotList.Find(o => !o.activeSelf);
        if (_curdot == null)
        {
            _curdot = CreateDot();
        }

        _curdot.SetActive(true);
        _curdot.transform.localPosition = d.position;
        _curdot.name = d.name;
        switch (d.name)
        {
            case "hold":
                _curdot.GetComponent<Point_Vis>().lineColor = Color.black;
                break;
            case "behold":
                _curdot.GetComponent<Point_Vis>().lineColor = Color.red;
                break;
            case "create":
                _curdot.GetComponent<Point_Vis>().lineColor = Color.green;
                break;
        }

        _curdot.GetComponent<Point_Vis>().UpdatePoint();
    }

    public GameObject CreateDot()
    {
        if (transform.Find("_dotes"))
        {
            dot = transform.Find("_dotes").gameObject;
        }
        else
        {
            dot = new GameObject("_dotes");
        }

        dot.transform.parent = transform;
        dot.transform.localPosition = Vector3.zero;
        dot.transform.localRotation = new Quaternion(0, 0, 0, 0);
        dot.transform.localScale = Vector3.one;
        GameObject o = new GameObject();
        o.transform.localScale = new Vector3(3, 3, 3);
        o.AddComponent<Point_Vis>();
        o.GetComponent<Point_Vis>().UpdatePoint();
        o.transform.parent = dot.transform;
        mDotList.Add(o);
        return o;
    }

    void ReturnDot()
    {
        for (int i = mDotList.Count - 1; i >= 0; i--)
        {
            if (mDotList[i] == null)
            {
                mDotList.RemoveAt(i);
            }
        }

        foreach (var o in mDotList)
        {
            if (o != null)
            {
                o.SetActive(false);
            }
        }
    }

    void CreateBox(int count)
    {
        if (!transform.Find("_boxes"))
        {
            m_boxes = new GameObject("_boxes");
            m_boxes.transform.parent = transform;
        }
        else
        {
            m_boxes = transform.Find("_boxes").gameObject;
            if (mBoxList.Count == 0)
            {
                foreach (Transform t in m_boxes.transform)
                {
                    if (t != null)
                    {
                        t.gameObject.SetActive(false);
                        mBoxList.Add(t.gameObject);
                    }
                }

                if (mBoxList.Count > 0)
                    return;
            }
        }

        //加载AttackBox
        for (int i = 0; i != count; i++)
        {
            AddBoxTo(m_boxes);
        }
    }

    //添加box 
    void AddBoxTo(GameObject father)
    {
        GameObject o = null;
        o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.gameObject.name = "BoxColider";
        o.AddComponent<Collider_Vis>();
        var material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        material.color = new Color(1f, 1f, 1f, 0.2f);
        o.GetComponent<MeshRenderer>().material = material;

        o.transform.parent = father.transform;
        o.SetActive(false);
        o.hideFlags = HideFlags.DontSave;
        if (o != null)
        {
            mBoxList.Add(o);
        }
    }

    void SetBoxAttribute(AniBoxCollider _box, GameObject _curBox = null)
    {
        if (_curBox != null)
            _curBox.transform.localRotation = new Quaternion(0, 0, 0, 0);
        _curBox.SetActive(true);
        //重新设置box的属性
        _curBox.gameObject.name = _box.mName;
        _curBox.layer = LayerMask.NameToLayer(_box.mBoxType);
        //if (_curBox.transform.localPosition != _box.mPosition) 
        //{
        _curBox.transform.localPosition = _box.mPosition;
        //} 
        //计算scale
        var _colider = _curBox.GetComponent<BoxCollider>();
        _curBox.transform.localScale = new Vector3(_box.mSize.x / _colider.size.x, _box.mSize.y / _colider.size.y,
            _box.mSize.z / _colider.size.z);


        if (isShowBoxLine)
        {
            if (!_curBox.GetComponent<Collider_Vis>())
                _curBox.AddComponent<Collider_Vis>();
            if (!_curBox.GetComponent<LineRenderer>())
                _curBox.AddComponent<LineRenderer>();
            if (!_curBox.GetComponent<MeshRenderer>())
                _curBox.AddComponent<MeshRenderer>();
            SetBoxColor(_curBox);
        }
        else
        {
            if (_curBox.GetComponent<Collider_Vis>())
                DestroyImmediate(_curBox.GetComponent<Collider_Vis>());
            if (_curBox.GetComponent<LineRenderer>())
                DestroyImmediate(_curBox.GetComponent<LineRenderer>());
            if (_curBox.GetComponent<MeshRenderer>())
                DestroyImmediate(_curBox.GetComponent<MeshRenderer>());
        }
    }

    public void SetBoxColor(GameObject _curBox)
    {
        if (isShowBoxLine)
        {
            //颜色
            Collider_Vis collider_Vis = null;
            LineRenderer lineRenderer = null;
            MeshRenderer meshRenderer = null;

            if (_curBox.GetComponent<MeshRenderer>() && _curBox.GetComponent<Collider_Vis>())
            {
                collider_Vis = _curBox.GetComponent<Collider_Vis>();
                meshRenderer = _curBox.GetComponent<MeshRenderer>();
                collider_Vis.linewidth = 0.2f;
                collider_Vis.updateColl();
            }

            lineRenderer = _curBox.GetComponent<LineRenderer>();

            var material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));

            if (boxcolor.ContainsKey(LayerMask.LayerToName(_curBox.layer))) //Attck
            {
                if (collider_Vis != null)
                {
                    collider_Vis.lineColor = boxcolor[LayerMask.LayerToName(_curBox.layer)].linecolor;
                }

                material.color = boxcolor[LayerMask.LayerToName(_curBox.layer)].boxcolor;
            }

            meshRenderer.enabled = true;
            lineRenderer.enabled = true;
            meshRenderer.material = material;
            collider_Vis.updateColl();
        }
    }

    #endregion

    #region 特效

    class die
    {
        public int effid;
        public int lifetime; //per pose -1;
    }

    List<die> livetimelist = new List<die>();

    public int dir = 1;
    private bool onece = false;
    GameObject go = null;
    //每帧检测
    void SetEffect(Frame f)
    {
        CheckEffect();
        foreach (var e in f.effectList)
        {
            Transform o = this.transform.Find(e.follow);
            if (e.lifeframe > 0)
            {
                if (o != null)
                {
                    die d = new die();
                    d.lifetime = e.lifeframe;
                    //d.effid = AniResource.PlayEffectLooped(e.name, e.position, dir, o);
                    livetimelist.Add(d);
                }
            }
            else
            {
                //AniResource.PlayEffect(e.name, o, e.position, e.isFollow, dir);
            }
        }
    }

    //管理生命周期
    void CheckEffect()
    {
        //编辑器中误操作，移除所有为null的引用
        for (int i = livetimelist.Count - 1; i >= 0; i--)
        {
            livetimelist[i].lifetime--; //生命周期每帧 -1

            if (livetimelist[i].lifetime <= 0) //生命周期结束 删除特效
            {
                //AniResource.CloseEffectLooped(livetimelist[i].effid);
                livetimelist.RemoveAt(i);
            }
        }

        //Resources.UnloadUnusedAssets();
        //GC.Collect();
    }



    #endregion

    #region 音效

    private List<AudioSource> m_audioSourceList = new List<AudioSource>();

    public AudioSource GetValidSource()
    {
        for (int i = 0; i < m_audioSourceList.Count; i++)
        {
            if (m_audioSourceList[i]==null)
            {
                m_audioSourceList.Remove(m_audioSourceList[i]);
                i--;
            }
        }

        foreach (var item in m_audioSourceList)
        {
            if (!item.isPlaying)
            {
                return item;
            }
        }
        return null;
    }

    public void StopAllAudio()
    {
        foreach (var item in m_audioSourceList)
        {
            if (item.isPlaying)
            {
                item.Stop();
            }
        }
    }

    public void ClearAudioSource(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            if (m_audioSourceList.Contains(audioSource))
            {
                m_audioSourceList.Remove(audioSource);
            }
        }
        DestroyImmediate(audioSource);
    }

    void SetAudio(Frame f)
    {
        foreach (var audio in f.audioList)
        {
            PlaySound(audio);
        }
    }

    public void PlaySound(string audioName)
    {
        var sfx = Resources.Load(audioName) as AudioClip;
        if (sfx == null)
        {
            Debug.LogWarning("can't find audioName in Resources" + audioName);
            return;
        }

        if (m_playRunTime)
        {
            //编辑器
            PlaySound(sfx);
        }
        else
        {
            //运行模式暂时用这种，有统一音频框架后替换
            PlaySound(sfx);
        }
    }

    void PlaySound(AudioClip audioClip)
    {
        AudioSource audioSource = GetValidSource();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            m_audioSourceList.Add(audioSource);
        }
        audioSource.clip = audioClip;
        audioSource.Play();

        StartCoroutine(AudioPlayFinished(audioClip.length, ()=> {
            ClearAudioSource(audioSource);
        }));
    }

    private IEnumerator AudioPlayFinished(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);

        if (callback != null)
        {
            callback();
        }
    }

    #endregion
}
