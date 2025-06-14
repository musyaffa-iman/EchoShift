using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 5f;
    
    private Rigidbody2D rb;
    private ObjectPool parentPool;
    private GameObject shooter;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        StartCoroutine(DisableAfterTime());
    }
    
    public void Initialize(Vector2 direction, GameObject shooter, ObjectPool pool)
    {
        this.shooter = shooter;
        this.parentPool = pool;
        rb.velocity = direction.normalized * speed;
    }
    
    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == shooter) return;
        
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.GetHit(damage, shooter);
            }
            ReturnToPool();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Prop"))
        {
            ReturnToPool();
        }
    }
    
    private void ReturnToPool()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        StopAllCoroutines();
        
        if (parentPool != null)
        {
            parentPool.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        StopAllCoroutines();
    }
}
