using UnityEngine;

public class EnemyRanged : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public GameObject bulletPrefab; // (Lưu ý: Nếu dùng Pool thì biến này có thể không cần, nhưng cứ để đó)
    public Transform firePoint; 
    public float stopDistance = 6f; 
    
    [Header("Pool Settings")]
    public string bulletPoolName = "skeleton_bullet"; 
    public float bulletDamage = 15f;
    public float bulletSpeed = 10f;

    protected override void Start()
    {
        base.Start();
        attackRange = stopDistance;
        
        // Debug check ngay từ đầu
        if (GeneralObjectPoolManager.Instance == null)
            Debug.LogError($"❌ {name}: KHÔNG TÌM THẤY PoolManager! Đã kéo PoolManager vào Scene chưa?");
    }

    protected override void LogicChase()
    {
        // --- ĐOẠN CODE "MÁY DÒ LỖI" ---
        if (agent == null)
        {
            Debug.LogError($"❌ LỖI: Thằng '{gameObject.name}' bị mất NavMeshAgent!", gameObject);
            return;
        }

        if (!agent.isOnNavMesh)
        {
            // gameObject ở tham số thứ 2 giúp đồng chí bấm vào log là nó trỏ ngay đến vật thể đó
            Debug.LogError($"🚨 BẮT ĐƯỢC RỒI: Thằng '{gameObject.name}' đang đứng ở tọa độ {transform.position} nhưng KHÔNG chạn vào NavMesh!", gameObject);
            return;
        }

        if (!agent.isActiveAndEnabled)
        {
            Debug.LogError($"💤 LỖI: Thằng '{gameObject.name}' có Agent nhưng đang bị Disable!", gameObject);
            return;
        }
        // -----------------------------

        // Code cũ
       // agent.SetDestination(target.position);

        if (target == null) return; // Fix null reference

        // Check ngụy trang (giữ nguyên logic cũ của đồng chí)
        if (IsPlayerDisguised()) return; 

        // 1. NHẢ PHANH (QUAN TRỌNG)
        if (agent.isOnNavMesh && agent.isStopped) agent.isStopped = false;

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= stopDistance)
        {
            //if(showDebugLogs) Debug.Log($"🛑 {name}: Đủ tầm bắn ({dist}m) -> Dừng lại & Bắn!");
            agent.ResetPath();
            ChangeState(EnemyState.Attack);
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    protected override void LogicAttack()
    {
        if (target == null) return;

        // Quay mặt
        if (sr != null)
        {
            Vector2 dir = target.position - transform.position;
            sr.flipX = dir.x < 0; // Skeleton thường mặc định mặt quay phải
        }

        if (Time.time > lastAttackTime + attackCooldown)
        {
            //if(showDebugLogs) Debug.Log($"🔫 {name}: Hết Cooldown -> BẮN!");

            //if (anim != null) anim.SetTrigger("Attack");

            SpawnBulletFromPool();

            lastAttackTime = Time.time;
        }

        // Logic quay lại Chase
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > stopDistance * 1.2f)
        {
            if(showDebugLogs) Debug.Log($"🏃 {name}: Player chạy xa ({dist}m) -> Đuổi theo!");
            ChangeState(EnemyState.Chase);
        }
    }

    void SpawnBulletFromPool()
    {
        // 1. Check FirePoint
        if (firePoint == null)
        {
            Debug.LogError($"❌ {name}: LỖI NẶNG! Chưa gán FirePoint (Transform nòng súng) trong Inspector!");
            return;
        }

        // 2. Check Pool Manager
        if (GeneralObjectPoolManager.Instance == null) return; // Đã báo lỗi ở Start rồi

        // 3. Thử lấy đạn
        GameObject bullet = GeneralObjectPoolManager.Instance.SpawnObject(
            bulletPoolName,
            firePoint.position,
            Quaternion.identity
        );

        if (bullet != null)
        {
            //if(showDebugLogs) Debug.Log($"✅ {name}: Đã lấy được đạn '{bullet.name}' từ Pool.");

            // Tính hướng bắn
            // (Lấy target.position + offset nhẹ để bắn vào thân thay vì chân)
            Vector3 targetPos = target.position + Vector3.up * 0.5f; 
            Vector2 shootDir = (targetPos - firePoint.position).normalized;

            // Xử lý vật lý
            Rigidbody2D rbBul = bullet.GetComponent<Rigidbody2D>();
            if (rbBul != null)
            {
                // LƯU Ý: Unity cũ dùng .velocity, Unity 6 mới dùng .linearVelocity
                // Tôi dùng .velocity cho an toàn, nếu đồng chí dùng Unity 6 thì đổi lại nhé
                rbBul.linearVelocity = shootDir * bulletSpeed; 
            }
            else
            {
                Debug.LogError($"❌ Prefab đạn trong Pool thiếu Rigidbody2D!");
            }

            // Xử lý Script Bullet
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
                bulletScript.SetDirection(shootDir); // Nếu script Bullet tự xử lý di chuyển
            }
            else
            {
                Debug.LogWarning($"⚠️ Prefab đạn thiếu script 'Bullet'!");
            }
        }
        else
        {
            Debug.LogError($"❌ {name}: KHÔNG LẤY ĐƯỢC ĐẠN! Kiểm tra tên Pool '{bulletPoolName}' có đúng với trong Manager không?");
        }
    }
}