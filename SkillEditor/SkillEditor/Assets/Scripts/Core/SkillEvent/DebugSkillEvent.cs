using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkillEventAttribute("Debug", "测试Debug")]
public class DebugSkillEvent : ISkillEventEditor
{
    public SkillEvent OnGuiEditor(SkillEvent se)
    {
        return se;
    }
}
