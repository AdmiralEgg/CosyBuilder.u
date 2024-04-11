using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Can materials appear in multiple groups? How to resolve conflicts?
/// How do you control selection of a single face on double click?
/// </summary>
public class CustomizationManager : MonoBehaviour
{
    [SerializeField]
    Material[] WallMaterialGroup;

    [SerializeField]
    GameObject[] AllWalls;
    
    public Material[] FindMaterialGroup(Material m)
    {
        if (WallMaterialGroup.Contains(m))
        {
            Debug.Log($"Found the material {m.name} in WallMaterialGroup");
            return WallMaterialGroup;
        }
        else
        {
            return null;
        }
    }

    public List<Material> FindAllGroupedMaterials(GameObject selectedMesh)
    {
        // If the selected wall isn't in the AllWalls group, return null
        if (!WallMaterialGroup.Contains<Material>(selectedMesh.GetComponent<Material>()))
        {
            Debug.Log($"Could not find material {selectedMesh.GetComponent<Material>()} in WallMaterialGroup");
            return null;
        }
            
        return WallMaterialGroup.ToList<Material>();
    }

    public List<Material> FindAllGroupedWalls(GameObject selectedMesh)
    {
        GameObject selectedWall = selectedMesh.transform.parent.gameObject;

        // If the selected wall isn't in the AllWalls group, return null
        if (!AllWalls.Contains<GameObject>(selectedWall))
        {
            Debug.Log($"Could not find wall {selectedWall} in AllWalls Group");
            return null;
        }
                
        List<Material> returnMaterials = new List<Material>();

        foreach (GameObject wall in AllWalls)
        {
            returnMaterials.Add(wall.GetComponentInChildren<MeshRenderer>().material);
        }

        return returnMaterials;
    }
}
