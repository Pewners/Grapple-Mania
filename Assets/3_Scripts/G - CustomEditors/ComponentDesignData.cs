using Dave;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "RcLab/System/Hardcoded/ComponentDesignData")]
public class ComponentDesignData : ScriptableObject
{
    public new string name;
    public Texture icon;
    public float baseHeigth;
    public float bgAlphaValue;
    public EditorUi.CustomColor color;
    public bool drawSpaceBeforeFirstVariable;
    public bool drawSoonAvailableInspectorNote;
}
