using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayModeSound : MonoBehaviour
{
    [SerializeField]
    private BoxCollider _houseBox, _riverBox;

    [SerializeField]
    private EventReference _gameplayMusic, _ambientTrack, _riverTrack;

    private EventInstance _gameplayMusicInstance, _ambientTrackInstance, _riverTrackInstance;

    void Awake()
    {
        _gameplayMusicInstance = RuntimeManager.CreateInstance(_gameplayMusic);
        _gameplayMusicInstance.setVolume(0.5f);

        _ambientTrackInstance = RuntimeManager.CreateInstance(_ambientTrack);
        _ambientTrackInstance.setVolume(0.5f);

        _riverTrackInstance = RuntimeManager.CreateInstance(_riverTrack);
        _riverTrackInstance.setVolume(0.5f);

        GameManager.GameStateChange += EnteredPlayMode;
    }

    private void EnteredPlayMode(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Play)
        {
            _gameplayMusicInstance.start();
        }
    }

    private void OnDestroy()
    {
        _gameplayMusicInstance.release();
        _ambientTrackInstance.release();
        _riverTrackInstance.release();
    }

    // If inside the house, play gameplay track
    // If outside the house, fade gameplay track and play birds track
    // If inside the river, fade in the river track
    // If outside the river, fade out the river track

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger!");
        
        if (collider == _houseBox)
        {
            // if gameplay track not playing, play it
            Debug.Log("play gameplay track, fade out ambience");
            _gameplayMusicInstance.start();
            _ambientTrackInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (collider == _riverBox)
        {
            Debug.Log("play river track");
            _riverTrackInstance.start();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("Trigger!");

        if (collider == _houseBox)
        {
            // if gameplay track playing, stop it
            Debug.Log("stop gameplay track, start ambience");
            _ambientTrackInstance.start();
            _gameplayMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (collider == _riverBox)
        {
            Debug.Log("stop river track");
            _riverTrackInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
