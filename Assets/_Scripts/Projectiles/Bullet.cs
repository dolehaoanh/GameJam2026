using UnityEngine;

/// <summary>
/// Bullet script - Quản lý viên đạn (di chuyển, va chạm, damage)
/// Được instantiate trực tiếp bởi EnemyRanged hoặc các weapon
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("--- BULLET STATS ---")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f; // Tự động destroy sau thời gian
    [SerializeField] private bool destroyOnHit = true;

    [Header("--- DEBUG ---")]
    [SerializeField] private bool showDebugLogs = false;

    private float spawnTime;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        spawnTime = Time.time;

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        // Tự động destroy khi hết lifetime
        if (Time.time - spawnTime > lifetime)
        {
            if (showDebugLogs) Debug.Log($"⏱️ Bullet lifetime expired, destroying...");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Va chạm với player
        if (collision.CompareTag("Player"))
        {
            if (showDebugLogs) Debug.Log($"💥 Bullet hit Player!");
            OnBulletHit(collision);
            return;
        }

        // Va chạm với enemy (để tránh friendly fire)
        // Có thể bỏ qua nếu muốn
    }

    void OnBulletHit(Collider2D collision)
    {
        // Gây damage nếu target có EnemyBaseFSM hoặc health component
        EnemyBaseFSM enemy = collision.GetComponent<EnemyBaseFSM>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (showDebugLogs) Debug.Log($"💢 Enemy took {damage} damage");
        }

        // Destroy hoặc Instantiate explosion effect nếu cần
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    // --- PUBLIC API ---

    /// <summary>
    /// Thiết lập damage cho bullet
    /// </summary>
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Thiết lập velocity trực tiếp (thường dùng bằng Rigidbody2D.linearVelocity)
    /// </summary>
    public void SetVelocity(Vector2 velocity)
    {
        if (rb != null)
            rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Rotate bullet theo hướng
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public float GetDamage() => damage;
}
