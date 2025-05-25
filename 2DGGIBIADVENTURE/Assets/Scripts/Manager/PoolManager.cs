using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public List<GameObject> prefabs;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
        InitializePools();
    }


    public void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject prefab = pool.prefabs[Random.Range(0, pool.prefabs.Count)];
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj) ;
            }

            poolDictionary.Add(pool.tag, objectPool) ;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag) || poolDictionary[tag].Count == 0)
        {
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();

        if (obj == null)
        {
            return null;
        }

        obj.SetActive(true); 
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }
    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(tag)) return;

        poolDictionary[tag].Enqueue(obj);
    }
}
