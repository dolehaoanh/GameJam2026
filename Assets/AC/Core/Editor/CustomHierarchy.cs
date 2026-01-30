#pragma warning disable CS0618 // Tắt cảnh báo API cũ để code gọn gàng

using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchy
{
    private static Vector2 offset = new Vector2(0, 0); // Đã chỉnh lại offset cho đẹp

    static CustomHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyWindowItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        // Lấy object từ ID
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        // Logic: Nếu tên có dấu "#" thì đổi màu nền
        if (obj.name.Contains("#"))
        {
            // Màu mặc định (Xám đậm)
            Color backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            Color fontColor = Color.white;

            // Nếu đang được chọn thì đổi màu xanh của Unity
            if (Selection.instanceIDs.Contains(instanceID))
            {
                backgroundColor = new Color(0.24f, 0.48f, 0.90f);
            }

            // Vẽ hình chữ nhật nền màu
            Rect bgRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width + 50, selectionRect.height);
            EditorGUI.DrawRect(bgRect, backgroundColor);

            // Vẽ lại tên Object đè lên trên cho rõ
            Rect textRect = new Rect(selectionRect.x + 18, selectionRect.y, selectionRect.width, selectionRect.height);
            EditorGUI.LabelField(textRect, obj.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = fontColor },
                fontStyle = FontStyle.Bold
            });
        }
    }
}