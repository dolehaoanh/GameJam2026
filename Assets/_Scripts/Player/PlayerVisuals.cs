using UnityEngine;
using System.Collections;

public class PlayerVisuals : MonoBehaviour
{
    [Header("--- KẾT NỐI MODULE ---")]
    [SerializeField] private PlayerControllerFSM controller;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerColorManager colorManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("--- CẤU HÌNH HITBOX ---")]
    public GameObject hitboxObj;
    public float hitboxDuration = 0.3f;

    [Header("--- CẤU HÌNH TÊN GỌI ---")]
    public string weaponPrefix = "Sword";
    public int comboStep = 1;

    private string currentAnimName;
    private Vector2 lastVisualDirection = Vector2.down;
    private PlayerControllerFSM.State lastState;

    void Awake()
    {
        if (controller == null) controller = GetComponentInParent<PlayerControllerFSM>();
        if (animator == null) animator = GetComponent<Animator>();
        if (colorManager == null) colorManager = GetComponent<PlayerColorManager>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (controller == null || animator == null) return;

        UpdateAnimationState();
        HandleColorInput(); // <--- Xử lý phím Cách ở đây
    }

    void UpdateAnimationState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Nếu đang chém dở thì giữ nguyên
        if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime < 1.0f) return;

        // 1. CẬP NHẬT HƯỚNG
        if (controller.currentState != PlayerControllerFSM.State.Attack &&
            controller.currentState != PlayerControllerFSM.State.Die)
        {
            Vector2 dir = controller.GetLastDirection();
            if (dir.magnitude > 0.1f) lastVisualDirection = dir;
        }

        // 2. XỬ LÝ FLIP X (Logic: Anim Right bị ngược -> Cần Flip khi đánh Phải)
        if (controller.currentState == PlayerControllerFSM.State.Attack)
        {
            if (lastVisualDirection.x > 0.1f) spriteRenderer.flipX = true; // Chém Phải -> LẬT
            else spriteRenderer.flipX = false; // Các hướng khác -> KHÔNG LẬT
        }
        else
        {
            spriteRenderer.flipX = false; // Đi đứng bình thường -> KHÔNG LẬT
        }

        // 3. TẠO TÊN ANIMATION
        string finalStateName = "";
        string directionSuffix = GetDirectionSuffix(lastVisualDirection);

        switch (controller.currentState)
        {
            case PlayerControllerFSM.State.Idle:
                finalStateName = $"Idle_{directionSuffix}";
                break;
            case PlayerControllerFSM.State.Move:
                finalStateName = $"Run_{directionSuffix}";
                break;
            case PlayerControllerFSM.State.Attack:
                finalStateName = $"{weaponPrefix}_Attack_{comboStep}_{directionSuffix}";
                break;
            case PlayerControllerFSM.State.Die:
                finalStateName = $"Death_{directionSuffix}";
                break;
        }

        // 4. CHẠY ANIMATION
        if (currentAnimName != finalStateName)
        {
            PlayAnimationDirectly(finalStateName);
            if (controller.currentState == PlayerControllerFSM.State.Attack)
            {
                StartCoroutine(ActivateHitboxRoutine(lastVisualDirection));
            }
        }
        else if (controller.currentState == PlayerControllerFSM.State.Attack && lastState != PlayerControllerFSM.State.Attack)
        {
            PlayAnimationDirectly(finalStateName);
            StartCoroutine(ActivateHitboxRoutine(lastVisualDirection));
        }

        lastState = controller.currentState;
    }

    IEnumerator ActivateHitboxRoutine(Vector2 dir)
    {
        if (hitboxObj == null) yield break;

        float rotationZ = 0f;
        if (dir.y < -0.1f) rotationZ = 0f;       // Down
        else if (dir.y > 0.1f) rotationZ = 180f; // Up
        else if (dir.x < -0.1f) rotationZ = -90f; // Left
        else if (dir.x > 0.1f) rotationZ = 90f;   // Right

        hitboxObj.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        hitboxObj.SetActive(true);
        yield return new WaitForSeconds(hitboxDuration);
        hitboxObj.SetActive(false);
    }

    void PlayAnimationDirectly(string animName)
    {
        if (animator.HasState(0, Animator.StringToHash(animName)))
        {
            animator.Play(animName, 0, 0f);
            currentAnimName = animName;
        }
    }

    string GetDirectionSuffix(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) return dir.x > 0 ? "Right" : "Left";
        else return dir.y > 0 ? "Up" : "Down";
    }

    public void SetComboStep(int step) { comboStep = step; }

    // --- HÀM XỬ LÝ ĐỔI MÀU (SPACE) ---
    void HandleColorInput()
    {
        // Phím CÁCH (Space) -> Gọi hàm đổi màu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (colorManager != null)
            {
                // Gọi hàm đổi màu vòng tròn (Giả định đồng chí đã viết hàm này trong ColorManager)
                colorManager.CycleNextMask();
            }
        }

        // Các phím debug cũ (giữ lại phòng hờ)
        if (Input.GetKeyDown(KeyCode.Alpha4)) colorManager?.SetMask(MaskType.Red);
        if (Input.GetKeyDown(KeyCode.Alpha5)) colorManager?.SetMask(MaskType.White);
        if (Input.GetKeyDown(KeyCode.Alpha6)) colorManager?.SetMask(MaskType.Black);
    }
}