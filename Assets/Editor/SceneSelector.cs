using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class SceneSelector
{
    // Chạy ngay khi Unity load xong
    static SceneSelector()
    {
        SceneView.duringSceneGui -= OnSceneGUI; // Tránh trùng lặp
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        // Vị trí: Góc trên cùng, chính giữa màn hình Scene View
        // Đây là chỗ dễ nhìn nhất khi đang làm Level
        float width = 200f;
        float height = 25f;
        float x = (sceneView.position.width - width) / 2f;
        float y = 10f;

        GUILayout.BeginArea(new Rect(x, y, width, height));

        // Lấy tên Scene hiện tại để hiển thị lên nút
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(currentSceneName)) currentSceneName = "Unsaved Scene";

        // Vẽ nút bấm. Bấm vào sẽ sổ ra danh sách Scene
        GUIStyle style = new GUIStyle(EditorStyles.popup);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow; // Màu vàng cho dễ thấy

        if (GUILayout.Button($"🎬 {currentSceneName}", style))
        {
            ShowSceneMenu();
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void ShowSceneMenu()
    {
        GenericMenu menu = new GenericMenu();

        // 1. Lấy tất cả Scene trong Build Settings (Các màn chơi chính)
        var buildScenes = EditorBuildSettings.scenes;
        if (buildScenes.Length > 0)
        {
            menu.AddDisabledItem(new GUIContent("--- Build Settings Scenes ---"));
            foreach (var scene in buildScenes)
            {
                if (scene.enabled)
                {
                    string path = scene.path;
                    string name = Path.GetFileNameWithoutExtension(path);
                    menu.AddItem(new GUIContent(name), false, () => OpenScene(path));
                }
            }
            menu.AddSeparator("");
        }

        // 2. Quét toàn bộ Project tìm tất cả file .unity (Dành cho scene test chưa add vào build)
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        if (guids.Length > 0)
        {
            menu.AddDisabledItem(new GUIContent("--- All Project Scenes ---"));
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileNameWithoutExtension(path);
                
                // Tránh lặp lại tên nếu muốn, hoặc cứ hiện hết để dễ tìm
                menu.AddItem(new GUIContent($"All/{name}"), false, () => OpenScene(path));
            }
        }

        menu.ShowAsContext();
    }

    private static void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}