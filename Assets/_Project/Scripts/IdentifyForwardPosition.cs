using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifyForwardPosition : MonoBehaviour
{
    MeshFilter _filter;
    
    void Awake()
    {
        _filter = GetComponent<MeshFilter>();
    }


    private void Start()
    {
        Debug.Log($"Mesh {_filter.name} has: {_filter.mesh.normals.Length}");
    }

    private void OnDrawGizmos()
    {
        if ( _filter != null )
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, _filter.sharedMesh.normals[0]);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _filter.sharedMesh.normals[1]);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _filter.sharedMesh.normals[2]);
        }
    }
}
