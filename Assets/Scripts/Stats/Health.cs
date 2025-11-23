using UnityEngine;

public class Health : MonoBehaviour, IDamageable {
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageData damage) {
        float finalDamage = damage.baseDamage;

        // Example: apply modifiers by type
        if (damage.type == DamageType.Ethereal) finalDamage *= 0.8f;

        currentHealth -= finalDamage;
        Debug.Log($"{gameObject.name} took {finalDamage} {damage.type} damage.");

        if (currentHealth <= 0) Die();
    }

    private void Die() {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}