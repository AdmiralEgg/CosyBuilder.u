using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCeilingRendering : MonoBehaviour
{
    private void Start()
    {
        //PlayerInput.GetPlayerByIndex(0).actions["EnterPhotoMode"].started += OnEnterPhotoMode;
        //PlayerInput.GetPlayerByIndex(0).actions["ExitPhotoMode"].started += OnExitPhotoMode;
    }

    public void OnEnterPhotoMode(InputAction.CallbackContext context)
    {
        ShowCeiling(show: true);
    }

    public void OnExitPhotoMode(InputAction.CallbackContext context)
    {
        ShowCeiling(show: false);
    }

    private void ShowCeiling(bool show)
    {
        if (show == true)
        {
            this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }
        else
        {
            this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
}
