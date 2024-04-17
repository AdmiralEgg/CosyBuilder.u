using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class CbObjectAudioController : MonoBehaviour
{
    public enum ObjectAudio { NotImplemented, Spawn }
    
    // FMOD Sounds
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _objectSpawn;

    private void Awake()
    {
        // Subscribe to triggers
    }

    public void PlaySound(EventReference sound)
    {
        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(sound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, this.transform);
        
        // Play then release
        instance.start();
        instance.release();
    }

    public void PlaySoundOneShot(ObjectAudio audioName)
    {
        switch (audioName)
        {
            case ObjectAudio.Spawn:
                PlaySound(_objectSpawn);
                break;
            case ObjectAudio.NotImplemented:
                Debug.Log($"Sound not implemented.");
                break;
        }
    }
}