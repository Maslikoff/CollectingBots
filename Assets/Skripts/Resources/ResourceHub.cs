using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHub : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 20f;
    [SerializeField] private ResourcePool _resourcePool;

    private Dictionary<ResourceType, HashSet<ITakeResource>> _freeResources = new Dictionary<ResourceType, HashSet<ITakeResource>>();
    private Dictionary<ResourceType, HashSet<ITakeResource>> _busyResources = new Dictionary<ResourceType, HashSet<ITakeResource>>();

    public event Action ResourcesUpdated;

    private void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            _freeResources[type] = new HashSet<ITakeResource>();
            _busyResources[type] = new HashSet<ITakeResource>();
        }
    }

    public void RegisterResource(ITakeResource resource)
    {
        ResourceType resourceType = GetResourceType(resource);
        if (_busyResources[resourceType].Contains(resource) == false)
            _freeResources[resourceType].Add(resource);
    }

    public void UnregisterResource(ITakeResource resource)
    {
        ResourceType resourceType = GetResourceType(resource);
        _freeResources[resourceType].Remove(resource);
        _busyResources[resourceType].Remove(resource);
    }

    public ITakeResource GetAvailableResource(ResourceType resourceType)
    {
        if (_freeResources[resourceType].Count == 0)
            ScanForNewResources();

        foreach (var resource in _freeResources[resourceType])
        {
            if (resource != null && IsResourceAccessible(resource) && GetResourceType(resource) == resourceType)
            {
                _freeResources[resourceType].Remove(resource);
                _busyResources[resourceType].Add(resource);

                return resource;
            }
        }

        return null;
    }

    public void ReturnResource(ITakeResource resource)
    {
        if (resource != null)
        {
            ResourceType resourceType = GetResourceType(resource);
            _busyResources[resourceType].Remove(resource);

            if (resource is MonoBehaviour behaviour && behaviour.gameObject.activeInHierarchy)
                _freeResources[resourceType].Add(resource);
        }
    }

    public void MarkResourceAsDelivered(ITakeResource resource)
    {
        ResourceType resourceType = GetResourceType(resource);
        _busyResources[resourceType].Remove(resource);

        if (resource is Resource resourceObj && _resourcePool != null)
            _resourcePool.ReturnResource(resourceObj);
    }

    private void ScanForNewResources()
    {
        if (_resourcePool != null)
        {
            var activeResources = _resourcePool.ActiveResourceList;

            foreach (var resource in activeResources)
            {
                ResourceType resourceType = GetResourceType(resource);

                if (_busyResources[resourceType].Contains(resource) == false &&
                    _freeResources[resourceType].Contains(resource) == false &&
                    IsResourceInRange(resource))
                {
                    RegisterResource(resource);
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

    private ResourceType GetResourceType(ITakeResource resource)
    {
        if (resource is MonoBehaviour behaviour)
        {
            var resourceComponent = behaviour.GetComponent<Resource>();

            if (resourceComponent != null)
                return resourceComponent.Type;
        }

        return ResourceType.Nothing; 
    }
}