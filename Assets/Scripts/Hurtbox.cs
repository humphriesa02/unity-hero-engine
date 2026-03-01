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
    List<Hitbox> hitboxesToRemove = new();
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

    void Update()
    {
        if (damageObjectToHurtContext.Count > 0)
        {
            foreach (var kvp in damageObjectToHurtContext)
            {
                Debug.Log("Processing hit on " + kvp.Key.name);
                Hitbox hitbox = kvp.Key;
                HurtContext context = kvp.Value;
                context.timeSinceLastHit += Time.deltaTime;
                if (context.numHitsCurrent <= context.data.hitCount && context.timeSinceLastHit >= context.data.hitDelay)
                {
                    context.numHitsCurrent++;
                    context.timeSinceLastHit = 0f;
                    Debug.Log("Hit " + hitbox.name + " for " + context.data.damage + " damage. Hit count: " + context.numHitsCurrent);
                    hitbox.OnHit(context);
                    hitboxesToRemove.Add(hitbox);
                }
            }

            foreach (var hitbox in hitboxesToRemove)
            {
                damageObjectToHurtContext.Remove(hitbox);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Hitbox hitbox))
        {
            Debug.Log("Collided with " + hitbox.name);
            if (damageObjectToHurtContext.ContainsKey(hitbox))
                return;
            Debug.Log("Registering hit on " + hitbox.name);
            HurtContext context = new(data, gameObject, other.ClosestPoint(transform.position));
            damageObjectToHurtContext.Add(hitbox, context);
        }
    }
}
