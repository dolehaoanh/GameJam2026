using UnityEngine;
using System.Collections.Generic;

public enum EnemyType
{
    Zombie,   // Melee
    Skeleton, // Ranged
    Charger   // Tank
}
public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    [System.Serializable]
    public struct PoolItem
    {
        public EnemyType type;
        public GameObject prefab;
        public int initialPoolSize;
    }

    [Header("Pool Configuration")]
    public List<PoolItem> poolItems; // Kéo Prefab vào đây trong Inspector

    // Dictionary để quản lý từng hàng đợi cho từng loại quái
    private Dictionary<EnemyType, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
    }

    void InitializePool()
    {
        poolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();

        foreach (var item in poolItems)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < item.initialPoolSize; i++)
            {
                GameObject obj = CreateNewEnemy(item.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(item.type, objectPool);
        }
    }

    GameObject CreateNewEnemy(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform); // Con của PoolManager cho gọn
        obj.SetActive(false);
        return obj;
    }

    // --- HÀM XUẤT KHO (SPAWN) ---
    public GameObject SpawnEnemy(EnemyType type, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool không có loại {type}!");
            return null;
        }

        GameObject objToSpawn;

        // Nếu còn hàng trong kho thì lấy ra
        if (poolDictionary[type].Count > 0)
        {
            objToSpawn = poolDictionary[type].Dequeue();
        }
        else
        {
            // Hết hàng thì tạo mới (Expand pool)
            PoolItem item = poolItems.Find(x => x.type == type);
            objToSpawn = CreateNewEnemy(item.prefab);
        }

        // Kích hoạt và đặt vị trí
        objToSpawn.transform.position = position;
        objToSpawn.SetActive(true);

        // Reset lại chỉ số của quái (Máu, Trạng thái) -> Quan trọng!
        // Lưu ý: Hàm OnEnable bên trong EnemyBaseFSM sẽ tự chạy khi SetActive(true)
        
        return objToSpawn;
    }

    // --- HÀM NHẬP KHO (RETURN) ---
    public void ReturnToPool(GameObject enemyObj, EnemyType type)
    {
        enemyObj.SetActive(false);
        poolDictionary[type].Enqueue(enemyObj);
    }
}