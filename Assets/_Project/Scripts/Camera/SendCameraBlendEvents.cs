using Cinemachine;
using System;
using UnityEngine;

public class SendCameraBlendEvents : MonoBehaviour
{
    [SerializeField]
    private bool IsBlending;

    CinemachineBrain brain;

    public static Action CameraBlendStarted;
    public static Action CameraBlendFinished;

    void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
        IsBlending = brain.IsBlending;
    }

    private void Update()
    {
        if (brain.IsBlending == IsBlending) return;
        
        if (brain.IsBlending == true)
        {
            CameraBlendStarted?.Invoke();
        }

        if (brain.IsBlending == false)
        {
            CameraBlendFinished?.Invoke();
        }

        IsBlending = brain.IsBlending;
    }
}
