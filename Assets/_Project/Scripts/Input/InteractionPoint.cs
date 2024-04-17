using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionPoint : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject[] _toggleActive;

    [SerializeField]
    private string[] _runAnimation;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only left click
        if (eventData.button == 0)
        {
            ToggleActive();
            RunAnimations();
        }
    }

    private void ToggleActive()
    {
        foreach (GameObject go in _toggleActive)
        {
            // Switch the active state of this game object
            go.SetActive(!go.activeInHierarchy);
        }
    }

    private void RunAnimations()
    {
        Animator animator = this.GetComponentInParent<Animator>();

        if (_runAnimation.Length == 0) return;

        if (animator == null)
        {
            Debug.Log("No animator present, cannot run animation");
            return;
        }

        foreach (string animationName in _runAnimation)
        {
            animator.Play(animationName);
        }
    }
}
