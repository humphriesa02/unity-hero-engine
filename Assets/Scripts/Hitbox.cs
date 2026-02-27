using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [SerializeField] UnityEvent onHitEvent;
    public void OnHit(HurtContext context)
    {
        onHitEvent?.Invoke();
    }
}
