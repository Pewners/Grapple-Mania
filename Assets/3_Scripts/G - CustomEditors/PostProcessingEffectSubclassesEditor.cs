using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Note:
// I could easily merge all of these classes into one if I can find a way around this
// [CustomEditor(typeof())] thing, that limits each editor to one specific class as far as i know

#region Vignette

[CustomEditor(typeof(VignetteEffect))]
public class VignetteEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Vignette);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("Vignette", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region Motion Blur

[CustomEditor(typeof(MotionBlurEffect))]
public class MotionBlurEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.MotionBlur);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("MotionBlur", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region LensDistortion

[CustomEditor(typeof(LensDistortionEffect))]
public class LensDistortionEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.LensDistortion);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("LensDistortion", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region Bloom

[CustomEditor(typeof(BloomEffect))]
public class BloomEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Bloom);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("Bloom", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region ChromaticAberration

[CustomEditor(typeof(ChromaticAberrationEffect))]
public class ChromaticAberrationEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.ChromaticAberration);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("ChromaticAberration", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region ColorAdjustments

[CustomEditor(typeof(ColorAdjustmentsEffect))]
public class ColorAdjustmentsEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.ColorAdjustments);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("ColorAdjustments", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region DepthOfField

[CustomEditor(typeof(DepthOfFieldEffect))]
public class DepthOfFieldEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.DepthOfField);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("DepthOfField", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region FilmGrain

[CustomEditor(typeof(FilmGrainEffect))]
public class FilmGrainEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.FilmGrain);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("FilmGrain", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#region WhiteBalance

[CustomEditor(typeof(WhiteBalanceEffect))]
public class WhiteBalanceEffectEditor : Editor
{
    public PostProcessingEffectSerializedProperties postProcessingEffectProperties;

    private void OnEnable()
    {
        postProcessingEffectProperties = PostProcessingEffectEditor.GetSerializedProperties(serializedObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.WhiteBalance);

        // header postProcessingEffect
        PostProcessingEffectEditor.DrawPostProcessingEffectEditor(postProcessingEffectProperties, serializedObject);

        // header specific effect
        EditorUi.DrawHeader("WhiteBalance", 2, EditorUi.CustomColor.white);

        DrawPropertiesExcluding(serializedObject, PostProcessingEffectEditor.propertyNames);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion
