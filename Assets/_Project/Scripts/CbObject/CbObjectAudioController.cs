using FMODUnity;
using UnityEngine;

public class CbObjectAudioController : MonoBehaviour
{
    public enum ObjectAudio { NotImplemented, Spawn, Detatched, Drop }
    
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
                FMODUnity.RuntimeManager.PlayOneShotAttached(_objectSpawn, this.gameObject);
                break;
            case ObjectAudio.Detatched:
                FMODUnity.RuntimeManager.PlayOneShotAttached(_objectDetatch, this.gameObject);
                break;
            case ObjectAudio.Drop:
                FMODUnity.RuntimeManager.PlayOneShotAttached(_objectDrop, this.gameObject);
                break;
            case ObjectAudio.NotImplemented:
                Debug.Log($"Sound not implemented.");
                break;
        }
    }
}