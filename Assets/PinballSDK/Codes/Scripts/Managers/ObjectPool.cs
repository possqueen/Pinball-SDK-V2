using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool PoolManager;
    [System.Serializable] public class Pool
    {
        public string Name;
        public GameObject Prefab;
        public int Amount;
        public Transform Container;
    }
    public List<Pool> ObjectPools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> PoolDictionary = new Dictionary<string, Queue<GameObject>>();
    // Start is called before the first frame update

    private void Awake()
    {
        PoolManager = this;
    }
    void Start()
    {
        foreach (Pool p in ObjectPools)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < p.Amount; i++)
            {
                GameObject obj = Instantiate(p.Prefab, p.Container);
                obj.name = p.Prefab.name + " " + i;
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            PoolDictionary.Add(p.Name, pool);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject SpawnFromPool (string Name, Vector3 Position, Quaternion Rotation)
    {
        if (!PoolDictionary.ContainsKey(Name))
        {
            Debug.LogWarning("Object Pool \"" + Name + "\" does not exist.");
            return null;
        }

        GameObject newObj = PoolDictionary[Name].Dequeue();
        newObj.SetActive(true);
        newObj.transform.position = Position;
        newObj.transform.rotation = Rotation;
        PoolDictionary[Name].Enqueue(newObj);
        return newObj;
    }
}
