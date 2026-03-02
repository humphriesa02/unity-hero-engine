using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates on hit
/// </summary>
public struct HurtContext
{
    public readonly HitData data;
    public readonly GameObject instigator;
    public readonly Vector3 point;
    public int numHitsCurrent;
    public float timeSinceLastHit;

    public HurtContext(HitData data, GameObject instigator, Vector3 point, int numHitsCurrent = 0, float timeSinceLastHit = 0f)
    {
        this.data = data;
        this.instigator = instigator;
        this.point = point;
        this.numHitsCurrent = numHitsCurrent;
        this.timeSinceLastHit = timeSinceLastHit;
    }
}

public class Hurtbox : MonoBehaviour
{
    Dictionary<Hitbox, HurtContext> damageObjectToHurtContext = new();
    [SerializeField] private HitData data;
    [SerializeField] private Collider hitCollider;

    void Start()
    {
        hitCollider.enabled = false;
    }

    public void StartDealDamage()
    {
        hitCollider.enabled = true;
        damageObjectToHurtContext.Clear();
    }

    public void EndDealDamage()
    {
        hitCollider.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Hitbox hitbox))
        {
            if (damageObjectToHurtContext.ContainsKey(hitbox))
            {
                // Process the hit
                HurtContext context = damageObjectToHurtContext[hitbox];
                context.timeSinceLastHit += Time.deltaTime;

                if (context.numHitsCurrent < context.data.hitCount && context.timeSinceLastHit >= context.data.hitDelay)
                {
                    context.numHitsCurrent++;
                    context.timeSinceLastHit = 0f;
                    Debug.Log("Hit " + hitbox.name + " for " + context.data.damage + " damage. Hit count: " + context.numHitsCurrent);
                    hitbox.OnHit(context);
                }
                damageObjectToHurtContext[hitbox] = context;
            }
            else
            {
                // Generate new context
                HurtContext context = new(data, gameObject, other.ClosestPoint(transform.position));
                damageObjectToHurtContext.Add(hitbox, context);
            } 
        }
    }
}
