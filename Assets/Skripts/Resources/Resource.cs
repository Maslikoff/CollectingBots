using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour, ITakeResource
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;

    private Rigidbody _rigidbody;

    public ResourceType Type => _resourceType;
    public bool IsCollected { get; set; }
    public Vector3 Position => transform.position;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Collect()
    {
        IsCollected = true;
        _renderer.enabled = false;
        _collider.enabled = false;
    }

    public void ReturnToPool()
    {
        IsCollected = false;
        _renderer.enabled = true;
        _collider.enabled = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit))
        {
            _rigidbody.isKinematic = true;

            if (unit != null && unit.TargetResource == this && !IsCollected)
                unit.PickUpResource();
        }
    }
}