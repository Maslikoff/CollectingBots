using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _resourceCarryVisual;

    private Vector3 _basePosition;
    private ITakeResource _carriedResource;
    private Mover _mover;

    private bool _hasResource = false;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_mover.IsMoving && !_hasResource;

    public event Action<Unit> BecameAvailable;
    public event Action<Unit> BecameBusy;
    public event Action<Unit, ITakeResource> ResourceDelivered;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    public void AssignToCollectResource(ITakeResource resource, Vector3 basePosition)
    {
        if (IsAvailable == false)
            return;

        if (resource != null)
        {
            _carriedResource = resource;
            _basePosition = basePosition;

            _mover.MoveTo(resource.Position, OnPickUpResource);

            BecameBusy?.Invoke(this);
        }
    }

    private void OnPickUpResource()
    {
        if (_carriedResource == null)
            return;

        _hasResource = true;

        var resourceMono = _carriedResource as MonoBehaviour;
        resourceMono.transform.SetParent(_resourceCarryVisual);
        resourceMono.transform.localPosition = Vector3.zero;
        resourceMono.transform.localRotation = Quaternion.identity;

        if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Debug.Log("Go to BAZA");
        _mover.MoveTo(_basePosition, OnDepositResource);
    }

    private void OnDepositResource()
    {
        Debug.Log("BAZA");
        if (_carriedResource != null)
        {
            _carriedResource.Collect();
            ResourceDelivered?.Invoke(this, _carriedResource);
            _carriedResource = null;
        }

        _hasResource = false;
        BecameAvailable?.Invoke(this);
    }
}