using UnityEngine;

public class CoinLootBehavior : ILootBehavior
{
    public void PlayCollectionSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.coinPickup);
        }
    }

    public void ApplyLootEffect(GameObject collector, int value, LootDrop lootDropComponent)
    {
        LevelManager levelManager = lootDropComponent.GetComponent<LevelManager>();
        if (levelManager == null)
        {
            levelManager = Object.FindObjectOfType<LevelManager>();
        }
        
        if (levelManager != null)
        {
            levelManager.AddScore(value);
        }
        else
        {
            Debug.LogWarning("LevelManager not found - cannot add score");
        }
    }

    public LootType GetLootType()
    {
        return LootType.Coin;
    }
}