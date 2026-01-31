using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu vật va chạm có Tag là "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Tắt kẻ địch ngay lập tức
            other.gameObject.SetActive(false);
            Debug.Log($"💀 Đã tiễn {other.name} lên bảng đếm số!");
        }
    }
}