using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class StartupManager
{
    private const string PREF_KEY_LOCK = "MyGame_LockStartup";

    public static bool IsLocked
    {
        get => EditorPrefs.GetBool(PREF_KEY_LOCK, false);
        set
        {
            EditorPrefs.SetBool(PREF_KEY_LOCK, value);
            ApplyStartupLogic();
        }
    }

    static StartupManager()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.delayCall += ApplyStartupLogic;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode) ApplyStartupLogic();
    }

    private static void ApplyStartupLogic()
    {
        if (!IsLocked)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        var scenes = EditorBuildSettings.scenes;
        if (scenes.Length > 0)
        {
            SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[0].path);
            if (startScene != null) EditorSceneManager.playModeStartScene = startScene;
            else IsLocked = false;
        }
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        // --- SỬA TỌA ĐỘ TẠI ĐÂY ---
        float width = 140f; 
        float height = 22f;
        
        // SceneSelector rộng 200px (tức là mỗi bên 100px từ tâm).
        // Ta cần lùi sang trái: 100px (của selector) + 10px (khoảng hở) + 140px (chiều rộng nút này) = 250px.
        // Để cho thoáng, tôi để -260f.
        float x = (sceneView.position.width / 2f) - 260f; 
        float y = 10f;

        GUILayout.BeginArea(new Rect(x, y, width, height));

        string startSceneName = "None";
        if (EditorBuildSettings.scenes.Length > 0)
        {
            startSceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[0].path);
        }

        GUIStyle toggleStyle = new GUIStyle(EditorStyles.miniButton);
        toggleStyle.fixedHeight = height; // Cố định chiều cao cho bằng nút bên cạnh

        if (IsLocked)
        {
            toggleStyle.normal.textColor = Color.green;
            if (GUILayout.Button($"🔒 Start: {startSceneName}", toggleStyle))
            {
                IsLocked = false;
                Debug.Log("🔓 Đã mở khóa Startup Scene.");
            }
        }
        else
        {
            toggleStyle.normal.textColor = Color.gray;
            if (GUILayout.Button($"🔓 Play Current", toggleStyle))
            {
                IsLocked = true;
                Debug.Log($"🔒 Đã khóa Startup Scene: {startSceneName}");
            }
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }
}