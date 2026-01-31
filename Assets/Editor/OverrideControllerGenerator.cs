using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class OverrideControllerGenerator : EditorWindow
{
    // --- INPUT ---
    public RuntimeAnimatorController baseController; // Animator gốc (Để lấy danh sách state cần override)
    public DefaultAsset targetFolder; // Thư mục chứa Animation mới (Kéo folder vũ khí vào đây)
    public string outputName = "New_Weapon_Override"; // Tên file sinh ra

    // --- PREVIEW ---
    Vector2 scrollPos;
    List<string> logMessages = new List<string>();

    [MenuItem("Mad Tools/Weapon Override Generator (Auto Map)")]
    public static void ShowWindow()
    {
        GetWindow<OverrideControllerGenerator>("Auto Mapper");
    }

    void OnGUI()
    {
        GUILayout.Label("CÔNG CỤ TẠO OVERRIDE NHANH", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 1. CHỌN BASE CONTROLLER
        baseController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Gốc:", baseController, typeof(RuntimeAnimatorController), false);
        
        // 2. CHỌN FOLDER CHỨA ANIMATION MỚI
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Folder Anim Mới:", targetFolder, typeof(DefaultAsset), false);

        // 3. TÊN FILE OUTPUT
        outputName = EditorGUILayout.TextField("Tên File Output:", outputName);

        GUILayout.Space(20);

        if (baseController == null || targetFolder == null)
        {
            EditorGUILayout.HelpBox("Hãy kéo đủ Animator Gốc và Folder chứa Animation mới vào!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("🚀 QUÉT VÀ TẠO NGAY", GUILayout.Height(40)))
        {
            Generate();
        }

        GUILayout.Space(10);
        GUILayout.Label("Log Kết Quả:", EditorStyles.miniLabel);
        
        // HIỂN THỊ LOG
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "box", GUILayout.Height(300));
        foreach (var msg in logMessages)
        {
            if (msg.StartsWith("✅")) GUI.color = Color.green;
            else if (msg.StartsWith("❌")) GUI.color = Color.red;
            else GUI.color = Color.white;
            
            GUILayout.Label(msg);
        }
        GUI.color = Color.white;
        EditorGUILayout.EndScrollView();
    }

    void Generate()
    {
        logMessages.Clear();
        string folderPath = AssetDatabase.GetAssetPath(targetFolder);

        // 1. TÌM TẤT CẢ ANIMATION CLIP TRONG FOLDER
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
        List<AnimationClip> newClips = new List<AnimationClip>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null) newClips.Add(clip);
        }

        logMessages.Add($"📂 Tìm thấy {newClips.Count} clips trong folder '{targetFolder.name}'");

        // 2. TẠO OVERRIDE CONTROLLER MỚI
        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        
        // Lấy danh sách clip gốc từ Base Controller
        foreach (AnimationClip originalClip in baseController.animationClips)
        {
            // BỎ QUA NẾU LÀ CLIP RỖNG
            if (originalClip == null) continue;

            // 3. THUẬT TOÁN TÌM KIẾM THÔNG MINH (SMART MATCHING)
            AnimationClip matchedClip = FindBestMatch(originalClip.name, newClips);

            if (matchedClip != null)
            {
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, matchedClip));
                logMessages.Add($"✅ Map thành công: [{originalClip.name}] ---> [{matchedClip.name}]");
            }
            else
            {
                // Nếu không tìm thấy, giữ nguyên clip cũ
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, null)); // null = dùng cái cũ
                logMessages.Add($"❌ KHÔNG TÌM THẤY file nào khớp với: [{originalClip.name}]");
            }
        }

        // 4. ÁP DỤNG VÀ LƯU FILE
        overrideController.ApplyOverrides(overrides);

        string savePath = Path.Combine(folderPath, outputName + ".overrideController");
        // Đảm bảo tên unique
        savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

        AssetDatabase.CreateAsset(overrideController, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        logMessages.Add("-----------------------------");
        logMessages.Add($"🎉 ĐÃ TẠO XONG: {savePath}");
        
        // Ping file vừa tạo
        EditorGUIUtility.PingObject(overrideController);
    }

    // --- THUẬT TOÁN SO KHỚP TÊN ---
    AnimationClip FindBestMatch(string originalName, List<AnimationClip> candidates)
    {
        // VÍ DỤ: 
        // Original: "Idle_Down"
        // Candidate: "Sword_1_Template_Idle_Down-Sheet"

        // Cách 1: Tên candidate CHỨA trọn vẹn tên Original (Case Insensitive)
        // Đây là cách an toàn nhất
        string searchKey = originalName.ToLower();

        // Ưu tiên tìm chính xác trước (Phòng trường hợp Attack_1 vs Attack_10)
        foreach (var clip in candidates)
        {
            string clipName = clip.name.ToLower();
            
            // Logic so sánh:
            // 1. Clip mới phải chứa cụm từ của clip cũ (VD: chứa "idle_down")
            // 2. (Tùy chọn) Để tránh nhầm lẫn Attack_1 với Attack_1_Combo, có thể check kỹ hơn
            
            if (clipName.Contains(searchKey))
            {
                return clip;
            }
        }
        
        // Cách 2 (Nếu đặt tên khác kiểu): Tách từ khóa (Idle, Down) và tìm clip chứa CẢ HAI
        string[] keywords = originalName.Split('_'); // {"Idle", "Down"}
        foreach (var clip in candidates)
        {
            bool matchAll = true;
            string clipName = clip.name.ToLower();
            foreach (var key in keywords)
            {
                if (!clipName.Contains(key.ToLower()))
                {
                    matchAll = false;
                    break;
                }
            }
            if (matchAll) return clip;
        }

        return null;
    }
}