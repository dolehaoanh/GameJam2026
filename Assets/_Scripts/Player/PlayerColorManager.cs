using UnityEngine;

// Định nghĩa Enum này ở ngoài class để script nào cũng gọi được
public enum MaskType { Red, White, Black }

public class PlayerColorManager : MonoBehaviour
{
    [Header("Cấu hình màu sắc")]
    public Color colorRed = Color.red;
    public Color colorWhite = Color.white;
    public Color colorBlack = Color.black;

    [Header("Trạng thái hiện tại")]
    [SerializeField] private MaskType currentMask;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Mặc định vào game là màu Đỏ
        SetMask(MaskType.Red);
    }

    // Hàm đổi màu trực tiếp
    public void SetMask(MaskType type)
    {
        currentMask = type;
        
        if (sr == null) sr = GetComponent<SpriteRenderer>();

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
    }

    // Hàm xoay vòng (Logic cho phím Space)
    public void CycleNextMask()
    {
        // Cộng 1 rồi chia lấy dư cho 3 để quay vòng 0->1->2->0
        int nextIndex = ((int)currentMask + 1) % 3;
        SetMask((MaskType)nextIndex);
    }

    // Hàm public để các script logic khác (như Quái vật/Bẫy) kiểm tra xem Player đang đeo mặt nạ gì
    public MaskType GetCurrentMask()
    {
        return currentMask;
    }
}