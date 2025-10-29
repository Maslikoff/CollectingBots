using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] private Renderer _flagRenderer;
    [SerializeField] private ParticleSystem _particle;

    public void SetColor(Color color)
    {
        _flagRenderer.material.color = color;
    }

    public void ShowPlacementEffect()
    {
        if (_particle != null)
        {
            _particle.Play();
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        ShowPlacementEffect();
    }
}