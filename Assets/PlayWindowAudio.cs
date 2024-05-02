using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayWindowAudio : MonoBehaviour
{
    [SerializeField]
    private EventReference _birdsongRef;

    private EventInstance _birdsongInstance;
    
    void Awake()
    {
        _birdsongInstance = RuntimeManager.CreateInstance(_birdsongRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_birdsongInstance, this.gameObject.transform);
    }

    private void OnEnable()
    {
        _birdsongInstance.start();
    }

    private void OnDisable()
    {
        _birdsongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        _birdsongInstance.release();
    }
}
