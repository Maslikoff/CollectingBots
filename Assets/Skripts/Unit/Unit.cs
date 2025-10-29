using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Unit : MonoBehaviour, IResourceTypeListener
{
    [SerializeField] private Transform _resourceCarryVisual;

    private ITakeResource _carriedResource;
    private ResourceType _currentResourceType;
    private Vector3 _basePosition;
    private Mover _mover;
    private Coroutine _collectionCoroutine;

    private bool _hasResource = false;
    private bool _movementCompleted;
    private bool _isBuilder = true;

    public ITakeResource CarriedResource => _carriedResource;
    public bool IsAvailable => !_mover.IsMoving && !_hasResource;
    public bool IsBusy => _mover.IsMoving || _hasResource || _collectionCoroutine != null;
    public bool IsBuilder => _isBuilder;

    public event Action<Unit> BecameAvailable;
    public event Action<Unit> BecameBusy;
    public event Action<Unit, ITakeResource> ResourceDelivered;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    private void OnDestroy()
    {
        if (_collectionCoroutine != null)
            StopCoroutine(_collectionCoroutine);
    }

    public void OnResourceTypeChanged(ResourceType newResourceType)
    {
        _currentResourceType = newResourceType;

        if (_hasResource && _carriedResource != null)
        {
            ResourceType carriedType = GetResourceType(_carriedResource);

            if (carriedType != _currentResourceType)
            {
                if (_collectionCoroutine != null)
                {
                    StopCoroutine(_collectionCoroutine);
                    _collectionCoroutine = null;
                }

                ResetCarriedResource();
                BecameAvailable?.Invoke(this);
            }
        }
    }

    public void AssignToCollectResource(ITakeResource resource, Vector3 basePosition)
    {
        if (IsAvailable == false)
            return;

        if (resource != null)
        {
            ResourceType resourceType = GetResourceType(resource);

            if (resourceType != _currentResourceType)
                return;

            _carriedResource = resource;
            _basePosition = basePosition;

            _collectionCoroutine = StartCoroutine(CollectionRoutine());
            BecameBusy?.Invoke(this);
        }
    }

    public void ReturnToBase(Vector3 basePosition)
    {
        _basePosition = basePosition;

        if (_hasResource && _carriedResource != null)
        {
            if (_collectionCoroutine != null)
            {
                StopCoroutine(_collectionCoroutine);
                _collectionCoroutine = null;
            }

            _collectionCoroutine = StartCoroutine(ReturnToBaseWithResource());
        }
        else if(_hasResource == false && _carriedResource == null)
        {
            if (_collectionCoroutine != null)
            {
                StopCoroutine(_collectionCoroutine);
                _collectionCoroutine = null;
            }

            MoveToPosition(basePosition);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if (_collectionCoroutine != null)
        {
            StopCoroutine(_collectionCoroutine);
            _collectionCoroutine = null;
        }

        StartCoroutine(MoveToPositionRoutine(position));
    }

    public void StartBuildMission(Vector3 buildPosition, Action onBuildComplete)
    {
        if (IsAvailable == false || !_isBuilder)
            return;

        StartCoroutine(BuildMissionRoutine(buildPosition, onBuildComplete));
        BecameBusy?.Invoke(this);
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

    private IEnumerator ReturnToBaseWithResource()
    {
        yield return MoveToPositionRoutine(_basePosition);

        DepositResource();

        _collectionCoroutine = null;
        BecameAvailable?.Invoke(this);
    }

    private IEnumerator BuildMissionRoutine(Vector3 buildPosition, Action onBuildComplete)
    {
        yield return MoveToPositionRoutine(buildPosition);

        yield return new WaitForSeconds(2f);

        onBuildComplete?.Invoke();
        BecameAvailable?.Invoke(this);
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
        resourceMono.transform.SetParent(_resourceCarryVisual);
        resourceMono.transform.localPosition = Vector3.zero;
        resourceMono.transform.localRotation = Quaternion.identity;

        if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    private void DepositResource()
    {
        if (_carriedResource != null)
        {
            _carriedResource.Collect();
            ResourceDelivered?.Invoke(this, _carriedResource);
            _carriedResource = null;
        }

        _hasResource = false;
        BecameAvailable?.Invoke(this);
    }

    private void ResetCarriedResource()
    {
        if (_carriedResource != null)
        {
            var resourceMono = _carriedResource as MonoBehaviour;
            if (resourceMono != null)
            {
                resourceMono.transform.SetParent(null);

                if (resourceMono.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = false;
                    rb.detectCollisions = true;
                }

                if (_carriedResource is Resource resourceComponent)
                    resourceComponent.ReturnToPool();
            }
            _carriedResource = null;
        }
        _hasResource = false;
    }

    private ResourceType GetResourceType(ITakeResource resource)
    {
        if (resource is MonoBehaviour behaviour)
        {
            var resourceComponent = behaviour.GetComponent<Resource>();

            if (resourceComponent != null)
                return resourceComponent.Type;
        }

        return ResourceType.Wood;
    }
}