using AC.Attribute;
using AC.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool : MonoBehaviour
{
    GameObject _prefab = null;
    Stack<GameObject> _listItemInPool = new Stack<GameObject>();
    //[SerializeField]
    //List<GameObject> _listItemInPool = new List<GameObject>();
    [SerializeField, ReadOnlly]
    List<GameObject> _listItemActived = new List<GameObject>();
    public bool IsCreateSuccessed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Cleanup
    }

    public void CreatePool(GameObject prefab, int count)
    {
        IsCreateSuccessed = false;
        _prefab = prefab;
        
        if (_prefab != null)
        {
            IsCreateSuccessed = true;
            for(int i=0; i< count; i++)
            {
                CreatItemInPool();
            }
            gameObject.name = _prefab.name;
            PoolManager.Instance.CheckLoadAllPool();
        }
        else
        {
            LogManager.LogError("[Pool] Prefab is null!");
        }
    }

    void CreatItemInPool()
    {
        GameObject go = Instantiate(_prefab, transform);
        go.SetActive(false);
        _listItemInPool.Push(go);
    }


    public GameObject GetObjectInPool(Transform parrent = null)
    {
        GameObject go = null;
        do
        {
            if (_listItemInPool.Count <= 0)
                CreatItemInPool();
            go = _listItemInPool.Pop();
        } while (go == null);        
        go.transform.parent = parrent;
        
        _listItemActived.Add(go);
        go.SetActive(true); // Bật đối tượng khi lấy ra
        return go;
    }

    public bool ReturnItemToPool(GameObject go)
    {
        if(go == null) return false;
        if(_listItemActived == null || _listItemActived.Count <= 0) return false;
        if(_listItemActived.Contains(go))
        {
            _listItemActived.Remove(go);
            _listItemInPool.Push(go);
            go.transform.SetParent(transform);
            go.SetActive(false);           
                       
            return true;
        }
        return false;
    }
}
