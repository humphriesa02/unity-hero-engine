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
            Debug.Log($"Ouch! I got hit by {context.instigator.name} for {context.data.damage} damage at point {context.point}. Current health: {currentHealth}");
        }
    }

    private void ApplyKnockback(HurtContext context)
    {
        // TODO Implement knockback logic here, e.g., apply a force to the character's Rigidbody
        // based on context.data.knockBack and the direction from context.point to the character.
    }

    public void Die()
    {
        // Play death animation, disable character, etc.
        Debug.Log($"{gameObject.name} has died!");
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
    }
}
