using System.Collections.Generic;
using UnityEngine;

public class ResourceFactory : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 20f;
    [SerializeField] private ResourcePool _resourcePool;

    private HashSet<ITakeResource> _freeResources = new HashSet<ITakeResource>();
    private HashSet<ITakeResource> _busyResources = new HashSet<ITakeResource>();

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
        if (_freeResources.Count == 0)
            ScanForNewResources();

        foreach (var resource in _freeResources)
        {
            if (resource != null && IsResourceAccessible(resource))
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
        if (resource != null)
        {
            _busyResources.Remove(resource);

            if (resource is MonoBehaviour behaviour && behaviour.gameObject.activeInHierarchy)
                _freeResources.Add(resource);
        }
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

            foreach (var resource in activeResources)
            {
                if (_busyResources.Contains(resource) == false && _freeResources.Contains(resource) == false)
                    RegisterResource(resource);
            }
        }
    }

    private bool IsResourceAccessible(ITakeResource resource) => true;
}
