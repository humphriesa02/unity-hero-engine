using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [SerializeField] UnityEvent<HurtContext> onHitEvent;
    public void OnHit(HurtContext context)
    {
        onHitEvent?.Invoke(context);
    }
}
