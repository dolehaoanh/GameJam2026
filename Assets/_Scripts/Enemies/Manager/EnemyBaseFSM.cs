using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))] // <--- THAY BẰNG DÒNG NÀY
public class EnemyBaseFSM : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Die }

    [Header("--- DEBUG MODE (BẬT LÊN ĐỂ SOI) ---")]
    public bool showDebugLogs = true; // Tích vào cái này để xem log
    public bool showGizmos = true;    // Tích vào để xem dây nối

    [Header("--- POOLING & TYPE ---")]
    public EnemyType myType;

    [Header("--- COLOR STEALTH LOGIC ---")]
    public MaskType enemyMask;
    protected PlayerColorManager playerColorMgr;

    [Header("--- BASE STATS ---")]
    public float hp = 100f;
    public float chaseRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float moveSpeed = 3.5f; // Đảm bảo số này > 0
    protected float lastAttackTime;

    [Header("--- REFERENCES ---")]
    protected Transform target;
    protected NavMeshAgent agent;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    public EnemyState currentState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        // 1. SETUP NAVMESH 2D
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed; // Đồng bộ tốc độ ngay
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        FindPlayer();
        TintEnemyColor();

        // --- KIỂM TRA BỆNH LÝ NGAY KHI START ---
        if (showDebugLogs)
        {
            if (target == null) Debug.LogError($"❌ {name}: KHÔNG TÌM THẤY PLAYER! (Kiểm tra Tag 'Player' chưa?)");
            if (!agent.isOnNavMesh) Debug.LogError($"❌ {name}: KHÔNG NẰM TRÊN NAVMESH! (Đã Bake chưa? Z có bằng 0 không?)");
            else Debug.Log($"✅ {name}: Đã đặt chân lên NavMesh an toàn.");
        }

        ChangeState(EnemyState.Idle);
    }

    protected virtual void OnEnable()
    {
        hp = 100f;
        ChangeState(EnemyState.Idle); // Reset về Idle khi spawn
        if (agent != null)
        {
            agent.isStopped = false;
            agent.velocity = Vector3.zero;
        }
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = true;
        TintEnemyColor();
        if (target == null) FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;

            // --- SỬA DÒNG NÀY ---
            // Thay vì GetComponent (chỉ tìm trên cha), hãy dùng GetComponentInChildren (tìm cả con)
            playerColorMgr = playerObj.GetComponentInChildren<PlayerColorManager>();

            // --- THÊM DÒNG DEBUG ĐỂ KIỂM TRA ---
            if (playerColorMgr == null)
                Debug.LogError("❌ Enemy: Tìm thấy Player nhưng KHÔNG thấy script 'PlayerColorManager'! Kiểm tra lại xem gắn script chưa?");
            else
                Debug.Log("✅ Enemy: Đã kết nối với hệ thống màu sắc của Player.");
        }
    }
    protected virtual void Update()
    {
        if (target == null || currentState == EnemyState.Die) return;

        // [ĐÃ BỎ] Logic Flip theo yêu cầu

        // --- [LOGIC QUAN TRỌNG] NGỤY TRANG & PHANH GẤP ---
        if (IsPlayerDisguised())
        {
            // Nếu đang Húc hoặc đang Chạy mà thấy cùng màu -> DỪNG NGAY
            if (currentState != EnemyState.Idle)
            {
                // 1. HỦY MỌI CHIÊU THỨC (Quan trọng cho Charger đang gồng húc)
                StopAllCoroutines();

                // 2. PHANH NAVMESH (AI Dẫn đường)
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.ResetPath();             // Xóa đường đi
                    agent.velocity = Vector3.zero; // Xóa vận tốc AI
                    agent.isStopped = true;        // Bắt đứng lại
                }

                // 3. PHANH VẬT LÝ (Rigidbody) - Để chống trôi theo quán tính
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;    // Dừng trượt
                    rb.angularVelocity = 0f;       // Dừng xoay
                }

                if (showDebugLogs) Debug.Log($"🛑 {name}: Phát hiện cùng màu -> PHANH GẤP!");
                ChangeState(EnemyState.Idle);
            }
            return; // Ngắt luôn, không làm gì nữa
        }
        // -------------------------------------------------------

        // FSM SWITCH
        switch (currentState)
        {
            case EnemyState.Idle: LogicIdle(); break;
            case EnemyState.Chase: LogicChase(); break;
            case EnemyState.Attack: LogicAttack(); break;
        }
    }

    // --- LOGIC CHI TIẾT KÈM DEBUG ---

    protected virtual void LogicIdle()
    {
        if (target == null) return;
        float dist = Vector2.Distance(transform.position, target.position);

        // Debug nhẹ (chỉ log khi đến gần để đỡ spam)
        // if (showDebugLogs) Debug.Log($"{name} Idling... Dist: {dist}/{chaseRange}");

        if (dist < chaseRange)
        {
            if (showDebugLogs) Debug.Log($"👀 {name}: Phát hiện Player ({dist}m) -> CHASE!");
            ChangeState(EnemyState.Chase);
        }
    }
    // Thêm dòng này vào EnemyBaseFSM.cs để các con có chỗ mà override
    public virtual void ResetSpecialAbility()
    {
        // Để trống ở đây, con nào cần thì tự viết đè lên
    }

    protected virtual void LogicChase()
    {
        if (target == null) return;

        // --- 1. NHẢ PHANH (QUAN TRỌNG) ---
        // Nếu trước đó bị phanh gấp do đổi màu, giờ phải mở lại
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        // --- 2. TÍNH KHOẢNG CÁCH (KHAI BÁO 1 LẦN DUY NHẤT) ---
        float dist = Vector2.Distance(transform.position, target.position);

        // --- 3. DEBUG SOI KẸT ---
        if (showDebugLogs && agent.hasPath && agent.velocity.magnitude < 0.1f && !agent.isStopped)
        {
            Debug.LogWarning($"⚠️ {name}: Đang Chase nhưng kẹt chân! (HasPath: {agent.hasPath}, Status: {agent.pathStatus})");
        }

        // --- 4. CHUYỂN TRẠNG THÁI ---
        if (dist <= attackRange)
        {
            if (showDebugLogs) Debug.Log($"⚔️ {name}: Đủ tầm đánh ({dist}m) -> ATTACK!");

            agent.ResetPath(); // Dừng lại để đánh
            ChangeState(EnemyState.Attack);
        }
        else
        {
            // Chưa tới tầm thì chạy tiếp
            agent.SetDestination(target.position);
        }
    }

    protected virtual void LogicAttack()
    {
        if (target == null) return;
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist > attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            if (showDebugLogs) Debug.Log($"💨 {name}: Player chạy mất ({dist}m) -> Quay lại CHASE");
            ChangeState(EnemyState.Chase);
        }
    }

    // --- CORE ---

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return; // Tránh spam log nếu trạng thái không đổi

        if (showDebugLogs) Debug.Log($"🔄 {name} State Change: [{currentState}] ---> [{newState}]");
        currentState = newState;
    }

    protected bool IsPlayerDisguised()
    {
        if (playerColorMgr == null) return false;
        return playerColorMgr.GetCurrentMask() == enemyMask;
    }

    void TintEnemyColor()
    {
        if (sr == null) return;
        switch (enemyMask)
        {
            case MaskType.Red: sr.color = Color.red; break;
            case MaskType.White: sr.color = Color.white; break;
            case MaskType.Black: sr.color = new Color(0.3f, 0.3f, 0.3f, 1f); break;
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (showDebugLogs) Debug.Log($"🩸 {name} bị đánh! HP còn: {hp}");

        if (hp <= 0) Die();
    }

    void Die()
    {
        if (currentState == EnemyState.Die) return;
        ChangeState(EnemyState.Die);
        if (agent != null) agent.ResetPath();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.SetTrigger("Die");
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        StartCoroutine(ReturnToPoolRoutine());
    }

    IEnumerator ReturnToPoolRoutine()
    {
        yield return new WaitForSeconds(1f);
        if (EnemyPoolManager.Instance != null)
            EnemyPoolManager.Instance.ReturnToPool(this.gameObject, myType);
        else
            Destroy(gameObject);
    }

    // --- VISUAL DEBUGGING (VẼ HÌNH) ---
    protected virtual void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Vẽ vòng tròn tầm nhìn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Vẽ vòng tròn tầm đánh
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ dây nối tới Player (Nếu có Target)
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist < chaseRange)
            {
                Gizmos.color = Color.green; // Đang nhìn thấy
                Gizmos.DrawLine(transform.position, target.position);
            }
            else
            {
                Gizmos.color = Color.gray; // Player ở quá xa
                Gizmos.DrawLine(transform.position, target.position);
            }
        }

        // Vẽ Path của NavMesh (Đường đi dự kiến)
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            var path = agent.path;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
    }
}