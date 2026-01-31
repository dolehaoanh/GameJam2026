using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private PlayerControllerFSM controller;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerColorManager colorManager;

    private string currentAnimName;
    private Vector2 lastVisualDirection = Vector2.down;
    
    // Đã xóa biến SpriteRenderer vì không cần dùng để Flip nữa

    void Awake()
    {
        // 1. TÌM CHA (Logic nằm ở cha)
        if (controller == null) controller = GetComponentInParent<PlayerControllerFSM>();

        // 2. TÌM CHÍNH MÌNH (Visual nằm ở con)
        if (animator == null) animator = GetComponent<Animator>();
        if (colorManager == null) colorManager = GetComponent<PlayerColorManager>();
    }

    void Update()
    {
        if (controller == null || animator == null) return;

        UpdateAnimationState();
        HandleColorInput();
    }

    void UpdateAnimationState()
    {
        // Lấy hướng từ Controller cha
        Vector2 dir = controller.GetLastDirection();
        
        // Cập nhật hướng nhìn cuối cùng (để khi đứng yên không bị reset về default)
        if (dir.magnitude > 0.1f) lastVisualDirection = dir;

        // --- BỎ PHẦN XỬ LÝ FLIP TẠI ĐÂY ---

        // --- XỬ LÝ CHỌN ANIMATION ---
        string directionSuffix = GetDirectionSuffix(lastVisualDirection);
        string statePrefix = "Idle";

        switch (controller.currentState)
        {
            case PlayerControllerFSM.State.Idle:   statePrefix = "Idle"; break;
            case PlayerControllerFSM.State.Move:   statePrefix = "Run"; break;
            case PlayerControllerFSM.State.Attack: statePrefix = "Attack"; break;
            case PlayerControllerFSM.State.Die:    statePrefix = "Death"; break;
        }

        // Ghép tên: Ví dụ "Run_Left" (Animator sẽ tự chạy clip Run_Left)
        string newAnimName = $"{statePrefix}_{directionSuffix}";

        if (currentAnimName != newAnimName)
        {
            animator.Play(newAnimName);
            currentAnimName = newAnimName;
        }
    }

    string GetDirectionSuffix(Vector2 dir)
    {
        // Trả về hậu tố để ghép tên Animation
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // Trả về Left hoặc Right rõ ràng
            return dir.x > 0 ? "Right" : "Left"; 
        }
        else
        {
            return dir.y > 0 ? "Up" : "Down";
        }
    }

    void HandleColorInput()
    {
        if (controller.currentState == PlayerControllerFSM.State.Die) return;

        if (colorManager != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) colorManager.SetMask(MaskType.Red);
            if (Input.GetKeyDown(KeyCode.Alpha2)) colorManager.SetMask(MaskType.White);
            if (Input.GetKeyDown(KeyCode.Alpha3)) colorManager.SetMask(MaskType.Black);
            if (Input.GetKeyDown(KeyCode.Space))  colorManager.CycleNextMask();
        }
    }
}