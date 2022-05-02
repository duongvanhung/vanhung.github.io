using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IObserver
{
    [SerializeField] GameObject gameover;
    [SerializeField] GameObject dimOffBackGround;

    public void OnNotify(object key, object data)
    {
        if (typeof(GameStates) == key.GetType())
        {
            switch ((GameStates)key)
            {
                case GameStates.Gameover:
                    if (gameover) gameover.SetActive(true);
                    if (dimOffBackGround) dimOffBackGround.SetActive(true);
                    break;
                default:
                    Debug.LogWarning ("invalute game state");
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.RegisterObserver (GameStates.Gameover, this);

        if (!dimOffBackGround)
        {
            Debug.LogWarning("Misisng dim off back ground");
        }
        if (!gameover)
        {
            Debug.LogWarning("Misisng gameover panel");
        }
    }

    private void OnDestroy() {
        if (GameManager.IsEnable) GameManager.Instance.RemoveAllRegister (this);
    }
}
