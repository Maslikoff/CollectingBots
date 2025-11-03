using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Metall
}

public interface ITakeResource
{
    ResourceType Type { get; }
    Vector3 Position { get; }

    void Collect(Transform carryTransform);
    void ReturnToPool();
}