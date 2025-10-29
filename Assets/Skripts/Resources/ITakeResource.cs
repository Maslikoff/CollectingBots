using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Metall,
    Nothing
}

public interface ITakeResource
{
    ResourceType Type { get; }
    Vector3 Position { get; }

    void Collect();
    void ReturnToPool();
}