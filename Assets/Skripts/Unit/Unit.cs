using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Builder))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _resourceCarryVisual;

    private Mover _mover;
    private Builder _builder;
    private ITakeResource _carriedResource;
    private Base _base;
    private Vector3 _basePosition;
    private Coroutine _collectionCoroutine;
    private Coroutine _buildCoroutine;

    private bool _hasResource = false;
    private bool _movementCompleted;

    private Action _currentBuildCallback;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_mover.IsMoving && !_hasResource && !_builder.IsBuilding;
    public bool IsBuilder => _builder != null;

    public event Action<Unit> BecameAvailable;
    public event Action<Unit> BecameBusy;
    public event Action<Unit, ITakeResource> ResourceDelivered;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _builder = GetComponent<Builder>();
    }

    private void OnDestroy()
    {
        if (_collectionCoroutine != null)
            StopCoroutine(_collectionCoroutine);

        if (_buildCoroutine != null)
            StopCoroutine(_buildCoroutine);

        if (_builder != null)
        {
            _builder.BuildCompleted -= OnBuildCompleted;
            _builder.BuildStarted -= OnBuildStarted;
        }
    }

    public void SetBase(Base newBase)
    {
        _base = newBase;
        _basePosition = newBase.transform.position;
    }

    public void AssignToCollectResource(ITakeResource resource, Vector3 basePosition)
    {
        if (IsAvailable == false)
            return;

        _carriedResource = resource;
        _basePosition = basePosition;

        BecameBusy?.Invoke(this);

        _collectionCoroutine = StartCoroutine(CollectionRoutine());
    }

    public void BuildNewBase(Vector3 buildPosition, Action<Unit> onBuildComplete)
    {
        if (IsAvailable == false || IsBuilder == false)
            return;

        _currentBuildCallback = () => onBuildComplete?.Invoke(this);
        _buildCoroutine = StartCoroutine(BuildRoutine(buildPosition));

        BecameBusy?.Invoke(this);
    }

    private IEnumerator BuildRoutine(Vector3 buildPosition)
    {
        yield return MoveToPositionRoutine(buildPosition);

        _builder.StartBuilding(_currentBuildCallback);
    }

    private void OnBuildStarted(Builder builder)
    {
        Debug.Log("Build process started");
    }

    private void OnBuildCompleted(Builder builder)
    {
        _currentBuildCallback = null;
        _buildCoroutine = null;
        BecameAvailable?.Invoke(this);
    }

    private IEnumerator CollectionRoutine()
    {
        yield return MoveToPositionRoutine(_carriedResource.Position);

        PickUpResource();

        yield return MoveToPositionRoutine(_basePosition);

        DepositResource();

        _collectionCoroutine = null;
        BecameAvailable?.Invoke(this);
    }

    private IEnumerator MoveToPositionRoutine(Vector3 targetPosition)
    {
        _movementCompleted = false;
        _mover.MoveTo(targetPosition, OnMovementCompleted);

        yield return new WaitUntil(() => _movementCompleted);
    }

    private void OnMovementCompleted()
    {
        _movementCompleted = true;
    }

    private void PickUpResource()
    {
        if (_carriedResource == null)
            return;

        _hasResource = true;
        var resourceMono = _carriedResource as MonoBehaviour;

        if (resourceMono != null)
        {
            resourceMono.transform.SetParent(_resourceCarryVisual);
            resourceMono.transform.localPosition = Vector3.zero;
            resourceMono.transform.localRotation = Quaternion.identity;

            if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
    }

    private void DepositResource()
    {
        if (_carriedResource != null)
        {
            var resourceToDeliver = _carriedResource;
            _carriedResource.Collect();

            ResourceDelivered?.Invoke(this, resourceToDeliver);

            _carriedResource = null;
        }

        _hasResource = false;
    }
}