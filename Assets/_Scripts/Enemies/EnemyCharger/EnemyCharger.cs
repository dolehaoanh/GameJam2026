using UnityEngine;
using System.Collections;

public class EnemyCharger : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public float chargeSpeed = 30f; // Đã nâng cấp theo yêu cầu của đồng chí
    public float prepareTime = 1f;
    public float chargeDuration = 0.5f;

    private bool isCharging = false;
    private Color originalColor; 

    protected override void Start()
    {
        base.Start();
        if (sr != null) originalColor = sr.color;
    }

    // --- HÀM QUAN TRỌNG NHẤT ĐỂ TRỊ BỆNH LIỆT ---
    public override void ResetSpecialAbility()
    {
        isCharging = false; // Mở khóa trạng thái
        if (sr != null) sr.color = originalColor; // Trả lại màu đen
        
        // Nhả phanh vật lý để không bị trôi
        if (agent != null && agent.isOnNavMesh)
        {
            agent.velocity = Vector3.zero;
        }
        
        if(showDebugLogs) Debug.Log($"<color=cyan>⚡ {name}: Đã Reset biến isCharging! Sẵn sàng đuổi tiếp.</color>");
    }

    protected override void LogicAttack()
    {
        if (isCharging) return;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(ChargeRoutine());
            lastAttackTime = Time.time;
        }
        else
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist > attackRange) ChangeState(EnemyState.Chase);
        }
    }

    IEnumerator ChargeRoutine()
    {
        isCharging = true;
        
        if (agent.isOnNavMesh) 
        {
            agent.isStopped = true; 
            agent.ResetPath();
        }

        if (sr != null) sr.color = Color.yellow; // Gồng
        yield return new WaitForSeconds(prepareTime);

        if (sr != null) sr.color = originalColor; // Húc
        
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            float timer = 0;
            while (timer < chargeDuration)
            {
                agent.velocity = dir * chargeSpeed; // Lao đi
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // Kết thúc bình thường
        ResetSpecialAbility();
        ChangeState(EnemyState.Chase);
    }
}