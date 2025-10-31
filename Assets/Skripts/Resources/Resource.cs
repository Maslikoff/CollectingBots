using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour, ITakeResource
{
    [SerializeField] private ResourceType _resourceType;

    private Rigidbody _rigidbody;
    private Renderer _renderer;
    private Collider _collider;
    private Transform _originalParent;

    private bool _isCollected = false;
    private bool _isClaimed = false;

    public ResourceType Type => _resourceType;
    public Vector3 Position => transform.position;
    public bool IsCollected => _isCollected;
    public bool IsClaimed => _isClaimed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();

        _originalParent = transform.parent;
    }

    public void Claim()
    {
        _isClaimed = true;
    }

    public void Release()
    {
        _isClaimed = false;
    }

    public void Collect()
    {
        if (_isCollected) 
            return;

        _isCollected = true;
        _isClaimed = true;

        _collider.enabled = false;

        _rigidbody.isKinematic = true;
        _rigidbody.detectCollisions = false;
    }

    public void ReturnToPool()
    {
        _isCollected = false;
        _isClaimed = false;

        _renderer.enabled = true;
        _collider.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;

        transform.SetParent(_originalParent);
        gameObject.SetActive(false);
    }

    public void ResetToWorld()
    {
        _isCollected = false;
        _isClaimed = false;

        _renderer.enabled = true;
        _collider.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;

        transform.SetParent(_originalParent);
        gameObject.SetActive(true);
    }
}