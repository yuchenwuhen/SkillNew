using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniPlayer : MonoBehaviour
{
    public List<AniClip> clips;

    Dictionary<string, int> clipcache = null;

    private bool m_loop = false;
    int m_startframe;
    int m_endframe;
    float m_fps = -1;

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
            this._crossTimer = -1;
            crossFrame = null;

            lastClip = clip;
            lastframe = startframe;

            SetPose(clip, startframe, true);
            frameNow = lastClip.frames[lastframe];
            // TODO 修复设置后立马推帧问题
            timer = frameCounter / _fps;
        }
        else
        {
            if (lastClip != null && lastframe >= 0 && lastframe < lastClip.frames.Count)
            {
                RecCrossFrame();
                lastClip = clip;
                lastframe = startframe;
                // TODO 修复设置后立马推帧问题
                timer = frameCounter / _fps;
            }
            else
            {
                lastClip = clip;
                lastframe = startframe;

                SetPose(clip, startframe, true);
                frameNow = lastClip.frames[lastframe];
                // TODO 修复设置后立马推帧问题
                timer = frameCounter / _fps;
            }

            this._crossTimerTotal = this._crossTimer = crosstimer;
        }
    }

    #endregion
}
