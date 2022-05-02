using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Diamond : MonoBehaviour
{
    public ConcreteDiamond Instance;

    private void Awake() {
        Instance = DiamondFlyweightFactory.GetDiamond();
    }

    public void Pause ()
    {
        DOTween.Pause(this);
    }
}
