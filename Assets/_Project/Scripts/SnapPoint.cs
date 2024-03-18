using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    void Awake()
    {
        _inUse = false;

        // Create a sphere collider
        var sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = _snapRadius;
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

    public Vector3 GetSnapDirection()
    {
        return this.transform.parent.transform.forward;
    }
}
