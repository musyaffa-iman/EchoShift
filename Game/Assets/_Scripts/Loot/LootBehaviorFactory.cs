using UnityEngine;

public static class LootBehaviorFactory
{
    public static ILootBehavior CreateBehavior(LootType lootType)
    {
        switch (lootType)
        {
            case LootType.Coin:
                return new CoinLootBehavior();
            case LootType.Health:
                return new HealthLootBehavior();
            default:
                Debug.LogWarning($"Unknown loot type: {lootType}. Defaulting to Coin behavior.");
                return new CoinLootBehavior();
        }
    }
}