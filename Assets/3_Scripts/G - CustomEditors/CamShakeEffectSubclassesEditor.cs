using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Note:
// I could easily merge all of these classes into one if I can find a way around this
// [CustomEditor(typeof())] thing, that limits each editor to one specific class as far as i know

public class CamShakeEffectSubclassesEditor : MonoBehaviour
{
    #region Cinemachine Impulse

    [CustomEditor(typeof(CinemachineImpulseEffect))]
    public class CinemachineImpulseEffectEditor : Editor
    {
        public CamShakeEffectSerializedProperties camShakeEffectProperties;

        private void OnEnable()
        {
            camShakeEffectProperties = CamShakeEffectEditor.GetSerializedProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUi.DrawComponentTitle(EditorUiData.Component.CinemachineImpulseShake);

            // header postProcessingEffect
            CamShakeEffectEditor.DrawCamShakeEffectEditor(camShakeEffectProperties, serializedObject);

            // header specific effect
            EditorUi.DrawHeader("Impulse", 2, EditorUi.CustomColor.white);

            DrawPropertiesExcluding(serializedObject, CamShakeEffectEditor.propertyNames);

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Basic

    [CustomEditor(typeof(NormalCameraShakeEffect))]
    public class NormalCameraShakeEffectEditor : Editor
    {
        public CamShakeEffectSerializedProperties camShakeEffectProperties;

        private void OnEnable()
        {
            camShakeEffectProperties = CamShakeEffectEditor.GetSerializedProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUi.DrawComponentTitle(EditorUiData.Component.BasicShake);

            // header postProcessingEffect
            CamShakeEffectEditor.DrawCamShakeEffectEditor(camShakeEffectProperties, serializedObject);

            // header specific effect
            EditorUi.DrawHeader("Shake", 2, EditorUi.CustomColor.white);

            DrawPropertiesExcluding(serializedObject, CamShakeEffectEditor.propertyNames);

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Move Cam

    [CustomEditor(typeof(MoveCameraPositionEffect))]
    public class MoveCameraPositionEffectEditor : Editor
    {
        public CamShakeEffectSerializedProperties camShakeEffectProperties;

        private void OnEnable()
        {
            camShakeEffectProperties = CamShakeEffectEditor.GetSerializedProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUi.DrawComponentTitle(EditorUiData.Component.MoveCamShake);

            // header postProcessingEffect
            CamShakeEffectEditor.DrawCamShakeEffectEditor(camShakeEffectProperties, serializedObject);

            // header specific effect
            EditorUi.DrawHeader("Movement", 2, EditorUi.CustomColor.white);

            DrawPropertiesExcluding(serializedObject, CamShakeEffectEditor.propertyNames);

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Rotation

    [CustomEditor(typeof(RotateCameraEffect))]
    public class RotateCameraEffectEditor : Editor
    {
        public CamShakeEffectSerializedProperties camShakeEffectProperties;

        private void OnEnable()
        {
            camShakeEffectProperties = CamShakeEffectEditor.GetSerializedProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUi.DrawComponentTitle(EditorUiData.Component.RotationShake);
                
            // header postProcessingEffect
            CamShakeEffectEditor.DrawCamShakeEffectEditor(camShakeEffectProperties, serializedObject);

            // header specific effect
            EditorUi.DrawHeader("Rotation", 2, EditorUi.CustomColor.white);

            DrawPropertiesExcluding(serializedObject, CamShakeEffectEditor.propertyNames);

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Zoom

    [CustomEditor(typeof(ZoomCameraEffect))]
    public class ZoomCameraEffectEditor : Editor
    {
        public CamShakeEffectSerializedProperties camShakeEffectProperties;

        private void OnEnable()
        {
            camShakeEffectProperties = CamShakeEffectEditor.GetSerializedProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUi.DrawComponentTitle(EditorUiData.Component.ZoomShake);

            // header postProcessingEffect
            CamShakeEffectEditor.DrawCamShakeEffectEditor(camShakeEffectProperties, serializedObject);

            // header specific effect
            EditorUi.DrawHeader("Zoom", 2, EditorUi.CustomColor.white);

            DrawPropertiesExcluding(serializedObject, CamShakeEffectEditor.propertyNames);

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion
}
