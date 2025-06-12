using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour
{
    [SerializeField] private int currentDurability, maxDurability;
    [SerializeField] private bool isDestroyed = false;
    
    public UnityEvent<GameObject> OnHit, OnDestroyed;

    private void Awake()
    {
        if (OnHit == null)
            OnHit = new UnityEvent<GameObject>();
        if (OnDestroyed == null)
            OnDestroyed = new UnityEvent<GameObject>();
    }

    public void InitializeDurability(int durabilityValue)
    {
        currentDurability = durabilityValue;
        maxDurability = durabilityValue;
        isDestroyed = false;
    }

    public void TakeDamage(int amount, GameObject sender)
    {
        if (isDestroyed) return;

        currentDurability -= amount;

        if (currentDurability > 0)
        {
            OnHit?.Invoke(sender);
        }
        else
        {
            isDestroyed = true;
            OnDestroyed?.Invoke(sender);
        }
    }
    
    public int GetCurrentDurability() => currentDurability;
    public int GetMaxDurability() => maxDurability;
    public bool IsDestroyed() => isDestroyed;
}