using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("--- KẾT NỐI ---")]
    public PlayerWeaponManager playerWeapon;
    private PlayerControllerFSM playerFSM; // Để soi trạng thái tấn công

    [Header("--- KỊCH BẢN (TỰ ĐỘNG ĐIỀN) ---")]
    public DialogueLine[] step1_IntroLines;
    public DialogueLine[] step2_EquipLines;
    public DialogueLine[] step3_AttackLines;
    public DialogueLine[] step4_MaskLines;
    public DialogueLine[] step5_FinishLines;

    private int currentStep = 0;
    private float moveTimer = 0f;

    void Awake()
    {
        // --- 1. TỰ ĐỘNG NẠP DỮ LIỆU CƠ BẢN ---

        if (IsArrayEmpty(step1_IntroLines))
            step1_IntroLines = CreateDialogue("HỆ THỐNG", "Di chuyển cơ thể bằng các phím [W, A, S, D].");

        if (IsArrayEmpty(step2_EquipLines))
            step2_EquipLines = CreateDialogue("CẢNH BÁO", "Kẻ địch tiếp cận! Bấm phím [1] để rút Kiếm ngay!");

        if (IsArrayEmpty(step3_AttackLines))
            step3_AttackLines = CreateDialogue("NHIỆM VỤ", "Tiêu diệt mục tiêu giả định bằng [CHUỘT TRÁI].");

        // --- 2. CẬP NHẬT: GIẢI THÍCH CƠ CHẾ MÀU SẮC (CAMOUFLAGE) ---
        if (IsArrayEmpty(step4_MaskLines))
        {
            step4_MaskLines = new DialogueLine[] {
                new DialogueLine {
                    characterName = "HỆ THỐNG",
                    sentence = "Phát hiện nhiều kẻ địch thuộc các Hệ Màu khác nhau!"
                },
                new DialogueLine {
                    characterName = "TIẾN SĨ",
                    sentence = "Nghe cho kỹ đây 626! Đây là Nguyên Tắc Tắc Kè Hoa."
                },
                new DialogueLine {
                    characterName = "QUY TẮC",
                    sentence = "Nếu ngươi đeo Mặt Nạ CÙNG MÀU với kẻ địch (Ví dụ: Đỏ gặp Đỏ), chúng sẽ coi ngươi là đồng loại và KHÔNG TẤN CÔNG."
                },
                new DialogueLine {
                    characterName = "CẢNH BÁO",
                    sentence = "Nhưng nếu KHÁC MÀU, lớp ngụy trang sẽ vô dụng và chúng sẽ tiêu diệt ngươi ngay lập tức!"
                },
                new DialogueLine {
                    characterName = "NHIỆM VỤ",
                    sentence = "Nhấn [SPACE] (CÁCH) để đổi màu Mặt Nạ liên tục và luồn lách qua chúng!"
                }
            };
        }
        // ------------------------------------------------

        if (IsArrayEmpty(step5_FinishLines))
            step5_FinishLines = CreateDialogue("CHỈ HUY", "Hệ thống ổn định. Chúc may mắn, chiến binh!");
    }

    void Start()
    {
        // Tự tìm FSM để check trạng thái Attack
        if (playerWeapon != null) playerFSM = playerWeapon.GetComponent<PlayerControllerFSM>();

        // Đảm bảo quái chạy bình thường lúc đầu
        EnemyBaseFSM.IsGlobalFrozen = false;

        StartCoroutine(RunTutorialFlow());
    }

    IEnumerator RunTutorialFlow()
    {
        // ====================================================
        // GIAI ĐOẠN 1: DI CHUYỂN
        // ====================================================
        yield return StartCoroutine(PlayDialogue(step1_IntroLines));
        currentStep = 0;
        while (currentStep == 0)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            {
                moveTimer += Time.deltaTime;
                if (moveTimer > 1.5f) currentStep++;
            }
            yield return null;
        }

        // ====================================================
        // GIAI ĐOẠN 2: RÚT KIẾM (PHÍM 1)
        // ====================================================
        yield return StartCoroutine(PlayDialogue(step2_EquipLines));
        while (currentStep == 1)
        {
            if (playerWeapon != null && playerWeapon.HasWeapon()) currentStep++;
            yield return null;
        }

        // ====================================================
        // GIAI ĐOẠN 3: TẤN CÔNG (CHUỘT TRÁI)
        // ====================================================
        yield return StartCoroutine(PlayDialogue(step3_AttackLines));

        while (currentStep == 2)
        {
            // Kiểm tra: Hoặc là trạng thái Attack, hoặc là bấm Chuột Trái (0)
            bool isAttackingState = (playerFSM != null && playerFSM.currentState == PlayerControllerFSM.State.Attack);
            bool isClicking = Input.GetMouseButtonDown(0);

            if (isAttackingState || isClicking)
            {
                // Chờ 0.5 giây cho animation chém nó đẹp mắt rồi mới dừng
                yield return new WaitForSeconds(0.5f);
                currentStep++;
            }
            yield return null;
        }

        // ====================================================
        // GIAI ĐOẠN 4: MẶT NẠ & CƠ CHẾ MÀU (SPACE)
        // ====================================================

        // 1. ĐÓNG BĂNG QUÁI (Để người chơi đọc hội thoại)
        EnemyBaseFSM.IsGlobalFrozen = true;

        // 2. HIỆN THOẠI GIẢI THÍCH QUY TẮC MÀU
        yield return StartCoroutine(PlayDialogue(step4_MaskLines));

        // 3. CHỜ NGƯỜI CHƠI BẤM SPACE
        while (currentStep == 3)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentStep++;
            }
            yield return null;
        }

        // 4. XẢ BĂNG (QUÁI HOẠT ĐỘNG LẠI)
        EnemyBaseFSM.IsGlobalFrozen = false;

        // ====================================================
        // GIAI ĐOẠN 5: KẾT THÚC
        // ====================================================
        yield return StartCoroutine(PlayDialogue(step5_FinishLines));
    }

    // --- HÀM HỖ TRỢ ---
    IEnumerator PlayDialogue(DialogueLine[] lines)
    {
        if (VisualNovelManager.Instance != null)
        {
            VisualNovelManager.Instance.StartConversation(lines);
            while (VisualNovelManager.Instance.IsDialogueActive) yield return null;
        }
    }

    DialogueLine[] CreateDialogue(string name, string content)
    {
        return new DialogueLine[] { new DialogueLine { characterName = name, sentence = content } };
    }

    bool IsArrayEmpty(DialogueLine[] arr)
    {
        return arr == null || arr.Length == 0;
    }
}