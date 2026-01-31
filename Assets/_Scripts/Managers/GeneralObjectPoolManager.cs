using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic Object Pool Manager - Quản lý pool cho bất kỳ loại GameObject nào
/// (Projectiles, Effects, Items, Debris, v.v.)
/// 
/// Cách dùng:
/// 1. Thêm script vào GameObject trong Scene
/// 2. Cấu hình PoolItem trong Inspector (tên pool + prefab + kích thước)
/// 3. Gọi: GeneralObjectPoolManager.Instance.SpawnObject("projectile", position);
/// </summary>
public class GeneralObjectPoolManager : MonoBehaviour
{
    public static GeneralObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public struct PoolItem
    {
        public string poolName;           // Tên định danh pool (vd: "Bullet", "Explosion", "Item")
        public GameObject prefab;         // Prefab để spawn
        public int initialPoolSize;       // Kích thước pool ban đầu
        public bool expandable;           // Cho phép tạo thêm nếu hết pool?
    }

    [Header("Pool Configuration")]
    [SerializeField] private List<PoolItem> poolItems = new List<PoolItem>();

    // Dictionary: poolName -> Queue of GameObjects
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, PoolItem> poolItemMap; // Để expand pool khi cần

    [Header("Debug")]
    public bool showDebugLogs = false;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }

    void InitializePool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolItemMap = new Dictionary<string, PoolItem>();

        foreach (var item in poolItems)
        {
            // Kiểm tra trùng tên
            if (poolDictionary.ContainsKey(item.poolName))
            {
                Debug.LogError($"❌ Pool '{item.poolName}' đã tồn tại! Tên pool phải duy nhất.");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Tạo pool ban đầu
            for (int i = 0; i < item.initialPoolSize; i++)
            {
                GameObject obj = CreateNewObject(item.prefab, item.poolName);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(item.poolName, objectPool);
            poolItemMap.Add(item.poolName, item);

            if (showDebugLogs)
                Debug.Log($"✅ Initialized pool '{item.poolName}' với {item.initialPoolSize} objects.");
        }
    }

    GameObject CreateNewObject(GameObject prefab, string poolName)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = $"{prefab.name} [Pool: {poolName}]";
        obj.SetActive(false);
        return obj;
    }

    // --- PUBLIC API: SPAWN OBJECT ---
    /// <summary>
    /// Spawn một object từ pool
    /// </summary>
    public GameObject SpawnObject(string poolName, Vector3 position)
    {
        return SpawnObject(poolName, position, Quaternion.identity);
    }

    public GameObject SpawnObject(string poolName, Vector3 position, Quaternion rotation)
    {
        // Kiểm tra pool tồn tại
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"⚠️ Pool '{poolName}' không tồn tại! Kiểm tra tên pool lại.");
            return null;
        }

        GameObject objToSpawn;

        // Lấy từ queue nếu còn
        if (poolDictionary[poolName].Count > 0)
        {
            objToSpawn = poolDictionary[poolName].Dequeue();
        }
        else
        {
            // Hết hàng -> cố gắng expand nếu cho phép
            PoolItem item = poolItemMap[poolName];
            if (item.expandable)
            {
                if (showDebugLogs)
                    Debug.Log($"📦 Pool '{poolName}' hết hàng, tạo object mới...");
                objToSpawn = CreateNewObject(item.prefab, poolName);
            }
            else
            {
                Debug.LogWarning($"❌ Pool '{poolName}' hết hàng và không thể expand!");
                return null;
            }
        }

        // Thiết lập vị trí, rotation và kích hoạt
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.SetActive(true);

        if (showDebugLogs)
            Debug.Log($"▶️ Spawned '{poolName}' at {position}");

        return objToSpawn;
    }

    public GameObject SpawnObject(string poolName, Transform parent)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"⚠️ Pool '{poolName}' không tồn tại!");
            return null;
        }

        GameObject objToSpawn = poolDictionary[poolName].Count > 0
            ? poolDictionary[poolName].Dequeue()
            : CreateNewObject(poolItemMap[poolName].prefab, poolName);

        objToSpawn.transform.SetParent(parent);
        objToSpawn.transform.localPosition = Vector3.zero;
        objToSpawn.transform.localRotation = Quaternion.identity;
        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    // --- PUBLIC API: RETURN TO POOL ---
    public void ReturnToPool(GameObject obj, string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"⚠️ Pool '{poolName}' không tồn tại! Hủy object thay vì return.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[poolName].Enqueue(obj);

        if (showDebugLogs)
            Debug.Log($"◀️ Returned object to pool '{poolName}'");
    }

    // --- PUBLIC API: QUERY POOL INFO ---
    public int GetPoolCount(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
            return -1;
        return poolDictionary[poolName].Count;
    }

    public bool PoolExists(string poolName)
    {
        return poolDictionary.ContainsKey(poolName);
    }

    // --- DEBUG: CLEAR POOL ---
    public void ClearPool(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"⚠️ Pool '{poolName}' không tồn tại!");
            return;
        }

        // Xóa tất cả object trong pool
        while (poolDictionary[poolName].Count > 0)
        {
            GameObject obj = poolDictionary[poolName].Dequeue();
            Destroy(obj);
        }

        Debug.Log($"🗑️ Cleared pool '{poolName}'");
    }

    public void ClearAllPools()
    {
        foreach (var key in poolDictionary.Keys)
        {
            ClearPool(key);
        }
        Debug.Log("🗑️ Cleared all pools");
    }

    // --- DEBUG: PRINT POOL STATUS ---
    public void PrintPoolStatus()
    {
        Debug.Log("=== POOL STATUS ===");
        foreach (var kvp in poolDictionary)
        {
            Debug.Log($"  [{kvp.Key}] Available: {kvp.Value.Count}");
        }
    }
}
