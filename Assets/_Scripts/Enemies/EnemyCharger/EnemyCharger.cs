using UnityEngine;
using System.Collections;

public class EnemyCharger : EnemyBaseFSM
{
    [Header("Specific Settings")]
    public float chargeSpeed = 20f;
    public float prepareTime = 1f; // Thời gian cảnh báo
    public float chargeDuration = 0.5f; // Thời gian lao đi

    private bool isCharging = false;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        if (sr != null) originalColor = sr.color;
    }

    protected override void LogicAttack()
    {
        if (isCharging) return; // Đang húc thì kệ

        if (Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(ChargeRoutine());
            lastAttackTime = Time.time;
        }
        else
        {
            // Trong lúc chờ hồi chiêu thì vẫn bám theo
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist > attackRange) ChangeState(EnemyState.Chase);
        }
    }

    IEnumerator ChargeRoutine()
    {
        isCharging = true;
        agent.isStopped = true; // Tắt NavMesh để tự lao

        // 1. GỒNG (Cảnh báo)
        if (sr != null) sr.color = Color.yellow; // Nháy màu
        Debug.Log("Charger: Gồng...");
        yield return new WaitForSeconds(prepareTime);

        // 2. CHỐT HƯỚNG
        Vector3 dir = (target.position - transform.position).normalized;
        if (sr != null) sr.color = originalColor; // Trả màu

        // 3. HÚC
        float timer = 0;
        while (timer < chargeDuration)
        {
            // Dùng NavMeshAgent.velocity hoặc Transform để lao
            agent.velocity = dir * chargeSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        // 4. NGHỈ
        agent.velocity = Vector3.zero;
        agent.isStopped = false;
        isCharging = false;

        // Xong việc thì quay lại đuổi tiếp
        ChangeState(EnemyState.Chase);
    }
}