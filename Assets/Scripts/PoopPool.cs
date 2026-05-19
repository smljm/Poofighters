using System.Collections.Generic;
using UnityEngine;

public class PoopPool : MonoBehaviour
{
    public static PoopPool Instance;

    public GameObject poopPrefab;
    public int poolSize = 30;

    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(poopPrefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position)
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(poopPrefab);
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}