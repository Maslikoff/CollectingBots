using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const float MinDistance = 0.5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _resourceCarryVisual;

    private Vector3 _targetPosition;
    private Vector3 _basePosition;
    private ITakeResource _carriedResource;

    private bool _isMoving = false;
    private bool _hasResource = false;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_isMoving && !_hasResource;

    public event Action<Unit> BecameAvailable;
    public event Action<Unit> BecameBusy;
    public event Action<Unit, ITakeResource> ResourceDelivered;

    private void Update()
    {
        if (_isMoving)
            MoveToTarget();
    }

    public void AssignToCollectResource(ITakeResource resource, Vector3 basePosition)
    {
        if (IsAvailable == false)
            return;

        if (resource != null)
        {
            _targetPosition = new Vector3(resource.Position.x, transform.position.y, resource.Position.z);
            _carriedResource = resource;
            _isMoving = true;
            _basePosition = basePosition;

            BecameBusy?.Invoke(this);
        }
    }

    public void PickUpResource()
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

        _targetPosition = new Vector3(_basePosition.x, transform.position.y, _basePosition.z);
        _isMoving = true;
    }

    private void MoveToTarget()
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(transform.position, _targetPosition);

        if (distance < MinDistance)
            OnReachedTarget();
    }

    private void OnReachedTarget()
    {
        _isMoving = false;

        if (_hasResource == false)
            PickUpResource();
        else
            DepositResource();
    }

    private void DepositResource()
    {
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