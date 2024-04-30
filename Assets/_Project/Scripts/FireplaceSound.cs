using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceSound : MonoBehaviour
{
    // FMOD Sounds
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _fireplaceSound;

    [SerializeField]
    private Transform _attachedTo;

    private EventInstance _instance;

    [SerializeField]
    private float _volume = 0.5f;

    private void Awake() 
    {
        _instance = FMODUnity.RuntimeManager.CreateInstance(_fireplaceSound);
        _instance.setVolume(_volume);

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_instance, _attachedTo);
    }

    private void OnEnable()
    {
        _instance.start();
    }

    private void OnDisable()
    {
        _instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        _instance.release();
    }
}
