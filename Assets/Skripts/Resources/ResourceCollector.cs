using System;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    private int _totalResources = 0;
    private int _resourcesSpent = 0;

    public int TotalResources => _totalResources;
    public int AvailableResources => _totalResources - _resourcesSpent;

    public event Action<int> ResourcesChanged;

    public void AddResource()
    {
        _totalResources++;
        ResourcesChanged?.Invoke(AvailableResources);
    }

    public bool TrySpendResources(int amount)
    {
        if (AvailableResources >= amount)
        {
            _resourcesSpent += amount;
            ResourcesChanged?.Invoke(AvailableResources);

            return true;
        }

        return false;
    }

    public bool TrySpendResourcesForBuilding(int amount)
    {
        if (TotalResources >= amount)
        {
            _resourcesSpent += amount;
            ResourcesChanged?.Invoke(AvailableResources);
            Debug.Log($"Spent {amount} for building. Total: {TotalResources}, Available: {AvailableResources}");
            return true;
        }
        return false;
    }

    public void ResetResources()
    {
        _totalResources = 0;
        _resourcesSpent = 0;
        ResourcesChanged?.Invoke(0);
    }
}