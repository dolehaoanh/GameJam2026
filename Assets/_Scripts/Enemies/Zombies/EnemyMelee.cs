using UnityEngine;

public class EnemyMelee : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public float damage = 10f;

    // Trong file EnemyRanged.cs

    protected override void Update()
    {
        // --- 1. CHỐT CHẶN TUYỆT ĐỐI ---
        if (EnemyBaseFSM.IsGlobalFrozen)
        {
            // A. Dừng di chuyển
            if (agent != null && agent.enabled) agent.isStopped = true;

            // B. Cấm tấn công (Reset bộ đếm giờ tấn công)
            //attackTimer = 0f;

            // C. Ngắt Animation tấn công (nếu có) - Chuyển về Idle
            // animator.Play("Idle"); // Bỏ comment dòng này nếu muốn nó đứng im phăng phắc

            return; // ⛔ DỪNG NGAY! Cấm chạy bất kỳ dòng code nào bên dưới
        }
        else
        {
            // Xả băng thì mới cho đi lại
            if (agent != null && agent.enabled) agent.isStopped = false;
        }
        // ---------------------------------

        base.Update(); // Logic cũ chạy bình thường khi không đóng băng
    }
    protected override void LogicAttack()
    {
        // Kiểm tra Cooldown
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // TẤN CÔNG
            //if (anim != null) anim.SetTrigger("Attack");

            // TODO: Trừ máu Player thật ở đây
            // target.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            lastAttackTime = Time.time;
        }

        // Nếu Player chạy khỏi tầm đánh -> Đuổi tiếp
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }
}