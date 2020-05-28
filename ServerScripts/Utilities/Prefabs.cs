using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public GameObject Item;
    public GameObject Player;
    public GameObject planeObject;

    public GameObject[] Effects;

    public T InstantiatePrefab<T>(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab);
        return temp.GetComponent<T>();
    }

    public T InstantiateEffect<T>(int effectIndex)
    {
        GameObject temp = Instantiate(Effects[effectIndex]);
        return temp.GetComponent<T>();
    }
}
