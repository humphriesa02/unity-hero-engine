using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] GameObject ragdollObject;

    [SerializeField] private float camShakeTime = 0.2f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void OnHit(HurtContext context)
    {
        currentHealth -= context.data.damage;
        
        PlayHitVFX(context.point);
        PlayHitAnim();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(context);
        }
    }

    public void CameraShake(float intensity)
    {
        CameraController.Instance.CameraShake(intensity, camShakeTime);
    }

    public void PlayHitVFX(Vector3 hitPosition)
    {
        if (!hitVFXPrefab) return;
        GameObject hit = Instantiate(hitVFXPrefab, hitPosition, Quaternion.identity);
        Destroy(hit, 3.0f);
    }

    public void PlayHitAnim()
    {
        if (anim != null)
        {
            anim.SetTrigger("damage");
        }
    }

    private void ApplyKnockback(HurtContext context)
    {
        
    }

    public void Die()
    {
        if (ragdollObject) Instantiate(ragdollObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
