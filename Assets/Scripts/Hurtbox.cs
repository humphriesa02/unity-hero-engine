using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    List<GameObject> hasDealtDamage = new();

    [SerializeField] private float weaponDamage;
    private CapsuleCollider hitCollider;

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
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        hitCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered");
        if (!hasDealtDamage.Contains(other.transform.gameObject))
        {
            Debug.Log("Damage applied to: " + other.transform.gameObject.name);
            hasDealtDamage.Add(other.transform.gameObject);
        }
    }
}
