using UnityEngine;
using System.Collections.Generic;

public class PropDestroyer : MonoBehaviour
{
    [SerializeField] private Prop propData;
    [SerializeField] private GameObject lootDropPrefab;
    
    private Destructible destructibleComponent;
    
    private void Awake()
    {
        destructibleComponent = GetComponent<Destructible>();
        if (destructibleComponent != null)
        {
            destructibleComponent.OnDestroyed.AddListener(OnPropDestroyed);
        }
    }
    
    private void OnPropDestroyed(GameObject destroyer)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.objectBreak);
        if (propData != null && propData.possibleLootDrops.Count > 0)
        {
            DropLoot();
        }
        
        Destroy(gameObject);
    }
    
    private void DropLoot()
    {        
        foreach (LootItem lootItem in propData.possibleLootDrops)
        {
            if (lootItem == null) continue;
            
            float randomValue = Random.Range(0f, 1f);
            
            if (randomValue <= lootItem.dropChance)
            {
                if (lootDropPrefab == null) continue;
                
                Vector3 dropPosition = transform.position + new Vector3(0.5f, 0.5f, 0f);
                GameObject lootDrop = Instantiate(lootDropPrefab, dropPosition, Quaternion.identity);
                
                LootDrop lootComponent = lootDrop.GetComponent<LootDrop>();
                if (lootComponent != null)
                {
                    lootComponent.Initialize(lootItem);
                }
            }
        }
    }
    
    public void SetPropData(Prop prop)
    {
        propData = prop;
    }
    
    public void SetLootDropPrefab(GameObject prefab)
    {
        lootDropPrefab = prefab;
    }
    
    private void OnDestroy()
    {        
        if (destructibleComponent != null)
        {
            destructibleComponent.OnDestroyed.RemoveListener(OnPropDestroyed);
        }
    }
}