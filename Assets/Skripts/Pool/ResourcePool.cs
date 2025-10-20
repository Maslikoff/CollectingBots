using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : ObjectPool<Resource>
{
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private float _spawnRadius = 20f;
    [SerializeField] private int _maxResourcesOnMap = 10;

    private List<ITakeResource> _activeResources;
    private Coroutine _spawnCoroutine;

    public List<ITakeResource> ActiveResourceList => _activeResources;

    protected override void Awake()
    {
        _activeResources = new List<ITakeResource>();
        base.Awake();
    }

    private void Start()
    {
        _spawnCoroutine = StartCoroutine(SpawnResourcesRoutine());
    }

    private void OnDestroy()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
    }

    protected override void OnObjectGet(Resource resource)
    {
        base.OnObjectGet(resource);

        resource.IsCollected = false;
        if (resource.TryGetComponent<Renderer>(out var renderer))
            renderer.enabled = true;
        if (resource.TryGetComponent<Collider>(out var collider))
            collider.enabled = true;

        _activeResources.Add(resource);
    }

    protected override void OnObjectReturn(Resource resource)
    {
        base.OnObjectReturn(resource);
        _activeResources.Remove(resource);
        resource.gameObject.SetActive(false);
    }

    protected override void ResetObject(Resource resource)
    {
        base.ResetObject(resource);
        resource.IsCollected = false;
    }

    public void SpawnResource()
    {
        if (_activeResources.Count < _maxResourcesOnMap)
        {
            Resource resource = GetObject();
            Vector3 spawnPosition = GetRandomSpawnPosition();
            resource.transform.position = spawnPosition;
        }
    }

    public List<ITakeResource> GetAvailableResources()
    {
        List<ITakeResource> available = new List<ITakeResource>();

        for (int i = _activeResources.Count - 1; i >= 0; i--)
        {
            ITakeResource resource = _activeResources[i];

            if (resource == null || resource.IsCollected)
            {
                _activeResources.RemoveAt(i);
                continue;
            }

            available.Add(resource);
        }

        return available;
    }

    private IEnumerator SpawnResourcesRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (enabled)
        {
            yield return new WaitForSeconds(_spawnInterval);
            SpawnResource();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public void ReturnResource(Resource resource)
    {
        ReturnObject(resource);
    }
}