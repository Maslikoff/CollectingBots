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
    bool IsCollected { get; set; }
    Vector3 Position { get; }

    void Collect();
    void ReturnToPool();
}