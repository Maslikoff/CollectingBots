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

    public ResourceType Type => _resourceType;
    public Vector3 Position => transform.position;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();

        _originalParent = transform.parent;
    }

    public void Collect(Transform carryTransform)
    {
        transform.SetParent(carryTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _collider.enabled = false;
        _rigidbody.isKinematic = true;
        _rigidbody.detectCollisions = false;
    }

    public void ReturnToPool()
    {
        transform.SetParent(_originalParent);

        _renderer.enabled = true;
        _collider.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;

        gameObject.SetActive(false);
    }

    public void ResetToWorld()
    {
        transform.SetParent(_originalParent);

        _renderer.enabled = true;
        _collider.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;

        gameObject.SetActive(true);
    }
}