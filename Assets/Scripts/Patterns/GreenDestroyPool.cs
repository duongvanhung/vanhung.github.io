using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GreenDestroyPool : SingletonMono<GreenDestroyPool>
{
    private GreenDestroyPool() {}
    [SerializeField] private bool expanable = true;

    public List<GreenDestroyDiamond> poolObjects;
    // Start is called before the first frame update
    void Awake()
    {
        poolObjects = new List<GreenDestroyDiamond>();
    }
    
    public GreenDestroyDiamond GetObject (GreenDestroyDiamond prefab) 
    {
        foreach (var g in poolObjects)
        {
            if (!g.isActive)
            {
                g.Play();
                return g;
            }
        }

        if (expanable) 
        {
            GreenDestroyDiamond g = GenerateNewObject(prefab);
            
            g.Play();

            return g;
        }
        else return null;
    }

    private GreenDestroyDiamond GenerateNewObject (GreenDestroyDiamond prefab)
    {
        GreenDestroyDiamond gameObject = Instantiate(prefab);
        gameObject.transform.parent = transform;
        // gameObject.Play();
        poolObjects.Add(gameObject);
        
        return gameObject;
    }
}
