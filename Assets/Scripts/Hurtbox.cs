using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage = new();

    [SerializeField] private float weaponLength;
    [SerializeField] private float weaponDamage;
    [SerializeField] private LayerMask hitMask;

    void Start()
    {
        canDealDamage = false;
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, hitMask))
            {
                if (!hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    Debug.Log("Damage applied: " + weaponDamage);
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position-transform.up * weaponLength);
    }
}
