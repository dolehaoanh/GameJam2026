using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Core
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] List<PoolItem> _listPoolItem;
        Dictionary<string, Pool> _listPool = new Dictionary<string, Pool>();
        public CheckLoadCompleted CheckLoadAssetCompleted = new CheckLoadCompleted();

        public Action LoadAssetCompletedEvent;
        private void Start()
        {
            LoadAllPool();
        }

        public void LoadAllPool()
        {
            _listPool = new Dictionary<string, Pool>();
            for (int i=0; i< _listPoolItem.Count; i++)
            {
                PoolItem poolItem = _listPoolItem[i];
                CreatePool(poolItem.Prefab, poolItem.Count);
            }
        }
       
        void CreatePool(GameObject prefab, int count)
        {
            if (prefab == null) return;
            if (_listPool.ContainsKey(prefab.name)) return;
            GameObject newPool = new GameObject("New Pool", typeof(Pool));
            newPool.transform.SetParent(transform);
            Pool pool = newPool.GetComponent<Pool>();
            pool.CreatePool(prefab, count);
            _listPool.Add(prefab.name, pool);
        }
        public void CheckLoadAllPool()
        {
            if (CheckLoadAssetCompleted.IsLoadCompleted) return;
            foreach (Pool pool in _listPool.Values)
            {
                if (!pool.IsCreateSuccessed)
                {
                    return;
                }
            }
            CheckLoadAssetCompleted.IsLoadCompleted = true;
            LoadAssetCompletedEvent?.Invoke();
        }

        public GameObject Spawn(GameObject prefab, Transform parrent = null)
        {
            if(prefab == null)
            {
                LogManager.LogWarning("Prefab is Null");
                return null;
            }
            if(!_listPool.ContainsKey(prefab.name))
            {
                LogManager.LogWarning("Khong tim thay Pool chua Prefab");
                return null;
            }
            return _listPool[prefab.name].GetObjectInPool(parrent);
        }

        public void ReturnToPool(GameObject go)
        {
            foreach (Pool pool in _listPool.Values)
            {
                if(pool.ReturnItemToPool(go)) return;
            }
        }
    }

    [System.Serializable]
    public class PoolItem
    {
        public GameObject Prefab;
        public int Count;
    }
}

