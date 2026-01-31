using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("--- KHO VŨ KHÍ ---")]
    public WeaponStrategySO swordWeaponData; // Kéo file ScriptableObject kiếm vào đây

    [Header("--- DEBUG INFO ---")]
    public WeaponStrategySO currentWeapon; // Biến này null nghĩa là tay không
    private RuntimeAnimatorController originalController;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        // Cảnh báo nếu bố cũng có Animator
        if (GetComponent<Animator>() != null) Debug.LogError("❌ XÓA ANIMATOR Ở PLAYER ĐI!");

        if (animator != null) originalController = animator.runtimeAnimatorController;
    }

    void Update()
    {
        // --- YÊU CẦU 1: BẤM PHÍM 1 ĐỂ RÚT KIẾM ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(swordWeaponData);
            Debug.Log("⚔️ Đã rút kiếm! Giờ mới được chém!");
        }

        // Bấm 2 để cất kiếm (về tay không) - Để test
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UnequipWeapon();
            Debug.Log("✋ Đã cất kiếm (Tay không)");
        }
    }

    // --- HÀM KIỂM TRA: CÓ HÀNG KHÔNG? ---
    public bool HasWeapon()
    {
        return currentWeapon != null; // Trả về TRUE nếu đang cầm vũ khí
    }

    public void EquipWeapon(WeaponStrategySO newWeapon)
    {
        if (newWeapon == null || animator == null) return;
        currentWeapon = newWeapon;
        if (newWeapon.overrideController != null)
            animator.runtimeAnimatorController = newWeapon.overrideController;
    }

    public void UnequipWeapon()
    {
        currentWeapon = null;
        if (originalController != null && animator != null)
            animator.runtimeAnimatorController = originalController;
    }

    public void Attack()
    {
        // Hàm này để trống hoặc log thôi, Visuals lo hết rồi
    }
}