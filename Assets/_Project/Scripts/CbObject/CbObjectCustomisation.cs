using System.Collections.Generic;
using UnityEngine;

public class CbObjectCustomisation : MonoBehaviour
{
    MeshRenderer _renderer;

    const string SHADERCOLORPARAMNAME = "_ToColor";

    private void OnEnable()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();

        if (_renderer != null)
        {
            ApplyDefaultCustomisation();
        }
        else
        {
            Debug.Log("Cannot find renderer, no color applied" + this.gameObject.name);
        }
    }

    private void ApplyDefaultCustomisation()
    {
        if (GetComponent<CbObjectParameters>().RandomColorList.Count != 0)
        {
            List<Color> colorList = GetComponent<CbObjectParameters>().RandomColorList;

            System.Random rand = new System.Random();

            // apply to shader
            _renderer.material.SetColor(SHADERCOLORPARAMNAME, colorList[rand.Next(colorList.Count)]);
            return;
        }

        if (GetComponent<CbObjectParameters>().StaticColor != null) 
        {
            // apply to shader
            _renderer.material.SetColor(SHADERCOLORPARAMNAME, GetComponent<CbObjectParameters>().StaticColor);
            return;
        }

        Debug.Log("No colour to apply to object: " + this.gameObject.name );
    }
}