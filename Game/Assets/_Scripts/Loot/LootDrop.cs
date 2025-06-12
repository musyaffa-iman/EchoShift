using UnityEngine;
using System.Collections;

public class LootDrop : MonoBehaviour, ICollectable
{
    [SerializeField] private LootItem lootData;
    [SerializeField] private float collectDistance = 1f;
    [SerializeField] private float moveSpeed = 3f;
    
    private Transform player;
    private bool isCollected = false;
    private ILootBehavior lootBehavior;
    
    private void Start()
    {
        InitializeLootBehavior();
        StartCoroutine(FindPlayerCoroutine());
    }
    
    private void InitializeLootBehavior()
    {
        if (lootData != null)
        {
            lootBehavior = LootBehaviorFactory.CreateBehavior(lootData.lootType);
            
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && lootData.itemSprite != null)
            {
                spriteRenderer.sprite = lootData.itemSprite;
            }
        }
    }
    
    private IEnumerator FindPlayerCoroutine()
    {
        while (player == null)
        {
            if (Player.CurrentPlayer != null)
            {
                player = Player.CurrentPlayer;
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void Update()
    {
        if (isCollected || player == null || lootData == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= collectDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            
            if (distance <= 0.3f)
            {
                Collect(player.gameObject);
            }
        }
    }
    
    public void Collect(GameObject collector)
    {
        if (isCollected || lootBehavior == null) return;
        
        isCollected = true;
        
        lootBehavior.PlayCollectionSound();
        lootBehavior.ApplyLootEffect(collector, GetValue(), this);
        
        Destroy(gameObject);
    }
    
    public int GetValue()
    {
        return lootData != null ? lootData.value : 0;
    }
    
    public LootType GetLootType()
    {
        return lootData != null ? lootData.lootType : LootType.Coin;
    }
    
    public void Initialize(LootItem lootItem)
    {
        if (lootItem == null) return;
        
        lootData = lootItem;
        InitializeLootBehavior();
    }
}