using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const float MinDistance = 0.5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _resourceCarryVisual;

    private Vector3 _targetPosition;
    private ITakeResource _targetResource;
    private Base _ownerBase;

    private bool _isMoving = false;
    private bool _hasResource = false;

    public ITakeResource TargetResource => _targetResource;
    public bool IsAvailable => !_isMoving && !_hasResource;

    private Action<Unit, ITakeResource> ResourceReached;
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

    public void AssignResource(ITakeResource resource, Action<Unit, ITakeResource> onReached)
    {
        if (IsAvailable == false) 
            return;

        _targetResource = resource;
        _targetPosition = resource.Position;
        ResourceReached = onReached;
        _isMoving = true;

        _ownerBase?.UnitBecameBusy(this);
    }

    public void SendToBase(Vector3 basePosition, Action<Unit> onReached)
    {
        BaseReached = onReached;
        _targetPosition = basePosition;
        _isMoving = true;
    }

    public void PickUpResource()
    {
        if (_targetResource == null || _targetResource.IsCollected) 
            return;

        if (_targetResource is MonoBehaviour resourceMono)
        {
            resourceMono.transform.SetParent(_resourceCarryVisual);
            resourceMono.transform.localPosition = Vector3.zero;
            resourceMono.transform.localRotation = Quaternion.identity;
        }

        ResourceReached?.Invoke(this, _targetResource);
    }

    private void MoveToTarget()
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;

        if(direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(transform.position, _targetPosition);

        if (distance < MinDistance)
            OnReachedTarget();
    }

    private void OnReachedTarget()
    {
        _isMoving = false;

        if(_hasResource == false)
        {
            PickUpResource();
        }
        else
        {
            DepositResource();
            BaseReached?.Invoke(this);
        }
    }

    private void DepositResource()
    {
        if (_targetResource != null)
        {
            if (_targetResource is MonoBehaviour resourceMono)
            {
                ResourcePool resourcePool = FindObjectOfType<ResourcePool>();

                if (resourcePool != null && resourceMono is Resource resource)
                    resourcePool.ReturnResource(resource);
                else
                    resourceMono.gameObject.SetActive(false);
            }

            _targetResource = null;
        }

        _ownerBase?.UnitBecameAvailable(this);
    }
}