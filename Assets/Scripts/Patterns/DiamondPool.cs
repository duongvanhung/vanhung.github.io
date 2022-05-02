using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DiamondPool : SingletonMono<DiamondPool>
{
    private DiamondPool () {}
    [SerializeField] private bool expanable = true;

    public HashSet<GameObject> poolObjects;
    // Start is called before the first frame update
    void Awake()
    {
        poolObjects = new HashSet<GameObject>();
    }
    
    public GameObject GetObject (GameObject prefab) 
    {
        foreach (var g in poolObjects)
        {
            if (!g.activeSelf && g.tag == prefab.tag)
            {
                g.SetActive(true);
                return g;
            }
        }

        if (expanable) 
        {
            GameObject g = GenerateNewObject(prefab);
            g.SetActive(true);

            return g;
        }
        else return null;
    }

    public void RemoveObject (GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private GameObject GenerateNewObject (GameObject prefab)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.parent = transform;
        gameObject.SetActive(false);
        poolObjects.Add(gameObject);
        
        return gameObject;
    }
}
