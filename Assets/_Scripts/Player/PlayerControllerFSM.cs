using UnityEngine;

public class PlayerControllerFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Attack,
        Die
    }

    [Header("Stats")]
    public float moveSpeed = 5f;
    public float attackDuration = 0.4f;

    [Header("Debug Info")]
    public State currentState;
    private Vector2 movementInput;
    private Vector2 lastDirection = Vector2.down;
    private float stateTimer;
    private Rigidbody2D rb;

    // --- BIẾN ĐỂ XỬ LÝ "LAST INPUT PRIORITY" ---
    private float lastTimeLeft;
    private float lastTimeRight;
    private float lastTimeUp;
    private float lastTimeDown;

    private PlayerWeaponManager weaponManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponManager = GetComponent<PlayerWeaponManager>();
        currentState = State.Idle;
    }

    void Update()
    {
        if (currentState == State.Die) return;

        // 1. TÍNH TOÁN INPUT THEO KIỂU "ƯU TIÊN BẤM SAU"
        movementInput = GetPriorityInput();

        // Cập nhật hướng nhìn
        if (movementInput.magnitude > 0.1f)
        {
            lastDirection = movementInput;
        }

        // 2. Logic FSM
        HandleStateSwitch();

        // Test Chết (K)
        if (Input.GetKeyDown(KeyCode.K)) TriggerDeath();
    }

    void FixedUpdate()
    {
        if (currentState == State.Die || currentState == State.Attack)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == State.Move)
            rb.linearVelocity = movementInput * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    // --- HÀM XỬ LÝ INPUT THÔNG MINH (THAY THẾ GETAXISRAW) ---
    Vector2 GetPriorityInput()
    {
        // 1. Ghi lại thời điểm bấm phím
        if (Input.GetKeyDown(KeyCode.A)) lastTimeLeft = Time.time;
        if (Input.GetKeyDown(KeyCode.D)) lastTimeRight = Time.time;
        if (Input.GetKeyDown(KeyCode.W)) lastTimeUp = Time.time;
        if (Input.GetKeyDown(KeyCode.S)) lastTimeDown = Time.time;

        // 2. Tính trục X (Trái/Phải)
        float x = 0f;
        bool holdLeft = Input.GetKey(KeyCode.A);
        bool holdRight = Input.GetKey(KeyCode.D);

        if (holdLeft && !holdRight) x = -1f;      // Chỉ giữ Trái
        else if (!holdLeft && holdRight) x = 1f;  // Chỉ giữ Phải
        else if (holdLeft && holdRight)           // Giữ cả hai -> So sánh thời gian
        {
            x = (lastTimeLeft > lastTimeRight) ? -1f : 1f; // Cái nào bấm sau thì thời gian lớn hơn -> Thắng
        }

        // 3. Tính trục Y (Lên/Xuống)
        float y = 0f;
        bool holdUp = Input.GetKey(KeyCode.W);
        bool holdDown = Input.GetKey(KeyCode.S);

        if (holdDown && !holdUp) y = -1f;
        else if (!holdDown && holdUp) y = 1f;
        else if (holdDown && holdUp)
        {
            y = (lastTimeDown > lastTimeUp) ? -1f : 1f;
        }

        // Trả về vector đã chuẩn hóa
        return new Vector2(x, y).normalized;
    }

    void HandleStateSwitch()
    {
        switch (currentState)
        {
            case State.Idle:
                if (movementInput.magnitude > 0.1f)
                {
                    ChangeState(State.Move);
                }
                else if (CheckAttackInput())
                {
                }
                break;

            case State.Move:
                if (movementInput.magnitude < 0.1f)
                {
                    ChangeState(State.Idle);
                }
                else if (CheckAttackInput())
                {
                }
                break;

            case State.Attack:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0) ChangeState(State.Idle);
                break;
        }
    }

    bool CheckAttackInput()
    {
        // CHỈNH LẠI PHÍM J ĐỂ ĐÁNH (Space đã xóa)
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J))
        {
            if (weaponManager != null) weaponManager.Attack();
            ChangeState(State.Attack);
            return true;
        }
        return false;
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        if (currentState == State.Attack) stateTimer = attackDuration;
    }

    public void TriggerDeath()
    {
        if (currentState != State.Die) ChangeState(State.Die);
    }

    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }
}