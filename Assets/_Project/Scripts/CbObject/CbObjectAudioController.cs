using FMODUnity;
using UnityEngine;

public class CbObjectAudioController : MonoBehaviour
{
    public enum ObjectAudio { NotImplemented, Spawn }
    
    // FMOD Sounds
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _objectSpawn, _objectDetatch, _objectDrop;

    private void Awake() { }

    public void PlaySoundOneShot(ObjectAudio audioName)
    {
        switch (audioName)
        {
            case ObjectAudio.Spawn:
                Debug.Log("Playing spawned sound");
                FMODUnity.RuntimeManager.PlayOneShotAttached(_objectSpawn, this.gameObject);
                break;
            case ObjectAudio.NotImplemented:
                Debug.Log($"Sound not implemented.");
                break;
        }
    }
}