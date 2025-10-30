using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Unit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isBuilder = false;

    private Base _base;
    private Mover _mover;
    private Resource _carriedResource;
    private bool _isBusy = false;

    public bool IsAvailable => !_isBusy;
    public bool IsBuilder => _isBuilder;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    public void Initialize(Base unitBase)
    {
        _base = unitBase;
    }

    public void SetBase(Base newBase)
    {
        _base = newBase;
    }

    public void CollectResource(Resource resource)
    {
        if (_isBusy) return;

        _isBusy = true;
        _carriedResource = resource;

        _mover.MoveTo(resource.transform.position, OnReachedResource);
    }

    private void OnReachedResource()
    {
        // Забираем ресурс
        _carriedResource.Collect();

        // Возвращаемся на базу
        _mover.MoveTo(_base.transform.position, OnReachedBase);
    }

    private void OnReachedBase()
    {
        _base.AddResource(_carriedResource);
        _carriedResource = null;
        _isBusy = false;
    }

    public void BuildNewBase(Vector3 position, Action<Unit> onComplete)
    {
        if (!_isBuilder || _isBusy) return;

        _isBusy = true;
        _mover.MoveTo(position, () => OnReachedBuildSite(onComplete));
    }

    private void OnReachedBuildSite(Action<Unit> onComplete)
    {
        // Логика строительства базы
        _isBusy = false;
        onComplete?.Invoke(this);
    }
}