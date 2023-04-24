using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    // references
    private AnimationPlayer animationPlayer;
    private PlayerCam_MLab playerCam;
    private Stats playerStats;

    // coroutines
    private Dictionary<int, Coroutine> coroutineLookup = new Dictionary<int, Coroutine>();

    // testing
    public Effect testEffect;

    private void Start()
    {
        animationPlayer = PlayerReferences.instance.animationPlayer;
        playerCam = PlayerReferences.instance.cam;
        playerStats = PlayerReferences.instance.stats;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayEffect(testEffect);
        }
    }

    public void StartPlaylist(EffectPlaylist playlist)
    {
        int id = playlist.GetHashCode();

        if (coroutineLookup.ContainsKey(id))
            return;

        print("playing playlist with id " + id);
        Coroutine coroutine = StartCoroutine(EffectPlaylistCycle(playlist));
        coroutineLookup.Add(id, coroutine);
    }

    public void StopPlaylist(int playlistId)
    {
        if (!coroutineLookup.ContainsKey(playlistId)) 
            return;

        print("stopping playlist with id " + playlistId);

        StopCoroutine(coroutineLookup[playlistId]);
        coroutineLookup.Remove(playlistId);
    }

    public IEnumerator EffectPlaylistCycle(EffectPlaylist playlist)
    {
        for (int i = 0; i < playlist.effects.Count; i++)
        {
            EffectPlaylistItem playlistItem = playlist.effects[i];

            yield return new WaitForSeconds(playlistItem.startDelay);

            playlistItem.effect.intensityModifier = playlistItem.intensity;
            PlayEffect(playlistItem.effect);
        }
    }

    public void PlayEffect(Effect effect)
    {
        Effect.EffectType effectType = effect.GetEffectType();

        // camera
        if (effectType == Effect.EffectType.Camera)
        {
            Effect.CameraEffectType cameraEffectType = effect.GetCameraEffectType();

            // shake effects
            if (cameraEffectType == Effect.CameraEffectType.NormalShake ||
                cameraEffectType == Effect.CameraEffectType.CinemachineImpulse ||
                cameraEffectType == Effect.CameraEffectType.PositionMove ||
                cameraEffectType == Effect.CameraEffectType.RotationShake ||
                cameraEffectType == Effect.CameraEffectType.ZoomShake ||
                cameraEffectType == Effect.CameraEffectType.CamShake)
            {
                CameraShakeEffect camShakeEffect = (CameraShakeEffect)effect;
                playerCam.PlayShake(camShakeEffect);
            }

            // post processing effects
            if (cameraEffectType == Effect.CameraEffectType.Vignette ||
                cameraEffectType == Effect.CameraEffectType.ChromaticAberration ||
                cameraEffectType == Effect.CameraEffectType.ChromaticAberration ||
                cameraEffectType == Effect.CameraEffectType.Bloom ||
                cameraEffectType == Effect.CameraEffectType.DepthOfField ||
                cameraEffectType == Effect.CameraEffectType.FilmGrain ||
                cameraEffectType == Effect.CameraEffectType.WhiteBalance ||
                cameraEffectType == Effect.CameraEffectType.LensDistortion ||
                cameraEffectType == Effect.CameraEffectType.MotionBlur ||
                cameraEffectType == Effect.CameraEffectType.PaniniProjection ||
                cameraEffectType == Effect.CameraEffectType.ColorAdjustments)
            {
                PostProcessingEffect postProcessingEffect = (PostProcessingEffect)effect;
                PostProcessingManager.instance.SetupEffect(postProcessingEffect);
            }
        }

        // sound
        else if (effectType == Effect.EffectType.Sound)
        {
            // sound manager
        }

        // animation
        else if (effectType == Effect.EffectType.Animation)
        {
            // animationPlayer.Play()
        }

        // stat
        else if (effectType == Effect.EffectType.Stat)
        {
            StatEffect statEffect = (StatEffect)effect;

            playerStats.AddEffect(statEffect);
        }
    }
}
