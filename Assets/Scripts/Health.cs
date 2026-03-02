using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Animator anim;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void OnHit(HurtContext context)
    {
        currentHealth -= context.data.damage;
        if (anim != null)
        {
            anim.SetTrigger("Damage");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(context);
        }
    }

    private void ApplyKnockback(HurtContext context)
    {
        
    }

    public void Die()
    {
        // Play death animation, disable character, etc.
        Debug.Log($"{gameObject.name} has died!");
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        Destroy(gameObject);
    }
}
