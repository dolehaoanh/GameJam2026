using UnityEngine;

// Định nghĩa 3 loại mặt nạ (Thứ tự: 0=Red, 1=White, 2=Black)
public enum MaskType { Red, White, Black }

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Prototype Colors")]
    public Color colorRed = Color.red;
    public Color colorWhite = Color.white;
    public Color colorBlack = Color.black;

    [Header("State")]
    public MaskType currentMask;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        // Mặc định là Đỏ
        ChangeMask(MaskType.Red);
    }

    void Update()
    {
        // 1. Di chuyển
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // 2. Đổi mặt nạ trực tiếp (Phím 1, 2, 3)
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeMask(MaskType.Red);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeMask(MaskType.White);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeMask(MaskType.Black);

        // 3. Đổi mặt nạ xoay vòng (Phím SPACE) <<< TÍNH NĂNG MỚI
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleNextMask();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // Hàm chuyển sang mặt nạ tiếp theo trong danh sách
    void CycleNextMask()
    {
        // Logic toán học: (Hiện tại + 1) chia lấy dư cho Tổng số mặt nạ (3)
        // Ví dụ: 
        // Đang 0 (Red) -> (0+1)%3 = 1 (White)
        // Đang 1 (White)-> (1+1)%3 = 2 (Black)
        // Đang 2 (Black)-> (2+1)%3 = 0 (Red) -> Quay vòng lại
        
        int nextIndex = ((int)currentMask + 1) % 3;
        ChangeMask((MaskType)nextIndex);
    }

    public void ChangeMask(MaskType type)
    {
        currentMask = type;

        switch (type)
        {
            case MaskType.Red:
                sr.color = colorRed;
                break;
            case MaskType.White:
                sr.color = colorWhite;
                break;
            case MaskType.Black:
                sr.color = colorBlack;
                break;
        }

        // Debug.Log($"Đã đổi sang: {type}");
    }
}