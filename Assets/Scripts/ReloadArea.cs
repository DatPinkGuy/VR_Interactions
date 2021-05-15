using UnityEngine;

public class ReloadArea : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    public Collider Collider => _collider;
}
