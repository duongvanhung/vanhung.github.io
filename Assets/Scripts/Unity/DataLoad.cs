using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DataLoad
{

    private DataLoad () {}
    static readonly DataLoad instance = new DataLoad();
    public static DataLoad Instance => instance;

    public int HighScore
    {
        get {
            return PlayerPrefs.GetInt(keyHighScore, 0);
        }
    }

    private const string keyHighScore = "score";

    public void SaveHighScore (int highScore)
    {
        PlayerPrefs.SetInt (keyHighScore, highScore);
    }
}
