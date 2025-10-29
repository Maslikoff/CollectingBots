using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseInputHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _baseLayerMask;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private BuildSystem _buildSystem;

    private Camera _mainCamera;
    private Base _selectedBase;

    public static event Action<Base> BaseSelected;
    public static event Action<Vector3> GroundClicked;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            DeselectBase();
    }

    public void DeselectBase()
    {
        if (_selectedBase != null)
            _selectedBase = null;
    }

    private void HandleMouseClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) 
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _baseLayerMask))
        {
            Base baseComponent = hit.collider.GetComponent<Base>();

            if (baseComponent != null)
            {
                SelectBase(baseComponent);

                return;
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayerMask))
        {
            GroundClicked?.Invoke(hit.point);
            _buildSystem.TryCreateBuildSite(hit.point);
            DeselectBase();
        }
    }

    private void SelectBase(Base baseComponent)
    {
        _selectedBase = baseComponent;
        BaseSelected?.Invoke(_selectedBase);
    }

    private void OnDrawGizmos()
    {
        if (_mainCamera != null && Input.GetMouseButton(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        }
    }
}