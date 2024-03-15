using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectShowContextOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    GameObject _mouseIcon;

    private void Awake()
    {
        if (_mouseIcon != null)
        {
            _mouseIcon.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter - find places to add context options and add");

        // get the size and spawn a shape
        //Bounds bounds = GetComponent<MeshRenderer>().bounds;

        //var indicator = CreateExtentIndicator();
        //Vector3 space = Camera.main.WorldToScreenPoint(indicator.transform.position, Camera.MonoOrStereoscopicEye.Mono);
        //space += new Vector3(0, 100, 0);
        //space += new Vector3(100, 0, 0);
        //indicator.transform.position = Camera.main.ScreenToWorldPoint(space);

        if (_mouseIcon != null)
        {
            _mouseIcon.SetActive(true);
            //_mouseIcon.transform.position = space;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit - remove all context options");
    }

    //GameObject CreateExtentIndicator()
    //{
    //    GameObject go = new GameObject();
    //    go.transform.position = this.transform.position;
    //    Sphere sphere = go.AddComponent<Sphere>();
    //    sphere.Radius = 0.1f;

    //    return go;
    //}
}