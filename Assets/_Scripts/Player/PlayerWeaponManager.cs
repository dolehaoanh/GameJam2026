using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("--- KHO VŨ KHÍ ---")]
    public WeaponStrategySO swordWeaponData;

    [Header("--- DEBUG INFO ---")]
    public WeaponStrategySO currentWeapon;
    private RuntimeAnimatorController originalController;
    private Animator animator;

    // Xóa biến controller FSM vì Manager không cần soi state nữa, việc đó của Visuals
    // Xóa mấy biến string debug cũ đi cho gọn

    void Awake()
    {
        // 1. TÌM ANIMATOR (QUAN TRỌNG: Chỉ lấy của con, không lấy của bố)
        animator = GetComponentInChildren<Animator>();

        // Cảnh báo nếu bố cũng có Animator (Gây tranh chấp)
        if (GetComponent<Animator>() != null)
        {
            Debug.LogError("❌ LỖI NGHIÊM TRỌNG: Thằng Bố (Player) đang có component Animator! Hãy Remove nó ngay, chỉ để Animator ở thằng Con (Visuals) thôi!");
        }

        if (animator != null)
            originalController = animator.runtimeAnimatorController;
    }

    void Update()
    {
        // Phím tắt test
        if (Input.GetKeyDown(KeyCode.Alpha1)) UnequipWeapon();
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(swordWeaponData);
    }

    // --- HÀM NÀY CHỈ CÒN NHIỆM VỤ THAY SÚNG (DATA) ---
    public void EquipWeapon(WeaponStrategySO newWeapon)
    {
        if (newWeapon == null || animator == null) return;

        currentWeapon = newWeapon;

        // Đây là dòng duy nhất Manager được phép can thiệp vào Animator: TRÁO RUỘT
        if (newWeapon.overrideController != null)
        {
            animator.runtimeAnimatorController = newWeapon.overrideController;
            Debug.Log($"⚔️ Đã thay Controller thành: {newWeapon.overrideController.name}");
        }
    }

    public void UnequipWeapon()
    {
        currentWeapon = null;
        if (originalController != null && animator != null)
        {
            animator.runtimeAnimatorController = originalController;
            Debug.Log("✋ Đã về tay không");
        }
    }

    // --- HÀM ATTACK CŨ (NGUYÊN NHÂN GÂY LỖI) ---
    public void Attack()
    {
        // ❌ XÓA DÒNG NÀY NGAY: animator.SetTrigger("Attack");

        // Để trống hoặc chỉ log thôi. 
        // Việc chạy Animation bây giờ là do PlayerVisuals nó tự nhìn thấy FSM chuyển sang Attack là nó tự chạy.
        // Manager không cần làm gì cả.
        // Debug.Log("Manager: Đã nhận lệnh đánh, nhưng việc múa là của thằng Visuals lo!");
    }

    // --- GUI DEBUG (Giữ lại nếu thích) ---
    void OnGUI()
    {
        if (animator == null) return;
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.yellow;
        GUILayout.Label($"Weapon: {(currentWeapon ? currentWeapon.name : "Hand")}", style);
        GUILayout.Label($"Controller: {animator.runtimeAnimatorController.name}", style);
    }
}