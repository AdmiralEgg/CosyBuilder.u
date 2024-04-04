using Shapes;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [SerializeField]
    private bool _inUse;

    [SerializeField]
    private float _snapRadius = 0.4f;

    public bool InUse
    {
        get { return _inUse; }
        set { _inUse = value; }
    }

    [Header("Snap Radius Config")]
    [SerializeField]
    private Color _indicatorColor;

    [SerializeField]
    private float _snapIndicatorRadius;

    private Sphere _snapPointIndicator;


    void Awake()
    {
        _inUse = false;

        // Create a sphere collider
        var sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = _snapRadius;

        _snapPointIndicator = this.gameObject.AddComponent<Sphere>();
        _snapPointIndicator.Radius = _snapIndicatorRadius;
        _snapPointIndicator.Color = _indicatorColor;
        _snapPointIndicator.enabled = false;

        // subscribe to CbObject state changes
        CbObjectStateMachine.OnStateChange += OnCbObjectStateChange;
    }

    private void OnCbObjectStateChange(CbObjectStateMachine.CbObjectState state)
    {
        if (state != CbObjectStateMachine.CbObjectState.Selected)
        {
            ShowSnapPoint(false);
            return;
        }
        
        if (TempSelectedStateManager.SelectedObject.PlacedPosition != CbObjectScriptableData.PlacedPosition.SnapPoint) return;
        
        ShowSnapPoint(true);
    }

    private void OnDrawGizmos()
    {
        // Draw the wire sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _snapRadius);

        // Draw the snap direction
        Gizmos.color = Color.red;
        Ray r = new Ray(this.transform.position, this.transform.parent.transform.forward);
        Gizmos.DrawRay(r);
    }

    // Light that gets brighter as the cursor approaches

    private void ShowSnapPoint(bool show)
    {
        _snapPointIndicator.enabled = show;
    }
}
