using UnityEngine;
using UnityEngine.EventSystems;

public class BaseInputHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _baseLayerMask;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private MapBounds _mapBounds;

    private Camera _mainCamera;
    private Base _selectedBase;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
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
                _selectedBase = baseComponent;
                
                return;
            }
        }

        if (_selectedBase != null && Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayerMask))
        {
            if (_mapBounds != null)
            {
                bool isInside = _mapBounds.IsPositionInsideMap(hit.point);

                if (isInside)
                    _selectedBase.PlaceFlag(hit.point);
            }
            else
            {
                _selectedBase.PlaceFlag(hit.point);
            }

            _selectedBase = null;
        }
    }
}