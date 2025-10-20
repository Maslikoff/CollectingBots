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

    public ResourceType Type => _resourceType;
    public Vector3 Position => transform.position;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    public void Collect()
    {
        _renderer.enabled = false;
        _collider.enabled = false;

        _rigidbody.isKinematic = true;
        _rigidbody.detectCollisions = false;
    }

    public void ReturnToPool()
    {
        _renderer.enabled = true;
        _collider.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;

        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}