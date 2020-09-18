using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEventAttribute : Attribute
{
    public string Name { get; set; }
    public string Des { get; private set; }

    public SkillEventAttribute(string name, string des="")
    {
        this.Name = name;
        this.Des = des;
    }
}
