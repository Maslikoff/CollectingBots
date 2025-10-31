using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHub : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 20f;
    [SerializeField] private ResourcePool _resourcePool;

    private List<Resource> _collectedResources = new List<Resource>();
    private int _totalResources = 0;

    private HashSet<ITakeResource> _freeResources = new HashSet<ITakeResource>();
    private HashSet<ITakeResource> _busyResources = new HashSet<ITakeResource>();

    public int TotalResources => _totalResources;
    public IReadOnlyList<Resource> CollectedResources => _collectedResources;

    public event Action<int> ResourcesChanged;

    public void AddResource(Resource resource)
    {
        if (resource == null)
            return;

        _collectedResources.Add(resource);
        _totalResources++;

        ResourcesChanged?.Invoke(_totalResources);

        resource.ReturnToPool();

       if (_resourcePool != null)
           _resourcePool.ReturnResource(resource);
    }

    public bool TrySpendResources(int amount)
    {
        if (_totalResources < amount)
            return false;

        _totalResources -= amount;

        int resourcesToRemove = Math.Min(amount, _collectedResources.Count);
        _collectedResources.RemoveRange(0, resourcesToRemove);

        ResourcesChanged?.Invoke(_totalResources);

        return true;
    }

    public void ResetResources()
    {
        _collectedResources.Clear();
        _totalResources = 0;
        ResourcesChanged?.Invoke(0);
    }

    public void RegisterResource(ITakeResource resource)
    {
        if (_busyResources.Contains(resource) == false)
            _freeResources.Add(resource);
    }

    public void UnregisterResource(ITakeResource resource)
    {
        _freeResources.Remove(resource);
        _busyResources.Remove(resource);
    }

    public ITakeResource GetAvailableResource()
    {
        ScanForNewResources();

        foreach (var resource in _freeResources)
        {
            if (resource != null && IsResourceAccessible(resource) && IsResourceInRange(resource))
            {
                _freeResources.Remove(resource);
                _busyResources.Add(resource);

                return resource;
            }
        }

        return null;
    }

    public void ReturnResource(ITakeResource resource)
    {
        if (resource != null && _busyResources.Contains(resource) == false && _freeResources.Contains(resource) == false)
            _freeResources.Add(resource);
    }

    public void MarkResourceAsDelivered(ITakeResource resource)
    {
        _busyResources.Remove(resource);

        if (resource is Resource resourceObj && _resourcePool != null)
            _resourcePool.ReturnResource(resourceObj);
    }

    private void ScanForNewResources()
    {
        if (_resourcePool != null)
        {
            var activeResources = _resourcePool.ActiveResourceList;
            int registeredCount = 0;

            foreach (var resource in activeResources)
            {
                if (_busyResources.Contains(resource) == false && _freeResources.Contains(resource) == false && IsResourceInRange(resource))
                {
                    RegisterResource(resource);
                    registeredCount++;
                }
            }
        }
    }

    private bool IsResourceInRange(ITakeResource resource)
    {
        if (resource is MonoBehaviour behaviour)
        {
            float distance = Vector3.Distance(transform.position, behaviour.transform.position);

            return distance <= _scanRadius;
        }
        return true;
    }

    private bool IsResourceAccessible(ITakeResource resource) => true;
}