using UnityEngine;

public class PlayerControllerFSM : MonoBehaviour
{
    // Định nghĩa các trạng thái
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = State.Idle;
    }

    void Update()
    {
        if (currentState == State.Die) return;

        // 1. Input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        // Cập nhật hướng nhìn (quan trọng cho hàm GetLastDirection)
        if (movementInput.magnitude > 0.1f)
        {
            lastDirection = movementInput;
        }

        // 2. Logic FSM
        HandleStateSwitch();

        // Test Chết
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

    void HandleStateSwitch()
    {
        switch (currentState)
        {
            case State.Idle:
                if (movementInput.magnitude > 0.1f) ChangeState(State.Move);
                else if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) ChangeState(State.Attack);
                break;

            case State.Move:
                if (movementInput.magnitude < 0.1f) ChangeState(State.Idle);
                else if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) ChangeState(State.Attack);
                break;

            case State.Attack:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0) ChangeState(State.Idle);
                break;
        }
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

    // ---------------------------------------------------------
    // ĐÂY LÀ HÀM QUAN TRỌNG ĐANG BỊ THIẾU, HÃY ĐẢM BẢO NÓ Ở ĐÂY
    // ---------------------------------------------------------
    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }
}