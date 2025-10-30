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
<<<<<<< HEAD
<<<<<<< HEAD
    private ResourceHub _resourceHub;
=======
    private ResourceFactory _resourceFactory;
>>>>>>> parent of ba2fb3c (add mover)
=======
    private ResourceFactory _resourceFactory;
>>>>>>> parent of ba2fb3c (add mover)

    public List<ITakeResource> ActiveResourceList => new List<ITakeResource>(_activeResources);

    protected override void Awake()
    {
        _activeResources = new List<ITakeResource>();
        base.Awake();

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

        _resourceHub?.RegisterResource(resource);
        _activeResources.Add(resource);
    }

    protected override void OnObjectReturn(Resource resource)
    {
        base.OnObjectReturn(resource);

        _resourceHub?.UnregisterResource(resource);
        _activeResources.Remove(resource);
    }

    protected override void ResetObject(Resource resource)
    {
        base.ResetObject(resource);
 
        resource.transform.SetParent(null);
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

    public void ReturnResource(Resource resource)
    {
        ReturnObject(resource);
    }

    private IEnumerator SpawnResourcesRoutine()
    {
        yield return new WaitForSeconds(1f);

        var wait = new WaitForSeconds(_spawnInterval);

        while (enabled)
        {
            yield return wait;
            SpawnResource();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;

        return new Vector3(randomCircle.x, 0.5f, randomCircle.y);
    }
}