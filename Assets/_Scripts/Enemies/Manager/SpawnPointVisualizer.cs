using UnityEngine;

public class SpawnPointVisualizer : MonoBehaviour
{
    [Header("Debug Visuals")]
    public Color debugColor = Color.cyan;
    public float radius = 0.5f;

    // Hàm này giúp vẽ hình trong màn hình Scene của Editor
    void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(transform.position, radius);
        
        // Vẽ thêm cái dây nối xuống đất để dễ căn chỉnh
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 2);
    }

    // Vẽ tên khi chọn vào
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * 1.5f);
    }
}