using UnityEngine;

/// <summary>
/// Example: Projectile Pool Manager
/// Quản lý pool riêng cho các viên đạn, mũi tên, v.v.
/// Có thể dùng trực tiếp GeneralObjectPoolManager hoặc tạo wrapper như này
/// </summary>
public class ProjectilePoolManager : MonoBehaviour
{
    // Sử dụng GeneralObjectPoolManager singleton (nếu đã có trong scene)
    // Hoặc tạo pool riêng nếu cần behavior đặc biệt

    public static ProjectilePoolManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Spawn một projectile
    /// Ví dụ: ProjectilePoolManager.Instance.SpawnProjectile("arrow", position, direction);
    /// </summary>
    public GameObject SpawnProjectile(string projectileType, Vector3 position, Vector3 direction)
    {
        GameObject proj = GeneralObjectPoolManager.Instance.SpawnObject(projectileType, position);
        if (proj != null)
        {
            // Áp dụng direction (nếu có component Rigidbody2D hoặc script di chuyển)
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * 10f;
            }
        }
        return proj;
    }

    /// <summary>
    /// Trả projectile về pool
    /// </summary>
    public void ReturnProjectile(GameObject projectile, string projectileType)
    {
        GeneralObjectPoolManager.Instance.ReturnToPool(projectile, projectileType);
    }
}
