using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
<<<<<<< HEAD:Assets/Skripts/Unit/Unit.cs
    [Header("Settings")]
    [SerializeField] private bool _isBuilder = false;

    private Base _base;
    private Mover _mover;
    private Resource _carriedResource;
    private bool _isBusy = false;

    public bool IsAvailable => !_isBusy;
    public bool IsBuilder => _isBuilder;
=======
    private const float MinDistance = 0.5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _resourceCarryVisual;

    private Vector3 _targetPosition;
    private Vector3 _basePosition;
    private ITakeResource _carriedResource;

    private bool _isMoving = false;
    private bool _hasResource = false;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_isMoving && !_hasResource;

    public event Action<Unit> BecameAvailable;
    public event Action<Unit> BecameBusy;
    public event Action<Unit, ITakeResource> ResourceDelivered;
>>>>>>> parent of ba2fb3c (add mover):Assets/Skripts/Unit.cs

    private void Update()
    {
        if (_isMoving)
            MoveToTarget();
    }

    public void Initialize(Base unitBase)
    {
<<<<<<< HEAD:Assets/Skripts/Unit/Unit.cs
        _base = unitBase;
    }

    public void SetBase(Base newBase)
    {
        _base = newBase;
    }

    public void CollectResource(Resource resource)
    {
        if (_isBusy) return;
=======
        if (IsAvailable == false)
            return;

        if (resource != null)
        {
            _targetPosition = new Vector3(resource.Position.x, transform.position.y, resource.Position.z);
            _carriedResource = resource;
            _isMoving = true;
            _basePosition = basePosition;

            BecameBusy?.Invoke(this);
        }
    }

    public void PickUpResource()
    {
        if (_carriedResource == null)
            return;

        _hasResource = true;

        var resourceMono = _carriedResource as MonoBehaviour;
        resourceMono.transform.SetParent(_resourceCarryVisual);
        resourceMono.transform.localPosition = Vector3.zero;
        resourceMono.transform.localRotation = Quaternion.identity;

        if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        _targetPosition = new Vector3(_basePosition.x, transform.position.y, _basePosition.z);
        _isMoving = true;
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
            _carriedResource.Collect();
            ResourceDelivered?.Invoke(this, _carriedResource);
            _carriedResource = null;
        }
>>>>>>> parent of ba2fb3c (add mover):Assets/Skripts/Unit.cs

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