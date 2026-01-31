using UnityEngine;

public class EnemyRanged : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Tạo 1 GamObject con ở nòng súng rồi kéo vào đây
    public float stopDistance = 6f; // Đứng cách 6m bắn

    protected override void Start()
    {
        base.Start();
        // Skeleton tấn công xa, nên AttackRange phải bằng StopDistance
        attackRange = stopDistance;
    }

    protected override void LogicChase()
    {
        // Check ngụy trang lần nữa cho chắc
        if (IsPlayerDisguised()) return;

        float dist = Vector2.Distance(transform.position, target.position);

        // Đến tầm bắn là dừng lại ngay, không lao sát vào
        if (dist <= stopDistance)
        {
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
        // Quay mặt về phía Player khi bắn
        if (sr != null)
        {
            Vector2 dir = target.position - transform.position;
            sr.flipX = dir.x < 0;
        }

        if (Time.time > lastAttackTime + attackCooldown)
        {
            // BẮN
            if (anim != null) anim.SetTrigger("Attack");

            if (bulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Vector2 shootDir = (target.position - firePoint.position).normalized;
                
                // Giả sử đạn có Rigidbody2D
                if(bullet.GetComponent<Rigidbody2D>())
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDir * 10f;
            }

            lastAttackTime = Time.time;
        }

        // Nếu Player chạy xa quá tầm thì mới đuổi
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > stopDistance * 1.2f)
        {
            ChangeState(EnemyState.Chase);
        }
    }
}