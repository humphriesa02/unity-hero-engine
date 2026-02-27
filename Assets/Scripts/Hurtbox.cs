using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates on hit
/// </summary>
public readonly struct HurtContext
{
    public readonly HitData data;
    public readonly GameObject instigator;
    public readonly Vector3 point;
    public readonly Vector3 direction;
    public readonly int attackId;

    public HurtContext(HitData data, GameObject instigator, Vector3 point, Vector3 direction, int attackId)
    {
        this.data = data;
        this.instigator = instigator;
        this.point = point;
        this.direction = direction;
        this.attackId = attackId;
    }
}

public class Hurtbox : MonoBehaviour
{
    HashSet<GameObject> objectsToDealDamage = new();
    [SerializeField] private HitData data;
    private CapsuleCollider hitCollider;
    private int numHits = 0;

    void Awake()
    {
        hitCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        hitCollider.enabled = false;
    }

    public void StartDealDamage()
    {
        hitCollider.enabled = true;
        objectsToDealDamage.Clear();
        numHits = 0;
    }

    public void EndDealDamage()
    {
        hitCollider.enabled = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Hitbox hitbox))
        {
            // HurtContext context = new HurtContext(
            //     data,
            //     gameObject,
            //     other.
            // )
            // hitbox.OnHit(weaponDamage);
            objectsToDealDamage.Add(collision.gameObject);
        }
    }
}
