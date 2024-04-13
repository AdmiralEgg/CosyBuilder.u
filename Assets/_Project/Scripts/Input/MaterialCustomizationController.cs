using UnityEngine;

public class MaterialCustomizationController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _materialInstances;
    
    public void UpdateAllMaterialInstances(Color c)
    {
        foreach (GameObject materialObject in _materialInstances)
        {
            Material m = materialObject.GetComponentInChildren<MeshRenderer>().material;

            m.SetColor("_ToColor", c);
        }
    }

    public Color GetCurrentMaterialColor()
    {
        Color currentColor = _materialInstances[0].GetComponentInChildren<MeshRenderer>().material.GetColor("_ToColor");

        return currentColor;
    }
}
