using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;


public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private VolumeProfile volumeProfile;

    /// volume components
    [HideInInspector] public Vignette vignette;
    [HideInInspector] public Bloom bloom;
    [HideInInspector] public ColorAdjustments colorAdj;
    [HideInInspector] public DepthOfField depthOfField;
    [HideInInspector] public ChromaticAberration chromaticAb;
    [HideInInspector] public FilmGrain filmGrain;
    [HideInInspector] public LensDistortion lensDist;
    [HideInInspector] public MotionBlur motionBlur;
    [HideInInspector] public WhiteBalance whiteBalance;

    public PostProcessingEffect vignetteEffect;

    /// can't handly multiple effects of the same type simultaneously (currently)
    private List<Effect.CameraEffectType> bannedTypes;

    #region Singleton

    public static PostProcessingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        GetAllVolumeProfiles();

        bannedTypes = new List<Effect.CameraEffectType>();
    }

    private void GetAllVolumeProfiles()
    {
        for (int i = 0; i < volumeProfile.components.Count; i++)
        {
            if (volumeProfile.components[i].name == "Vignette")
                vignette = (Vignette)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "Bloom")
                bloom = (Bloom)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "ColorAdjustments")
                colorAdj = (ColorAdjustments)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "DepthOfField")
                depthOfField = (DepthOfField)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "ChromaticAberration")
                chromaticAb = (ChromaticAberration)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "FilmGrain")
                filmGrain = (FilmGrain)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "LensDistortion")
                lensDist = (LensDistortion)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "MotionBlur")
                motionBlur = (MotionBlur)volumeProfile.components[i];

            else if (volumeProfile.components[i].name == "WhiteBalance")
                whiteBalance = (WhiteBalance)volumeProfile.components[i];
        }
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            SetupEffect(vignetteEffect);
        }
    }

    #region Setup And Start Effects

    public void SetupEffect(PostProcessingEffect effect)
    {
        Effect.CameraEffectType cameraEffectType = effect.GetCameraEffectType();

        if (IsBannedType(cameraEffectType)) return;

        print("Setting up " + cameraEffectType + " effect... ");

        EffectData effectData = new EffectData(effect.duration, effect.effectCurve);

        float intensityModifier = effect.intensityModifier;

        // Vignette
        if(cameraEffectType == Effect.CameraEffectType.Vignette)
        {
            VignetteEffect vignetteEffect = (VignetteEffect)effect;
            effectData.AddFloatParameter(vignette.intensity, vignette.intensity.value, vignetteEffect.intensity * intensityModifier);
            effectData.AddFloatParameter(vignette.smoothness, vignette.smoothness.value, vignetteEffect.smoothness);

            effectData.AddColorParameter(vignette.color, vignette.color.value, vignetteEffect.color);

            /// set non-tweenable variables
            vignette.rounded.value = vignetteEffect.rounded;
        }

        // Chromatic Aberration
        else if (cameraEffectType == Effect.CameraEffectType.ChromaticAberration)
        {
            ChromaticAberrationEffect chromaticAbEffect = (ChromaticAberrationEffect)effect;
            effectData.AddFloatParameter(chromaticAb.intensity, chromaticAb.intensity.value, chromaticAbEffect.intensity * intensityModifier);
        }

        // Bloom
        else if (cameraEffectType == Effect.CameraEffectType.Bloom)
        {
            BloomEffect bloomEffect = (BloomEffect)effect;
            effectData.AddFloatParameter(bloom.intensity, bloom.intensity.value, bloomEffect.intensity * intensityModifier);
            effectData.AddFloatParameter(bloom.threshold, bloom.threshold.value, bloomEffect.threshold);
        }

        // Color Adjustments
        else if (cameraEffectType == Effect.CameraEffectType.ColorAdjustments)
        {
            ColorAdjustmentsEffect colorAdjEffect = (ColorAdjustmentsEffect)effect;
            effectData.AddFloatParameter(colorAdj.postExposure, colorAdj.postExposure.value, colorAdjEffect.postExposure);
            effectData.AddFloatParameter(colorAdj.saturation, colorAdj.saturation.value, colorAdjEffect.saturation);
            effectData.AddFloatParameter(colorAdj.contrast, colorAdj.contrast.value, colorAdjEffect.contrast);
            effectData.AddColorParameter(colorAdj.colorFilter, colorAdj.colorFilter.value, colorAdjEffect.colorFilter);
        }

        // White Balance
        else if (cameraEffectType == Effect.CameraEffectType.WhiteBalance)
        {
            WhiteBalanceEffect whiteBalanceEffect = (WhiteBalanceEffect)effect;
            effectData.AddFloatParameter(whiteBalance.temperature, whiteBalance.temperature.value, whiteBalanceEffect.temperature);
            effectData.AddFloatParameter(whiteBalance.tint, whiteBalance.tint.value, whiteBalanceEffect.tint);
        }

        // Depth Of Field
        else if(cameraEffectType == Effect.CameraEffectType.DepthOfField)
        {
            DepthOfFieldEffect depthOfFieldEffect = (DepthOfFieldEffect)effect;
            effectData.AddFloatParameter(depthOfField.gaussianStart, depthOfField.gaussianStart.value, depthOfFieldEffect.start);
            effectData.AddFloatParameter(depthOfField.gaussianEnd, depthOfField.gaussianEnd.value, depthOfFieldEffect.end);
        }

        // Film Grain
        else if (cameraEffectType == Effect.CameraEffectType.FilmGrain)
        {
            FilmGrainEffect filmGrainEffect = (FilmGrainEffect)effect;
            effectData.AddFloatParameter(filmGrain.intensity, filmGrain.intensity.value, filmGrainEffect.intensity * intensityModifier);
            effectData.AddFloatParameter(filmGrain.response, filmGrain.response.value, filmGrainEffect.response);
        }

        // Lens Distortion
        else if (cameraEffectType == Effect.CameraEffectType.LensDistortion)
        {
            LensDistortionEffect lensDistEffect = (LensDistortionEffect)effect;
            effectData.AddFloatParameter(lensDist.intensity, lensDist.intensity.value, lensDistEffect.intensity * intensityModifier);
            effectData.AddFloatParameter(lensDist.xMultiplier, lensDist.xMultiplier.value, lensDistEffect.xMultiplier);
            effectData.AddFloatParameter(lensDist.yMultiplier, lensDist.yMultiplier.value, lensDistEffect.yMultiplier);
            effectData.AddFloatParameter(lensDist.scale, lensDist.scale.value, lensDistEffect.scale);
        }

        // Motion Blur
        else if (cameraEffectType == Effect.CameraEffectType.MotionBlur)
        {
            MotionBlurEffect motionBlurEffect = (MotionBlurEffect)effect;
            effectData.AddFloatParameter(motionBlur.intensity, motionBlur.intensity.value, motionBlurEffect.intensity * intensityModifier);
            effectData.AddFloatParameter(motionBlur.clamp, motionBlur.clamp.value, motionBlurEffect.clamp);
        }

        bannedTypes.Add(cameraEffectType);
        PlayEffect(effectData, cameraEffectType);
    }

    // reset non-tweenable variables
    private void ResetCustomSettings(Effect.CameraEffectType type)
    {

    }

    private void PlayEffect(EffectData data, Effect.CameraEffectType type)
    {
        DOVirtual.Float(0f, 1f, data.duration, v =>
        {
            float t = data.effectCurve.Evaluate(v);

            for (int i = 0; i < data.floatParameters.Count; i++)
                data.floatParameters[i].value = Mathf.Lerp(data.floatStartValues[i], data.floatEndValues[i], t);

            for (int i = 0; i < data.colorParameters.Count; i++)
                data.colorParameters[i].value = Color.Lerp(data.colorStartValues[i], data.colorEndValues[i], t);

        }).OnComplete(() =>
        {
            for (int i = 0; i < data.floatParameters.Count; i++)
                data.floatParameters[i].value = data.floatStartValues[i];

            for (int i = 0; i < data.colorParameters.Count; i++)
                data.colorParameters[i].value = data.colorStartValues[i];

            RemoveBannedType(type);
            ResetCustomSettings(type);
        });
    }

    #endregion


    #region Banned Types

    private bool IsBannedType(Effect.CameraEffectType type)
    {
        for (int i = 0; i < bannedTypes.Count; i++)
        {
            if (bannedTypes[i] == type)
                return true;
        }

        return false;
    }

    private void RemoveBannedType(Effect.CameraEffectType type)
    {
        for (int i = 0; i < bannedTypes.Count; i++)
        {
            if (bannedTypes[i] == type)
                bannedTypes.RemoveAt(i);
        }
    }

    #endregion

}

public class EffectData
{
    public float duration;
    public AnimationCurve effectCurve;

    // floats
    public List<FloatParameter> floatParameters;
    public List<float> floatStartValues;
    public List<float> floatEndValues;

    // colors
    public List<ColorParameter> colorParameters;
    public List<Color> colorStartValues;
    public List<Color> colorEndValues;

    public EffectData(float argDuration, AnimationCurve argEffectCurve)
    {
        duration = argDuration;
        effectCurve = argEffectCurve;

        floatParameters = new List<FloatParameter>();
        floatStartValues = new List<float>();
        floatEndValues = new List<float>();

        colorParameters = new List<ColorParameter>();
        colorStartValues = new List<Color>();
        colorEndValues = new List<Color>();
    }
    
    public void AddFloatParameter(FloatParameter parameter, float start, float end)
    {
        floatParameters.Add(parameter);
        floatStartValues.Add(start);
        floatEndValues.Add(end);
    }

    public void AddColorParameter(ColorParameter parameter, Color start, Color end)
    {
        colorParameters.Add(parameter);
        colorStartValues.Add(start);
        colorEndValues.Add(end);
    }
}
