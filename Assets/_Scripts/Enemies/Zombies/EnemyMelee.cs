using UnityEngine;

public class EnemyMelee : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public float damage = 10f;

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