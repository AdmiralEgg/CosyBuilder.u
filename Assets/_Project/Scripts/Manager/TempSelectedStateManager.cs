using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TempSelectedStateManager : MonoBehaviour
{
    private TempSelectedStateManager Instance;
    
    private static CbObjectParameters _selectedObject;

    public static CbObjectParameters SelectedObject
    {
        get { return _selectedObject; }
        private set { _selectedObject = value; }
    }

    void Awake()
    {
        Instance = this;
    }

    public static void SetSelectedObject(CbObjectParameters data)
    {
        SelectedObject = data;
    }

    public static bool IsObjectSelected()
    {
        if (_selectedObject != null) return true;

        return false;
    }

    
}
