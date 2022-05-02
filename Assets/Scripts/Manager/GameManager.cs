using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : SingleSubject<GameManager>, IObserver
{
    private GameManager () {}
    GameStates gameStateBefore;
    private GameStates gameState = GameStates.DownDiamond;
    public GameStates GameState
    {
        get { return gameState; }
        private set {
            if (value != gameState) gameStateBefore = gameState; 
            gameState = value;    
        }
    }

    private int loadSceneId = 0;

    // Start is called before the first frame update
    void Start()
    {
        Board.Instance.RegisterObserver(BoardEvent.DownDiamond, this);
        Board.Instance.RegisterObserver(BoardEvent.AllClear, this);
        Board.Instance.RegisterObserver (BoardEvent.Invert, this);

        Support.Instance.RegisterObserver (SupportEvent.NoExistSupport, this);
    }

    private void OnDestroy() 
    {
        if (Board.IsEnable) Board.Instance.RemoveAllRegister(this);
        if (Support.IsEnable) Support.Instance.RemoveAllRegister(this);
    }

    private void HandleStartInvert ()
    {
        GameState = GameStates.OnInvertDiamond;

        SendMessage (GameStates.OnInvertDiamond);
    }

    private void HandleEndDown ()
    {
        GameState = GameStates.DownDiamond;

        SendMessage (GameStates.DownDiamond);
    }

    private void HandleClear ()
    {
        GameState = GameStates.OnWaitInput;
        SendMessage (GameStates.OnWaitInput);
    }

    private void HandleNoExistSupport ()
    {
        GameState = GameStates.Gameover;
        
        SendMessage (GameStates.Gameover);
    }

    public void ResetGame ()
    {
        SendMessage (GameStates.Restart);
        TimeController.TimeScale = 1f;
        GameState = GameStates.OnWaitInput;
        
        SceneManager.LoadScene(loadSceneId);
    }

    public void PauseGame ()
    {
        TimeController.TimeScale = 0;
        
        GameState = GameStates.Pause;

        SendMessage (GameStates.Pause);
    }

    public void ResumeGame ()
    {
        TimeController.TimeScale = 1;
        GameState = gameStateBefore;

        SendMessage (GameStates.Resume);
    }
    
    public void OnNotify(object key, object data)
    {
        if (typeof(BoardEvent) == key.GetType())
        {
            switch ((BoardEvent)key)
            {
                case BoardEvent.Invert:
                    HandleStartInvert ();
                    break;
                case BoardEvent.DownDiamond:
                    HandleEndDown ();
                    break;
                case BoardEvent.AllClear:
                    HandleClear ();
                    break;
                default:
                    Debug.LogWarning ("invalute board event");
                    break;
            }
        }
        else if (typeof(SupportEvent) == key.GetType())
        {
            switch ((SupportEvent)key)
            {
                case SupportEvent.NoExistSupport:
                    HandleNoExistSupport();
                    break;
                default:
                    Debug.LogWarning ("invalute support event");
                    break;
            }
        }
    }
}

public enum GameStates
{
    OnWaitInput,
    OnInvertDiamond,
    DownDiamond,
    Pause,
    Resume,
    Restart,
    Gameover,
    OnError
}
