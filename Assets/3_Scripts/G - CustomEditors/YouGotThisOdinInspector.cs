
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouGotThisOdinInspector : MonoBehaviour
{
    public EditorUiData.Component componentToChange;
    [Range(0f, 800f)] public float height;
    [Range(0f, 100f)] public float alphaValue;
    public bool drawSpaceBeforeFirstVariable;

    //[Button]
    public void ChangeComponent()
    {
        EditorUiData editorUiData = Resources.Load<EditorUiData>("EditorUiSettings");
        editorUiData.cd_componentDesingLookup[componentToChange].baseHeigth = height;
        editorUiData.cd_componentDesingLookup[componentToChange].bgAlphaValue = alphaValue;
        editorUiData.cd_componentDesingLookup[componentToChange].drawSpaceBeforeFirstVariable = drawSpaceBeforeFirstVariable;
    }
}
