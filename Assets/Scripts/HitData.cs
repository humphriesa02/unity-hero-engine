using UnityEngine;

/// <summary>
/// Attaches to a hurtbox. What will apply to a hitbox.
/// </summary>
[CreateAssetMenu(fileName = "HitData", menuName = "ScriptableObjects/HitData", order = 1)]
public class HitData : ScriptableObject
{
    public float damage;
    public float knockBack;
    public int hitCount = 1; // Allows for multihits
    public float hitDelay = 0.25f;
}
