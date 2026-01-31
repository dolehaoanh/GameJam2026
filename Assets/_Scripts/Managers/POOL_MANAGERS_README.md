# Pool Managers - Hướng Dẫn Sử Dụng

## 📦 GeneralObjectPoolManager
Pool manager generic để quản lý bất kỳ loại GameObject nào (projectiles, items, effects, debris, v.v.)

### Thiết Lập
1. Tạo một GameObject mới, đặt tên `GeneralObjectPoolManager`
2. Thêm script `GeneralObjectPoolManager.cs`
3. Trong Inspector, thêm các Pool Items:
   - **Pool Name**: Tên định danh (vd: "arrow", "explosion", "coin")
   - **Prefab**: Prefab để spawn
   - **Initial Pool Size**: Số lượng tạo ban đầu
   - **Expandable**: Cho phép tạo thêm nếu hết pool?

### Ví Dụ Cơ Bản
```csharp
// Spawn object
GameObject projectile = GeneralObjectPoolManager.Instance.SpawnObject("arrow", position);

// Trả về pool
GeneralObjectPoolManager.Instance.ReturnToPool(projectile, "arrow");

// Kiểm tra pool
int available = GeneralObjectPoolManager.Instance.GetPoolCount("arrow");
bool exists = GeneralObjectPoolManager.Instance.PoolExists("arrow");
```

---

## 🎯 ProjectilePoolManager (Wrapper)
Wrapper chuyên dụng cho projectiles, dễ sử dụng hơn.

### Cách Dùng
```csharp
// Spawn projectile với direction
GameObject arrow = ProjectilePoolManager.Instance.SpawnProjectile("arrow", position, direction);

// Trả về pool
ProjectilePoolManager.Instance.ReturnProjectile(arrow, "arrow");
```

### Inspector Setup
- Tạo GameObject `ProjectilePoolManager`
- Thêm script `ProjectilePoolManager.cs`
- Cấu hình GeneralObjectPoolManager trước (hoặc dùng chung)

---

## ✨ EffectPoolManager (VFX/Particles)
Quản lý hiệu ứng tự động return về pool sau duration.

### Cách Dùng
```csharp
// Spawn effect (tự động return sau duration)
EffectPoolManager.Instance.SpawnEffect("explosion", position, duration: 1f);

// Spawn và keep trong scene (không auto-return)
EffectPoolManager.Instance.SpawnOneTimeEffect("blood", position, duration: 0.5f);
```

### Inspector Setup
- Tạo GameObject `EffectPoolManager`
- Thêm script `EffectPoolManager.cs`

---

## 🔄 So Sánh: EnemyPoolManager vs GeneralObjectPoolManager

| Feature | EnemyPoolManager | GeneralObjectPoolManager |
|---------|-----------------|------------------------|
| Mục đích | Quản lý riêng Enemies | Bất kỳ GameObject nào |
| Enum/String | Enum (EnemyType) | String (poolName) |
| Mở rộng | Khó (phải sửa enum) | Dễ (thêm PoolItem) |
| Flexible | Ít | Cao |
| Singleton | ✅ | ✅ |

---

## 💡 Best Practices
1. **Một GeneralObjectPoolManager** cho toàn game (không cần nhiều)
2. **Wrappers riêng** (ProjectilePoolManager, EffectPoolManager) cho từng loại nếu có logic đặc biệt
3. **Đặt tên pool rõ ràng**: "arrow", "fireball", "blood_splat", "coin_drop"
4. **Set expandable = true** cho safety (nếu pool hết)
5. **Bật showDebugLogs** để debug, tắt trước ship

---

## 🎮 Ví Dụ Full: Shoot Arrow
```csharp
public class Archer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        Vector3 direction = transform.right;
        GameObject arrow = ProjectilePoolManager.Instance.SpawnProjectile(
            "arrow", 
            transform.position + direction, 
            direction
        );
        
        if (arrow != null)
        {
            // Setup arrow (damage, lifetime, etc.)
            arrow.GetComponent<Arrow>().SetLifetime(5f);
        }
    }
}
```

---

## 🐛 Debugging
```csharp
// Xem trạng thái pool
GeneralObjectPoolManager.Instance.PrintPoolStatus();

// Xóa một pool
GeneralObjectPoolManager.Instance.ClearPool("arrow");

// Xóa tất cả pools
GeneralObjectPoolManager.Instance.ClearAllPools();
```

---

## 📝 Notes
- **Thread-safe?** Không — chỉ dùng main thread
- **Scene persistence?** DontDestroyOnLoad nếu cần (thêm vào Awake)
- **Performance?** O(1) cho spawn/return (dequeue/enqueue là constant time)
