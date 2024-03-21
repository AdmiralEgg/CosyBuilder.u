using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TempSelectedStateManager : MonoBehaviour
{
    private TempSelectedStateManager Instance;
    
    private static CbObjectData _selectedObject;

    public static CbObjectData SelectedObject
    {
        get { return _selectedObject; }
        private set { _selectedObject = value; }
    }

    void Awake()
    {
        Instance = this;
    }

    public static void SetSelectedObject(CbObjectData data)
    {
        SelectedObject = data;
    }

    public static bool IsObjectSelected()
    {
        if (_selectedObject != null) return true;

        return false;
    }
}
