using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ManualAnimCreator : EditorWindow
{
    [Space(10)]
    [Header("1. CẤU HÌNH ĐẦU RA")]
    public DefaultAsset targetFolder;
    public string animName = "New Animation";
    
    [Space(10)]
    [Header("2. THÔNG SỐ")]
    public float frameRate = 12f;
    public bool loop = true;

    [Space(10)]
    [Header("3. KÉO THẢ SPRITE VÀO ĐÂY")]
    [SerializeField] private List<Sprite> spriteList = new List<Sprite>();

    private SerializedObject serializedObject;
    private SerializedProperty spriteListProperty;

    [MenuItem("Tools/Mad Scientist/Manual Anim Creator")]
    public static void ShowWindow()
    {
        GetWindow<ManualAnimCreator>("Anim Creator v2.2");
    }

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        spriteListProperty = serializedObject.FindProperty("spriteList");
    }

    private void OnGUI()
    {
        serializedObject.Update();

        // 1. Cấu hình
        GUILayout.Label("1. CẤU HÌNH", EditorStyles.boldLabel);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Thư mục lưu:", targetFolder, typeof(DefaultAsset), false);
        animName = EditorGUILayout.TextField("Tên Animation:", animName);

        // 2. Thông số
        GUILayout.Space(5);
        GUILayout.Label("2. THÔNG SỐ", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        frameRate = EditorGUILayout.FloatField("FPS:", frameRate);
        loop = EditorGUILayout.Toggle("Loop:", loop);
        GUILayout.EndHorizontal();

        // 3. Danh sách
        GUILayout.Space(10);
        GUILayout.Label("3. FRAMES (Kéo vào tự đặt tên)", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(spriteListProperty, new GUIContent("Danh sách Sprite"), true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            AutoNameFromSprite();
        }

        // 4. Nút tạo
        GUILayout.Space(20);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🎬 TẠO ANIMATION (Auto Reset)", GUILayout.Height(40)))
        {
            CreateAnimClip();
        }
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    private void AutoNameFromSprite()
    {
        if (spriteList.Count == 0 || spriteList[0] == null) return;
        string rawName = spriteList[0].name;
        // Regex xóa số đuôi
        string cleanName = Regex.Replace(rawName, @"[_\-\s\(]+\d+[\)]*$", "");
        animName = cleanName;
    }

    private void CreateAnimClip()
    {
        if (spriteList == null || spriteList.Count == 0)
        {
            EditorUtility.DisplayDialog("Lỗi", "Danh sách trống!", "OK");
            return;
        }
        
        spriteList.RemoveAll(s => s == null);

        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
        float timePerFrame = 1.0f / frameRate;

        for (int i = 0; i < spriteList.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = i * timePerFrame;
            keyFrames[i].value = spriteList[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        string savePath = "Assets/";
        if (targetFolder != null)
        {
            savePath = AssetDatabase.GetAssetPath(targetFolder) + "/";
        }

        string finalPath = savePath + animName + ".anim";
        finalPath = AssetDatabase.GenerateUniqueAssetPath(finalPath);

        AssetDatabase.CreateAsset(clip, finalPath);
        AssetDatabase.SaveAssets();

        EditorGUIUtility.PingObject(clip);
        Debug.Log($"✅ Đã tạo: {finalPath}");

        // --- TỰ ĐỘNG RESET ---
        spriteList.Clear();
        animName = "New Animation";
        GUI.FocusControl(null); // Bỏ chọn ô text để tránh lỗi nhập liệu
        Repaint(); // Cập nhật lại giao diện ngay lập tức
    }
}