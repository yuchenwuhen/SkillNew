using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPlayer : MonoBehaviour
{
    public Skills skills = null;

    private bool m_isPlayingSkill = false;

    private int m_curSkillBlockIndex = 0;

    private Skill m_curSkill = null;

    private List<SkillBlock> m_curSkillBlock;

    /// <summary>
	/// 播放技能，默认一段攻击
	/// </summary>
	/// <param name="id">技能ID</param>
	/// <param name="block">技能Block</param>
	public void PlaySkill(int id, int block)
    {
        m_isPlayingSkill = true;
        this.m_curSkillBlockIndex = block;
        try
        {
            var skill = skills.SkillList[id - 1];
            if (skill != null)
            {
                this.m_curSkill = skill;
                m_curSkillBlock = m_curSkill.Blocks[m_curSkillBlockIndex];
                var c = aniPlayer.GetClip(curSkillBlock.AniName);
                m_aniPlayer.Play(c);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("技能数据错误: id-{0} block-{1}", id, block));
        }

    }
}
