using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Window_AniEditor : EditorWindow
{

    bool m_lockFrame = false;
    bool m_frameRecord = false;

    int m_toolsBar = 0;

    Vector2 m_actionPos;
    Vector2 m_effectpos;
    Vector2 m_audioPos;

    private void OnGUI()
    {
        if (editor == null || editor.aniInEdit == null)
        {
            EditorGUILayout.HelpBox("无效信息", MessageType.Warning);
            return;
        }

        string label = editor.aniInEdit.name;
        GUILayout.BeginHorizontal();
        GUILayout.Label("PosePlus编辑:" + label + ",记得多多 Ctrl + S");

        if (GUILayout.Button("Play") )
        {
            aniPlayer.Play(editor.aniInEdit);
        }

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(editor.aniInEdit);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_frameRecord = GUILayout.Toggle(m_frameRecord, "自动记录动画");
        if (m_frameRecord)
        {
            //防止自动播放动画
            aniPlayer.editorPlay = false;
        }

        m_lockFrame = GUILayout.Toggle(m_lockFrame, "锁定帧");

        GUILayout.EndHorizontal();

        string[] toolsStr = new string[] { "动作编辑","特效编辑","声音编辑" };
        m_toolsBar = GUILayout.Toolbar(m_toolsBar, toolsStr);

        Layout_DrawSeparator(Color.white);

        GUI_AniPos();

        Layout_DrawSeparator(Color.white);

        GUILayout.BeginHorizontal();

        if (m_toolsBar == 0)    //动作编辑模式
        {
            m_actionPos = GUILayout.BeginScrollView(m_actionPos, GUILayout.Width(300));
            {
                if (!m_frameRecord)
                {
                    GUI_ClipFunc();
                }
                else
                {
                    GUI_FrameFunc();
                }
            }

            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);

        }
        else if (m_toolsBar == 1)  //特效编辑模式
        {
            m_effectpos = GUILayout.BeginScrollView(m_effectpos, GUILayout.Width(500));
            {
                GUI_Effect();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
        }
        else if (m_toolsBar == 2) //音效编辑模式
        {
            m_audioPos = GUILayout.BeginScrollView(m_audioPos, GUILayout.Width(300));
            {
                GUI_Audio();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
        }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制横填充框
    /// —
    /// </summary>
    /// <param name="color"></param>
    /// <param name="height"></param>
    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }

    /// <summary>
    /// 绘制垂直填充框
    /// |
    /// </summary>
    /// <param name="color"></param>
    /// <param name="width"></param>
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }

    #region 动作编辑模块

    int m_curframe = -1;
    Vector2 anipos = Vector2.zero;

    void SetFrame(int f, bool force = false)
    {
        if (m_lockFrame) return;

        if (m_curframe != f || force)
        {
            aniPlayer.SetPose(editor.aniInEdit, f, true);
            m_curframe = f;
        }
    }

    private void GUI_AniPos()
    {
        GUILayout.Label("Animation pos:(" + m_curframe + "/" + editor.aniInEdit.aniFrameCount + ")");

        int nf = (int)GUILayout.HorizontalScrollbar(m_curframe, 1, 0, editor.aniInEdit.aniFrameCount);
        anipos = EditorGUILayout.BeginScrollView(anipos, true, false, GUILayout.Height(230));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
        {
            var obc = GUI.backgroundColor;
            if (m_curframe == i)
            {
                GUI.backgroundColor = Color.green;
            }
            GUILayout.BeginVertical(GUILayout.Width(1));

            var oc = GUI.color;
            string txt = "F";
            if (editor.aniInEdit.frames[i].key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                txt = "K";

            }
            GUILayout.Label(i.ToString());
            if (GUILayout.Button(txt))
            {
                nf = i;
            }
            //关键帧下需要条垂直的空格
            if (editor.aniInEdit.frames[i].key)
            {
                GUILayout.Space(123);
            }
            GUI.color = oc;
            if (editor.aniInEdit.frames[i].key == false)
            {//调整曲线
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                float lerp = GUILayout.VerticalScrollbar(editor.aniInEdit.frames[i].lerp, 0.01f, 1.0f, 0, GUILayout.Height(120));
                if (lerp != editor.aniInEdit.frames[i].lerp)
                {
                    editor.aniInEdit.frames[i].lerp = lerp;
                    editor.aniInEdit.CalcLerpFrameOne(i);
                    if (i == m_curframe)
                    {
                        SetFrame(m_curframe, true);
                        EditorUtility.SetDirty(editor.aniInEdit);
                    }
                }
                GUILayout.EndHorizontal();
            }
            //box帧按钮
            string b_txt = "F";
            if (editor.aniInEdit.frames[i].box_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                b_txt = "K";

            }
            if (GUILayout.Button(b_txt))
            {

                nf = i;
            }


            GUI.color = oc;

            string d_txt = "○";
            if (editor.aniInEdit.frames[i].dot_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                d_txt = "●";

            }
            if (GUILayout.Button(d_txt))
            {

                nf = i;
            }
            GUILayout.EndVertical();
            GUI.color = oc;
            GUI.backgroundColor = obc;


        }

        SetFrame(nf);

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    uint m_newAniLen = 1;

    void GUI_ClipFunc()
    {
        GUILayout.Label("ClipFunc");

        bool bclip = GUILayout.Toggle(editor.aniInEdit.loop, "是否循环动画");
        if (bclip != editor.aniInEdit.loop)
        {
            editor.aniInEdit.loop = bclip;
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        if (GUILayout.Button("切换当前关键帧/非关键帧"))
        {
            if (m_curframe == 0 || m_curframe == editor.aniInEdit.aniFrameCount - 1)
            {
                EditorUtility.DisplayDialog("warning", "第一帧和最后一帧必须是关键帧", "ok");
            }
            else
            {
                editor.aniInEdit.frames[m_curframe].key = !editor.aniInEdit.frames[m_curframe].key;
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }

        GUILayout.Space(12);
        GUILayout.BeginHorizontal();
        GUILayout.Label("插入帧", GUILayout.Width(70));
        var str = GUILayout.TextField(m_newAniLen.ToString(), GUILayout.Width(40));
        uint.TryParse(str, out m_newAniLen);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("插入帧,当前帧之前"))
        {
            //if (editor.aniInEdit.frames.Count == 0)
            //{
            //    var fout = f.Clone() as FB.PosePlus.Frame;
            //    fout.key = true;
            //    editor.aniInEdit.frames.Insert(curframe + 1, fout);
            //}
            var f = editor.aniInEdit.frames[m_curframe];

            for (int i = 0; i < m_newAniLen; i++)
            {
                var fout = f.Clone() as Frame;
                fout.key = false;
                editor.aniInEdit.frames.Insert(m_curframe, fout);
            }

            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                    editor.aniInEdit.frames[i].box_key = true;
                    editor.aniInEdit.frames[i].dot_key = true;
                }
            }
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        if (GUILayout.Button("插入帧,当前帧之后"))
        {
            var f = editor.aniInEdit.frames[m_curframe];

            for (int i = 0; i < m_newAniLen; i++)
            {
                var fout = f.Clone() as Frame;
                fout.key = false;
                editor.aniInEdit.frames.Insert(m_curframe + 1, fout);
            }
            //首位帧为关键帧
            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                    editor.aniInEdit.frames[i].dot_key = true;
                    editor.aniInEdit.frames[i].box_key = true;
                }
            }
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        GUILayout.Space(12);
        if (GUILayout.Button("删除当前帧"))
        {
            if (editor.aniInEdit.frames.Count > 1)
            {
                editor.aniInEdit.frames.RemoveAt(m_curframe);
                for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
                {
                    editor.aniInEdit.frames[i].fid = i;
                    if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                    {
                        editor.aniInEdit.frames[i].key = true;
                        editor.aniInEdit.frames[i].box_key = true;
                        editor.aniInEdit.frames[i].dot_key = true;
                    }
                }
                if (m_curframe < 0) m_curframe = 0;
                if (m_curframe >= editor.aniInEdit.aniFrameCount)
                    m_curframe = editor.aniInEdit.aniFrameCount - 1;
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
        if (GUILayout.Button("删除当前开始5帧"))
        {
            for (int c = 0; c < 5; c++)
            {
                if (editor.aniInEdit.frames.Count > 1 && m_curframe < editor.aniInEdit.aniFrameCount)
                {
                    editor.aniInEdit.frames.RemoveAt(m_curframe);
                }
            }
            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                }
            }
            if (m_curframe < 0) m_curframe = 0;
            if (m_curframe >= editor.aniInEdit.aniFrameCount)
                m_curframe = editor.aniInEdit.aniFrameCount - 1;
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        GUILayout.Space(12);


        if (editor.aniInEdit.frames.Count > 0 && editor.aniInEdit.frames[m_curframe].key)
        {

            var oc = GUI.color;
            GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
            if (GUILayout.Button("记录到当前帧"))
            {
                AniClip ani = editor.aniInEdit;
                List<Transform> trans = new List<Transform>();
                foreach (var b in ani.boneinfo)
                {
                    trans.Add(editor.transform.Find(b));
                }
                var frame = new Frame(null, m_curframe, trans);
                editor.aniInEdit.frames[m_curframe].bonesinfo = new List<PoseBoneMatrix>(frame.bonesinfo);
                editor.aniInEdit.frames[m_curframe].key = true;

                //前后自动插值
                if (m_curframe != 0)
                {
                    editor.aniInEdit.ResetLerpFrameSegment((m_curframe - 1));
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
                if (m_curframe != editor.aniInEdit.frames.Count - 1)
                {
                    editor.aniInEdit.ResetLerpFrameSegment((m_curframe + 1));
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
                EditorUtility.SetDirty(editor.aniInEdit);
            }
            GUI.color = oc;
        }
        else
        {
            if (GUILayout.Button("重置非关键帧（线性）"))
            {
                editor.aniInEdit.ResetLerpFrameSegment(m_curframe);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
    }
    bool bLockFrame = false;
    bool bFrameRecord = false;

    System.DateTime lastRec = System.DateTime.Now;
    void GUI_FrameFunc()
    {
        GUILayout.Label("记录模式");
        GUILayout.Label("非关键帧不自动记录修改");
        GUILayout.Label("关键帧自动记录角色动作的状态");
        GUILayout.Label("每秒钟记录一次");
        this.Repaint();

        if ((System.DateTime.Now - lastRec).TotalSeconds > 1.0f)
        {
            lastRec = System.DateTime.Now;
            //非关键帧不自动记录修改
            if (editor.aniInEdit.frames[m_curframe].key == false) return;

            Debug.LogWarning("rec frame." + lastRec.ToLongTimeString());

            //record ani
            AniClip ani = editor.aniInEdit;
            List<Transform> trans = new List<Transform>();
            foreach (var b in ani.boneinfo)
            {
                trans.Add(editor.transform.Find(b));
            }
            var frame = new Frame(null, m_curframe, trans);
            frame.key = true;
            frame.box_key = true;
            editor.aniInEdit.frames[m_curframe] = frame;

            //记录盒子
            SaveEditData();

        }
    }

    void SaveEditData()
    {
        EditorUtility.SetDirty(editor.aniInEdit);
        SetFrame(m_curframe, true);
    }

    #endregion

    #region 绘画辅助函数等

    AniPlayer aniPlayer;
    public static void Show(Dev_AniEditor editor)
    {
        //获取现有的打开窗口或如果没有，创建一个新的
        var window = EditorWindow.GetWindow<Window_AniEditor>(false, "AniExEditor");
        window.SelfInit(editor);
    }

    Dev_AniEditor editor = null;
    void SelfInit(Dev_AniEditor editor)
    {
        aniPlayer = editor.GetComponent<AniPlayer>();
        this.editor = editor;
        m_curframe = 0;
        m_lockFrame = false;
        editor.aniInEdit.frames[0].key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].key = true;
        editor.aniInEdit.frames[0].box_key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].box_key = true;
        editor.aniInEdit.frames[0].dot_key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].dot_key = true;

    }

    #endregion

    #region 特效模块
    string effectAddr = "";
    string effectName = "null";
    private UnityEngine.Object obj;
    int effectlife = 0;
    GameObject effect = null;
    Vector2 effectScroll;
    bool isFollow = false;
    void GUI_Effect()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("跟随节点：");
        GUILayout.Space(5);
        effectAddr = GUILayout.TextField(effectAddr, GUILayout.Width(120));
        GUILayout.Space(5);
        if (GUILayout.Button("获得当前路径"))
        {

            //var name = Selection.activeTransform.gameObject.name;
            Transform tempObj = Selection.activeTransform.transform;
            string name = Selection.activeTransform.transform.name;
            for (int i = 0; ; i++)
            {
                if (tempObj.name != aniPlayer.transform.name)
                {
                    if (tempObj.parent == null)
                    {
                        EditorUtility.DisplayDialog("Warning", "请勿选择当前动画以外的节点！", "OK");
                        break;
                    }


                    tempObj = tempObj.parent;
                    if (tempObj.name != aniPlayer.transform.name)
                        name = tempObj.name + "/" + name;
                }
                else
                {
                    break;
                }
            }

            effectAddr = name;
        }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();

        //
        EditorGUILayout.HelpBox("拖拽特效文件,位于Resources目录下", MessageType.Info);
        GUILayout.BeginHorizontal();
        //        GUILayout.Label("特效：",GUILayout.Width(40));
        //        GUILayout.Space(5);

        obj = EditorGUILayout.ObjectField("特 效:", obj, typeof(Object), false);
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            effectName = path.Replace("Assets/Resources/", "").Replace(".prefab", "");
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("路 径:" + effectName);
        //
        effectlife = EditorGUILayout.IntField("生命周期(帧)：", effectlife);

        if (GUILayout.Button("添加特效"))
        {
            effect = Resources.Load(effectName) as GameObject;
            if (effect == null)
            {
                EditorUtility.DisplayDialog("waring", "Resource里不含改特效文件，请检查！", "OK");
            }
            else
            {
                Effect e = new Effect();
                e.name = effectName;
                e.follow = effectAddr;
                e.lifeframe = (int)effectlife;
                e.isFollow = isFollow;
                editor.aniInEdit.frames[m_curframe].effectList.Add(e);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }

        GUILayout.EndVertical();
        GUILayout.Label("特效数据:");
        //  effectScroll = GUILayout.BeginScrollView(effectScroll, GUILayout.Width(500));
        GUILayout.BeginHorizontal();
        GUILayout.Label("文件名", GUILayout.Width(50));
        GUILayout.Space(2);
        GUILayout.Label("跟随对象", GUILayout.Width(50));
        GUILayout.Space(2);
        GUILayout.Label("生命周期", GUILayout.Width(50));

        GUILayout.EndHorizontal();
        for (int m = editor.aniInEdit.frames[m_curframe].effectList.Count - 1; m >= 0; m--)
        {
            var e = editor.aniInEdit.frames[m_curframe].effectList[m];
            GUILayout.BeginHorizontal();
            e.name = GUILayout.TextField(e.name, GUILayout.Width(50));
            GUILayout.Space(2);
            e.follow = GUILayout.TextField(e.follow, GUILayout.Width(50));
            GUILayout.Space(2);
            string _str = GUILayout.TextField(e.lifeframe.ToString(), GUILayout.Width(50));
            int.TryParse(_str, out e.lifeframe);
            GUILayout.Space(2);
            if (GUILayout.Button("跟随当前选择", GUILayout.Width(80)))
            {
                Transform tempObj = Selection.activeTransform.transform;
                string name = Selection.activeTransform.transform.name;
                for (int i = 0; ; i++)
                {
                    if (tempObj.name != aniPlayer.transform.name)
                    {
                        if (tempObj.parent == null)
                        {
                            EditorUtility.DisplayDialog("Warning", "请勿选择当前动画以外的节点！", "OK");
                            return;
                        }
                        tempObj = tempObj.parent;
                        if (tempObj.name != aniPlayer.transform.name)
                            name = tempObj.name + "/" + name;
                    }
                    else
                    {
                        break;
                    }
                }

                e.follow = name;
            }
            GUILayout.Space(2);
            if (GUILayout.Button("DEL", GUILayout.Width(50)))
            {
                editor.aniInEdit.frames[m_curframe].effectList.Remove(e);
            }

            GUILayout.EndHorizontal();
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        //GUILayout.EndScrollView();
    }
    #endregion

    #region 音效模块

    private string m_path;

    Vector2 m_audioScroll;

    AudioClip go = null;

    void GUI_Audio()
    {
        GUILayout.Label("音乐系统：");

        GUILayout.BeginHorizontal();
        GUILayout.Label("路径：");
        m_path = EditorGUILayout.TextField(m_path,GUILayout.Width(100));

        if (GUILayout.Button("载入音乐"))
        {
            AudioClip music = Resources.Load(m_path) as AudioClip;
            if (music == null)
            {
                EditorUtility.DisplayDialog("waring", "Resource里不含该音乐文件，或该文件不是音效文件，请检查！", "OK");
            }
            else
            {
                editor.aniInEdit.frames[m_curframe].audioList.Add(m_path);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("音频：");
        go = EditorGUILayout.ObjectField(go, typeof(AudioClip), false) as AudioClip;

        if (GUILayout.Button("载入音乐"))
        {
            //AudioClip music = go as AudioClip;
            if (go == null)
            {
                EditorUtility.DisplayDialog("waring", "该文件不是音效文件，请检查！", "OK");
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(go.GetInstanceID());
                path = path.Replace("Assets/Resources/", "");
                path = path.Substring(0,path.LastIndexOf("."));
                editor.aniInEdit.frames[m_curframe].audioList.Add(path);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }

        GUILayout.EndHorizontal();

        //显示所有音乐信息
        m_audioScroll = GUILayout.BeginScrollView(m_audioScroll, GUILayout.Width(300));

        for (int i = editor.aniInEdit.frames[m_curframe].audioList.Count - 1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label((editor.aniInEdit.frames[m_curframe].audioList.Count - i).ToString() + ":", GUILayout.Width(40));
            GUILayout.Space(2);
            GUILayout.Label(editor.aniInEdit.frames[m_curframe].audioList[i], GUILayout.Width(150));
            GUILayout.Space(2);

            if (GUILayout.Button("Play", GUILayout.Width(45)))
            {
                //AudioPlayer.Instance().PlaySoundOnce(editor.aniInEdit.frames[curframe].aduioList[i]);
                //aniPlayer.resourceMgr.PlaySound();
                aniPlayer.PlaySound(editor.aniInEdit.frames[m_curframe].audioList[i]);
            }
            if (GUILayout.Button("DEL", GUILayout.Width(45)))
            {
                editor.aniInEdit.frames[m_curframe].audioList.RemoveAt(i);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    #endregion
}
