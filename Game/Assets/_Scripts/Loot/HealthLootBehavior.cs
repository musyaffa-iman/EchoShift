using UnityEngine;

public class HealthLootBehavior : ILootBehavior
{
    public void PlayCollectionSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.potionPickup);
        }
    }

    public void ApplyLootEffect(GameObject collector, int value, LootDrop lootDropComponent)
    {
        Health playerHealth = collector.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.GetHealed(value);
        }
        else
        {
            Debug.LogWarning("No Health component found on collector");
        }
    }

    public LootType GetLootType()
    {
        return LootType.Health;
    }
}