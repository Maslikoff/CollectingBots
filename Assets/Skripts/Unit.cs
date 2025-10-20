using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const float MinDistance = 0.5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _resourceCarryVisual;

    private Vector3 _targetPosition;
    private ITakeResource _carriedResource;
    private Base _ownerBase;

    private bool _isMoving = false;
    private bool _hasResource = false;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_isMoving && !_hasResource;

    private Action<Unit> ResourceReached;
    private Action<Unit> BaseReached;

    private void Update()
    {
        if (_isMoving)
            MoveToTarget();
    }

    public void Initialize(Base baseController)
    {
        _ownerBase = baseController;
    }

    public void AssignToCollectResource(ITakeResource resource, System.Action<Unit> onPickedUp, System.Action<Unit> onDelivered)
    {
        if (IsAvailable == false)
            return;

        ResourceReached = onPickedUp;
        BaseReached = onDelivered;

        if (resource != null)
        {
            _targetPosition = new Vector3(resource.Position.x, transform.position.y, resource.Position.z);
            _carriedResource = resource;
            _isMoving = true;

            _ownerBase?.UnitBecameBusy(this);
        }
    }

    public void SendToBase(Vector3 basePosition, Action<Unit> onReached)
    {
        BaseReached = onReached;
        _targetPosition = basePosition;
        _isMoving = true;
    }

    public void PickUpResource()
    {
        if (_carriedResource == null)
            return;

        _hasResource = true;

        if (_carriedResource is MonoBehaviour resourceMono)
        {
            resourceMono.transform.SetParent(_resourceCarryVisual);
            resourceMono.transform.localPosition = Vector3.zero;
            resourceMono.transform.localRotation = Quaternion.identity;

            if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }

        Vector3 basePosition = _ownerBase.transform.position;
        _targetPosition = new Vector3(basePosition.x, transform.position.y, basePosition.z);
        _isMoving = true;

        ResourceReached?.Invoke(this);
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
            if (_carriedResource is MonoBehaviour resourceMono)
                _carriedResource.Collect();
         
            _carriedResource = null;
        }

        _hasResource = false;
        BaseReached?.Invoke(this);
        _ownerBase?.UnitBecameAvailable(this);
    }
}