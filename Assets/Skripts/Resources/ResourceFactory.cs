using System.Collections.Generic;
using UnityEngine;

public class ResourceFactory : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 20f;
    [SerializeField] private ResourcePool _resourcePool;

<<<<<<< HEAD
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
=======
    private HashSet<ITakeResource> _freeResources = new HashSet<ITakeResource>();
    private HashSet<ITakeResource> _busyResources = new HashSet<ITakeResource>();

    public void RegisterResource(ITakeResource resource)
    {
        if (_busyResources.Contains(resource) == false)
            _freeResources.Add(resource);
>>>>>>> parent of ba2fb3c (add mover)
    }

    public void UnregisterResource(ITakeResource resource)
    {
<<<<<<< HEAD
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
=======
        _freeResources.Remove(resource);
        _busyResources.Remove(resource);
    }

    public ITakeResource GetAvailableResource()
    {
        if (_freeResources.Count == 0)
            ScanForNewResources();

        foreach (var resource in _freeResources)
        {
            if (resource != null && IsResourceAccessible(resource))
            {
                _freeResources.Remove(resource);
                _busyResources.Add(resource);
>>>>>>> parent of ba2fb3c (add mover)

                return resource;
            }
        }

        return null;
    }

    public void ReturnResource(ITakeResource resource)
    {
        if (resource != null)
        {
<<<<<<< HEAD
            ResourceType resourceType = GetResourceType(resource);
            _busyResources[resourceType].Remove(resource);

            if (resource is MonoBehaviour behaviour && behaviour.gameObject.activeInHierarchy)
                _freeResources[resourceType].Add(resource);
=======
            _busyResources.Remove(resource);

            if (resource is MonoBehaviour behaviour && behaviour.gameObject.activeInHierarchy)
                _freeResources.Add(resource);
>>>>>>> parent of ba2fb3c (add mover)
        }
    }

    public void MarkResourceAsDelivered(ITakeResource resource)
    {
<<<<<<< HEAD
        ResourceType resourceType = GetResourceType(resource);
        _busyResources[resourceType].Remove(resource);
=======
        _busyResources.Remove(resource);
>>>>>>> parent of ba2fb3c (add mover)

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
<<<<<<< HEAD
<<<<<<< HEAD:Assets/Skripts/Resources/ResourceHub.cs
                ResourceType resourceType = GetResourceType(resource);

                if (_busyResources[resourceType].Contains(resource) == false &&
                    _freeResources[resourceType].Contains(resource) == false &&
                    IsResourceInRange(resource))
                {
                    RegisterResource(resource);
                }
=======
                if (_busyResources.Contains(resource) == false && _freeResources.Contains(resource) == false)
                    RegisterResource(resource);
>>>>>>> parent of ba2fb3c (add mover):Assets/Skripts/Resources/ResourceFactory.cs
=======
                if (_busyResources.Contains(resource) == false && _freeResources.Contains(resource) == false)
                    RegisterResource(resource);
>>>>>>> parent of ba2fb3c (add mover)
            }
        }
    }

    private bool IsResourceAccessible(ITakeResource resource) => true;
<<<<<<< HEAD
<<<<<<< HEAD:Assets/Skripts/Resources/ResourceHub.cs

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
=======
}
>>>>>>> parent of ba2fb3c (add mover):Assets/Skripts/Resources/ResourceFactory.cs
=======
}
>>>>>>> parent of ba2fb3c (add mover)
