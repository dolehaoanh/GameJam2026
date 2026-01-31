// using UnityEngine;

// public class CustomSpriteAnimator : MonoBehaviour
// {
//     [Header("Settings")]
//     public float frameRate = 12f; // Tốc độ khung hình (FPS)
//     private SpriteRenderer sr;
//     private float timer;
//     private int currentFrame;
//     private bool isLooping = true;
//     private Sprite[] currentAnimation; // Animation đang chạy hiện tại

//     // --- KHO CHỨA ẢNH (Kéo Sprite vào đây) ---
//     [Header("--- IDLE (Đứng Yên) ---")]
//     public Sprite[] idleDown;
//     public Sprite[] idleUp;
//     public Sprite[] idleSide; // Dùng chung cho Trái/Phải

//     [Header("--- RUN (Chạy) ---")]
//     public Sprite[] runDown;
//     public Sprite[] runUp;
//     public Sprite[] runSide;

//     [Header("--- ATTACK (Tấn Công) ---")]
//     public Sprite[] attackDown;
//     public Sprite[] attackUp;
//     public Sprite[] attackSide;

//     [Header("--- DIE (Chết) ---")]
//     public Sprite[] dieDown;
//     public Sprite[] dieUp;
//     public Sprite[] dieSide;

//     // Các trạng thái animation để tránh set lại liên tục
//     private string currentAnimName; 

//     void Awake()
//     {
//         sr = GetComponent<SpriteRenderer>();
//     }

//     void Update()
//     {
//         if (currentAnimation == null || currentAnimation.Length == 0) return;

//         timer += Time.deltaTime;

//         // Tính toán chuyển frame
//         if (timer >= 1f / frameRate)
//         {
//             timer = 0;
//             currentFrame++;

//             // Xử lý lặp lại hay dừng lại
//             if (currentFrame >= currentAnimation.Length)
//             {
//                 if (isLooping)
//                 {
//                     currentFrame = 0; // Quay lại đầu
//                 }
//                 else
//                 {
//                     currentFrame = currentAnimation.Length - 1; // Giữ nguyên frame cuối (cho Die)
//                 }
//             }

//             // Gán ảnh vào SpriteRenderer
//             sr.sprite = currentAnimation[currentFrame];
//         }
//     }

//     // HÀM CHÍNH ĐỂ CONTROLLER GỌI
//     public void PlayAnimation(PlayerController.State state, Vector2 direction)
//     {
//         Sprite[] targetClip = null;
//         bool shouldLoop = true;
//         string newAnimName = state.ToString(); // Tạo key để check trùng

//         // 1. Xử lý lật ảnh (FlipX)
//         if (direction.x != 0)
//         {
//             sr.flipX = (direction.x < 0); // Nếu đi sang trái (x < 0) thì lật ảnh
//         }

//         // 2. Chọn bộ ảnh dựa trên State và Hướng
//         switch (state)
//         {
//             case PlayerController.State.Idle:
//                 if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) targetClip = idleSide;
//                 else if (direction.y > 0) targetClip = idleUp;
//                 else targetClip = idleDown;
//                 break;

//             case PlayerController.State.Move:
//                 if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) targetClip = runSide;
//                 else if (direction.y > 0) targetClip = runUp;
//                 else targetClip = runDown;
//                 break;

//             case PlayerController.State.Attack:
//                 shouldLoop = true; // Attack thường loop theo thời gian của Controller
//                 if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) targetClip = attackSide;
//                 else if (direction.y > 0) targetClip = attackUp;
//                 else targetClip = attackDown;
//                 break;

//             case PlayerController.State.Die:
//                 shouldLoop = false; // Chết là nằm im, không lặp lại
//                 if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) targetClip = dieSide;
//                 else if (direction.y > 0) targetClip = dieUp;
//                 else targetClip = dieDown;
//                 break;
//         }

//         // 3. Nếu animation thay đổi thì reset frame về 0
//         // Kết hợp tên state và hướng để biết có thay đổi không
//         string animKey = state.ToString() + (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? "Side" : (direction.y > 0 ? "Up" : "Down"));

//         if (currentAnimName != animKey)
//         {
//             currentAnimName = animKey;
//             currentAnimation = targetClip;
//             currentFrame = 0;
//             timer = 0;
//             isLooping = shouldLoop;
            
//             // Render ngay frame đầu tiên để không bị delay 1 nhịp
//             if (currentAnimation != null && currentAnimation.Length > 0)
//                 sr.sprite = currentAnimation[0];
//         }
//     }
// }