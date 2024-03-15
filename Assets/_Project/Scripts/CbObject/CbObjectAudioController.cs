using UnityEngine;

public class CbObjectAudioController : MonoBehaviour
{
    private void Awake()
    {
        // Subscribe to triggers
    }

    public void PlayOneShot(string soundName)
    {
        Debug.Log($"Trigger sound {soundName} to play (not implemented).");
    }
}