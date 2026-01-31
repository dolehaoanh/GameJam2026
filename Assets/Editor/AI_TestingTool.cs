using UnityEngine;
using UnityEditor;
using NavMeshPlus.Components;

public class AI_TestingTool : EditorWindow
{
    // --- CẤU HÌNH TOOL ---
    string spawnPointName = "SpawnPoint";
    int spawnCount = 5;
    float spawnRadius = 10f;
    Transform spawnRoot;

    Color zombieColor = new Color(1f, 0.3f, 0.3f);
    Color skeletonColor = new Color(0.9f, 0.9f, 0.9f);
    Color chargerColor = new Color(0.2f, 0.2f, 0.2f);

    [MenuItem("Mad Tools/AI Testing Generator (Sorting Layer Only)")]
    public static void ShowWindow()
    {
        GetWindow<AI_TestingTool>("AI Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("--- 1. RẢI ĐIỂM SPAWN ---", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        spawnPointName = EditorGUILayout.TextField("Tên Point:", spawnPointName);
        spawnCount = EditorGUILayout.IntField("Số lượng:", spawnCount);
        spawnRadius = EditorGUILayout.FloatField("Bán kính rải:", spawnRadius);
        spawnRoot = (Transform)EditorGUILayout.ObjectField("Thư mục cha:", spawnRoot, typeof(Transform), true);

        if (GUILayout.Button("Rải Spawn Points (+ Auto Bake)", GUILayout.Height(30)))
        {
            CreateSpawnPoints();
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.Label("--- 2. TẠO QUÁI TEST (DUMMY) ---", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Chỉ tự động gán SORTING LAYER = 'Enemy'", EditorStyles.miniLabel);

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("Spawn ZOMBIE", GUILayout.Height(30)))
            CreateEnemyDummy(EnemyType.Zombie, "Dummy_Zombie", zombieColor);

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Spawn SKELETON", GUILayout.Height(30)))
            CreateEnemyDummy(EnemyType.Skeleton, "Dummy_Skeleton", skeletonColor);

        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Spawn CHARGER", GUILayout.Height(30)))
            CreateEnemyDummy(EnemyType.Charger, "Dummy_Charger", chargerColor);
        
        EditorGUILayout.EndVertical();

        GUI.backgroundColor = Color.white;
        GUILayout.Space(20);
        if (GUILayout.Button("CHỈ BAKE NAVMESH THÔI", GUILayout.Height(40)))
        {
            ForceRebake();
        }
    }

    // --- LOGIC CHÍNH ---

    void CreateSpawnPoints()
    {
        if (spawnRoot == null)
        {
            GameObject rootObj = new GameObject("--- SPAWN POINTS ROOT ---");
            spawnRoot = rootObj.transform;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject point = new GameObject($"{spawnPointName}_{i + 1}");
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            point.transform.position = new Vector3(randomPos.x, randomPos.y, 0);
            point.transform.SetParent(spawnRoot);
            
            // Script hiển thị Gizmos (Đã có từ bước trước)
            point.AddComponent<SpawnPointVisualizer>();
            
            Undo.RegisterCreatedObjectUndo(point, "Create Spawn Point");
        }
        
        Debug.Log($"Đã rải {spawnCount} điểm spawn.");
        ForceRebake();
    }

    void CreateEnemyDummy(EnemyType type, string name, Color color)
    {
        GameObject enemyObj = new GameObject(name);
        enemyObj.transform.position = Vector3.zero;

        // 1. TẠO VISUAL VÀ CHỈ GÁN SORTING LAYER
        SpriteRenderer sr = enemyObj.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateDefaultSprite(color);

        // --- CHỈ XỬ LÝ SORTING LAYER ---
        if (IsSortingLayerExist("Enemy"))
        {
            sr.sortingLayerName = "Enemy"; // Chỉ set cái này!
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa tạo Sorting Layer 'Enemy'. Đã gán tạm Order = 5.");
            sr.sortingOrder = 5; // Fallback nếu chưa tạo
        }
        // --------------------------------

        // 2. GÁN SCRIPT AI
        EnemyBaseFSM script = null;
        switch (type)
        {
            case EnemyType.Zombie:
                script = enemyObj.AddComponent<EnemyMelee>();
                script.enemyMask = MaskType.Red;
                break;

            case EnemyType.Skeleton:
                script = enemyObj.AddComponent<EnemyRanged>();
                script.enemyMask = MaskType.White;
                GameObject gun = new GameObject("FirePoint");
                gun.transform.SetParent(enemyObj.transform);
                gun.transform.localPosition = Vector3.right * 0.6f;
                script.GetType().GetField("firePoint")?.SetValue(script, gun.transform);
                break;

            case EnemyType.Charger:
                script = enemyObj.AddComponent<EnemyCharger>();
                script.enemyMask = MaskType.Black;
                break;
        }

        // 3. CHỈNH COLLIDER
        BoxCollider2D box = enemyObj.GetComponent<BoxCollider2D>();
        if (box != null) box.size = new Vector2(0.64f, 0.64f);

        Undo.RegisterCreatedObjectUndo(enemyObj, "Create Dummy Enemy");
        Selection.activeGameObject = enemyObj;

        Debug.Log($"Đã tạo {name} (Sorting Layer: {sr.sortingLayerName}). Đang Re-bake...");
        ForceRebake();
    }

    // --- TIỆN ÍCH ---

    void ForceRebake()
    {
        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("<color=green>✔ AUTO-BAKED NAVMESH!</color>");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy 'NavMeshSurface'!");
        }
    }

    bool IsSortingLayerExist(string layerName)
    {
        foreach (var layer in SortingLayer.layers)
        {
            if (layer.name == layerName) return true;
        }
        return false;
    }

    Sprite GenerateDefaultSprite(Color color)
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool isBorder = (x < 2 || x >= size - 2 || y < 2 || y >= size - 2);
                texture.SetPixel(x, y, isBorder ? Color.black : color);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}