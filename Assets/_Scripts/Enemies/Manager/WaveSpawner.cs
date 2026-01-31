using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int zombieCount;
        public int skeletonCount;
        public int chargerCount;
        public float timeBetweenSpawns = 1f;
    }

    public Transform[] spawnPoints; // Kéo các GameObj vị trí vào đây
    public Wave[] waves; // Cấu hình các đợt tấn công
    public float timeBetweenWaves = 5f;

    private int currentWaveIndex = 0;

    void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        // Duyệt qua từng Wave
        while (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log($"Bắt đầu đợt: {currentWave.waveName}");

            // 1. Spawn Zombie
            for (int i = 0; i < currentWave.zombieCount; i++)
            {
                SpawnEnemy(EnemyType.Zombie);
                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }

            // 2. Spawn Skeleton
            for (int i = 0; i < currentWave.skeletonCount; i++)
            {
                SpawnEnemy(EnemyType.Skeleton);
                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }

            // 3. Spawn Charger
            for (int i = 0; i < currentWave.chargerCount; i++)
            {
                SpawnEnemy(EnemyType.Charger);
                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }

            Debug.Log("Hết đợt! Nghỉ ngơi...");
            yield return new WaitForSeconds(timeBetweenWaves);
            
            currentWaveIndex++;
        }
        
        Debug.Log("CHIẾN THẮNG! (Hoặc lặp lại vô tận tùy logic)");
    }

    void SpawnEnemy(EnemyType type)
    {
        // Chọn ngẫu nhiên 1 điểm spawn
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // GỌI SINGLETON POOL MANAGER
        EnemyPoolManager.Instance.SpawnEnemy(type, spawnPoint.position);
    }
}