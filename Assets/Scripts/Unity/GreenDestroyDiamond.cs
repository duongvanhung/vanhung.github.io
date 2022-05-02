using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDestroyDiamond : MonoBehaviour
{
    [SerializeField] Animator greenDesotry;

    public bool isActive => gameObject.activeSelf;

    private void Start() {
        if (!greenDesotry)
        {
            if (!TryGetComponent<Animator>(out greenDesotry))
            {
                Debug.LogWarning("Missing animator in green destroy effect");
            }
        }
    }

    public void Play() 
    {
        gameObject.SetActive (true);

        if (greenDesotry) 
        {
            greenDesotry.SetBool("isStart", true);
        }
    }

    public void ReturnPool ()
    {
        if (greenDesotry) greenDesotry.SetBool("isStart", false);
        gameObject.SetActive(false);
    }
}
