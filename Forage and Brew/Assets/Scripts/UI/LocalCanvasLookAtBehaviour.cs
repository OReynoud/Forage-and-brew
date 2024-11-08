using UnityEngine;

public class LocalCanvasLookAtBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RectTransform canvasRectTransform;

    // Cache fields
    private Camera _camera;


    private void Start()
    {
        _camera = Camera.main;
    }
    
    private void Update()
    {
        canvasRectTransform.forward = _camera.transform.forward;
    }
}
