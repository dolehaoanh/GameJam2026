using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon Strategy/Basic Weapon")]
public class WeaponStrategySO : ScriptableObject
{
    [Header("--- CẤU HÌNH ANIMATION ---")]
    // Đây là "LINH HỒN" của việc đổi animation
    // Nó sẽ thay thế các clip mặc định bằng clip của vũ khí này
    public string weaponName; 
    public AnimatorOverrideController overrideController;

    [Header("--- CẤU HÌNH CHIẾN ĐẤU ---")]
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;

    [Header("--- VISUAL KHÁC ---")]
    public Sprite weaponIcon; // Để hiển thị UI nếu cần

    // Hàm xử lý tấn công riêng (Strategy Pattern logic)
    public virtual void PerformAttack(Transform playerTransform, Transform attackPoint)
    {
        // Mặc định là xử lý đánh gần (Melee)
        // Nếu là súng (Ranged), đồng chí tạo class con kế thừa rồi override hàm này
        Debug.Log($"Đang tấn công bằng {this.name} với damage {attackDamage}");
        
        // Ví dụ: Physics2D.OverlapCircle... để gây damage
    }
}
