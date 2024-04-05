using UnityEngine;

public class Interaction : MonoBehaviour
{ 
    [SerializeField]
    private GameObject[] _toggleActive;

    [SerializeField]
    private string[] _runAnimation;

    private void OnTriggerEnter(Collider other)
    {
        // Only action if this was involked by a call from the interaction collider script
        if (other.gameObject != this.gameObject) return;
        
        ToggleActive();
        RunAnimations();
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
