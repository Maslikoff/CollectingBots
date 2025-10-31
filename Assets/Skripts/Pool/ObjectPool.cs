using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private int _initialSize = 10;
    [SerializeField] protected Transform _container;
    [SerializeField] private T _prefab;

    private Queue<T> _pool = new Queue<T>();
    private HashSet<T> _activeObjects = new HashSet<T>();

    public IEnumerable<T> ActiveObjects => _activeObjects;

    protected virtual void Awake()
    {
        InitializePool();
    }

    public T GetObject()
    {
        T obj;

        if (_pool.Count == 0)
            obj = CreateNewObject();
        else
            obj = _pool.Dequeue();

        _activeObjects.Add(obj);
        obj.gameObject.SetActive(true);
        OnObjectGet(obj);

        return obj;
    }

    public void ReturnObject(T obj)
    {
        if (obj == null)
            return;

        _activeObjects.Remove(obj);
        _pool.Enqueue(obj);

        obj.gameObject.SetActive(false);
        ResetObject(obj);

        OnObjectReturn(obj);
    }

    public void ResetPool()
    {
        foreach (var obj in _activeObjects)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
                ResetObject(obj);
            }
        }

        _activeObjects.Clear();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialSize; i++)
            CreateNewObject();
    }

    private T CreateNewObject()
    {
        T newObject = _container != null ?
            Instantiate(_prefab, _container) :
            Instantiate(_prefab);

        newObject.gameObject.SetActive(false);
        _pool.Enqueue(newObject);
        InitializeObject(newObject);

        return newObject;
    }

    protected virtual void InitializeObject(T obj) { }
    protected virtual void ResetObject(T obj)
    {
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
    }
    protected virtual void OnObjectGet(T obj) { }
    protected virtual void OnObjectReturn(T obj) { }
}