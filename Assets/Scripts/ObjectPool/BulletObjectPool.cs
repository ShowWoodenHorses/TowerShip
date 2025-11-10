using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    private static BulletObjectPool instance;
    public static BulletObjectPool Instance { get { return instance; } }

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializePool();
    }

    private void InitializePool()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                BulletContoller bulletController = obj.GetComponent<BulletContoller>();
                if (bulletController != null)
                {
                    bulletController.SetSettings();
                }
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectQueue);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool for prefab " + prefab.name + " not found!");
            return null;
        }

        if (poolDictionary[prefab].Count == 0)
        {
            // Расширяем пул, если необходимо
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            poolDictionary[prefab].Enqueue(newObj);
        }

        GameObject objToSpawn = poolDictionary[prefab].Dequeue();
        objToSpawn.SetActive(true);
        return objToSpawn;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        BulletContoller bulletController = obj.GetComponent<BulletContoller>();
        if (bulletController != null)
        {
            bulletController.SetSettings();
        }
        // Возвращаем объект в его соответствующий пул
        foreach (var pool in poolDictionary)
        {
            if (obj.name.Contains(pool.Key.name))
            {
                pool.Value.Enqueue(obj);
                break;
            }
        }
    }
}
