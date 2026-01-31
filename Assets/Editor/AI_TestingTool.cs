using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using NavMeshPlus.Components; // Thư viện NavMeshPlus

public class AI_TestingTool : EditorWindow
{
    // --- CẤU HÌNH TOOL ---
    string spawnPointName = "SpawnPoint";
    int spawnCount = 5;
    float spawnRadius = 10f;
    Transform spawnRoot;

    // Màu sắc
    Color zombieColor = new Color(1f, 0.3f, 0.3f); // Đỏ
    Color skeletonColor = new Color(0.9f, 0.9f, 0.9f); // Trắng
    Color chargerColor = new Color(0.2f, 0.2f, 0.2f); // Đen

    [MenuItem("Mad Tools/AI Generator (Ultimate)")]
    public static void ShowWindow()
    {
        GetWindow<AI_TestingTool>("AI Ultimate");
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
        GUILayout.Label("Tự động: Capsule Collider, NavMesh Radius nhỏ, Charger Stats xịn", EditorStyles.miniLabel);

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("Spawn ZOMBIE (Melee)", GUILayout.Height(30)))
            CreateEnemyDummy(EnemyType.Zombie, "Dummy_Zombie", zombieColor);

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Spawn SKELETON (Ranged)", GUILayout.Height(30)))
            CreateEnemyDummy(EnemyType.Skeleton, "Dummy_Skeleton", skeletonColor);

        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Spawn CHARGER (Tank Húc)", GUILayout.Height(30)))
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
            
            // Gắn visualizer (yêu cầu đã có script SpawnPointVisualizer)
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

        // 1. VISUAL (SPRITE & LAYER)
        SpriteRenderer sr = enemyObj.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateDefaultSprite(color);
        
        // Chỉ gán Sorting Layer (Không chạm vào Physics Layer)
        if (IsSortingLayerExist("Enemy")) sr.sortingLayerName = "Enemy";
        else sr.sortingOrder = 5;

        // 2. GÁN SCRIPT AI & CẤU HÌNH STATS
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
                
                // Gán súng vào biến firePoint (nếu tìm thấy)
                script.GetType().GetField("firePoint")?.SetValue(script, gun.transform);
                break;

            case EnemyType.Charger:
                // --- CẤU HÌNH RIÊNG CHO CHARGER ---
                var charger = enemyObj.AddComponent<EnemyCharger>();
                charger.enemyMask = MaskType.Black;
                
                // Chỉ số "Bò Điên" theo yêu cầu:
                charger.attackRange = 8f;       // Tầm kích hoạt xa 8m
                charger.chargeSpeed = 30f;      // Tốc độ tên lửa
                charger.chargeDuration = 0.5f;  // Húc nhanh gọn
                
                script = charger;
                break;
        }

        // 3. XỬ LÝ COLLIDER (CAPSULE CHỐNG KẸT)
        // Nếu script FSM tự thêm BoxCollider2D (do RequireComponent cũ), ta xóa nó đi
        BoxCollider2D box = enemyObj.GetComponent<BoxCollider2D>();
        if (box != null) DestroyImmediate(box);

        // Thêm CapsuleCollider2D (Trơn hơn Box)
        CapsuleCollider2D capsule = enemyObj.GetComponent<CapsuleCollider2D>();
        if (capsule == null) capsule = enemyObj.AddComponent<CapsuleCollider2D>();
        
        capsule.size = new Vector2(0.6f, 0.6f);
        capsule.direction = CapsuleDirection2D.Vertical;

        // 4. TỐI ƯU NAVMESH AGENT
        NavMeshAgent agent = enemyObj.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.25f; // Bán kính nhỏ để luồn lách tốt
            agent.height = 1f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }

        Undo.RegisterCreatedObjectUndo(enemyObj, "Create Dummy Enemy");
        Selection.activeGameObject = enemyObj;

        Debug.Log($"Đã tạo {name} (Stats chuẩn). Đang Re-bake...");
        ForceRebake();
    }

    // --- TIỆN ÍCH ---

    void ForceRebake()
    {
        NavMeshSurface surface = FindFirstObjectByType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("<color=green>✔ AUTO-BAKED NAVMESH!</color>");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy 'NavMeshSurface'! Hãy tạo GameObject NavMesh trước.");
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