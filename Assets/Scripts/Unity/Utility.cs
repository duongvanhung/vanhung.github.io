using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortDiamondFollowY : IComparer<Diamond>
{
    public int Compare(Diamond x, Diamond y)
    {
        return (int)(x.transform.position.y - y.transform.position.y);
    }
}