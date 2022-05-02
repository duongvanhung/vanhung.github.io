using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeController
{
    private static float localTimeScale = 1f;

    public static bool IsPaused
    {
        get
        {
            return localTimeScale == 0f;
        }
    }

    public static float TimeScale
    {
        get
        {
            return Time.timeScale * localTimeScale;
        }
        set
        {
            if (value == 0f) 
            {
                localTimeScale = 0f;
                return;
            }
            if (value == 1f)
            {
                localTimeScale = 1f;
                return;
            }
            if (value > 0f && value != 1f)
            {
                Time.timeScale = value;
                return;
            }
        }
    }
}
