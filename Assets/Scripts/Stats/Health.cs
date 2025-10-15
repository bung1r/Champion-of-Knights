using UnityEngine;

public class Health : MonoBehaviour, IDamageable {
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageData damage) {
        float finalDamage = damage.baseDamage;

        // Example: apply modifiers by type
        if (damage.type == DamageType.Holy) finalDamage *= 0.8f;

        currentHealth -= finalDamage;
        Debug.Log($"{gameObject.name} took {finalDamage} {damage.type} damage.");

        if (currentHealth <= 0) Die();
    }

    private void Die() {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}