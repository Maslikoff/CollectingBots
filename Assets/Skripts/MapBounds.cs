using UnityEngine;

public class MapBounds : MonoBehaviour
{
    [SerializeField] private Vector3 _mapCenter = Vector3.zero;
    [SerializeField] private Vector3 _mapSize = new Vector3(50f, 0f, 50f);

    public bool IsPositionInsideMap(Vector3 position)
    {
        Bounds mapBounds = new Bounds(_mapCenter, _mapSize);

        return mapBounds.Contains(position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_mapCenter, _mapSize);
    }
}