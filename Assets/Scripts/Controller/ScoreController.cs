using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour, IObserver
{
    [SerializeField] Text scoreUI; 
    [SerializeField] Text highScoreUI;
    [SerializeField] int score = 0;
    [SerializeField] int minNumberAmount = 5;

    const int scorePerDiamond = 5;

    private void Start() {
        Board.Instance.RegisterObserver(BoardEvent.EarnDiamonds, this);

        GameManager.Instance.RegisterObserver (GameStates.Gameover, this);
        GameManager.Instance.RegisterObserver (GameStates.Restart, this);
        GameManager.Instance.RegisterObserver (GameStates.OnError, this);

        if (!scoreUI)
        {
            Debug.LogWarning("Missing score UI text");
        }
        if (!highScoreUI)
        {
            Debug.LogWarning ("Missing high score UI text");
        }
        else
        {
            highScoreUI.text = PadZero (DataLoad.Instance.HighScore, minNumberAmount);
        }
    }

    private void OnDestroy() {
        SaveDataScore ();
        
        if (Board.IsEnable) Board.Instance.RemoveAllRegister(this);
        if (GameManager.IsEnable) GameManager.Instance.RemoveAllRegister (this);
    }

    public void SaveDataScore ()
    {
        if (score > DataLoad.Instance.HighScore) 
        {
            DataLoad.Instance.SaveHighScore (score);

            if (highScoreUI) highScoreUI.text = PadZero (DataLoad.Instance.HighScore, minNumberAmount);
        }
    }

    private void UpdateUIScore ()
    {
        if (!scoreUI) return;

        scoreUI.text = PadZero(score, minNumberAmount);
    }

    string PadZero(int score,int padDigits)
	{
		string result = score.ToString();

		while (result.Length < padDigits)
		{
			result = "0" + result;
		}
		return result;
	}

    private void GetScore (object data)
    {
        if (data == null)
        {
            Debug.LogWarning ("There are no position of diamond earned");
            return;
        }

        try 
        {
            HashSet<Vector2Int> positions = (HashSet<Vector2Int>)data;
            int diamondAmount = positions.Count;
            int newScorePerDiamond = scorePerDiamond + diamondAmount;
            score += newScorePerDiamond * diamondAmount;
            UpdateUIScore();
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
        
        return;
    }

    public void OnNotify(object key, object data)
    {
        if (key.GetType() == typeof(BoardEvent))
        {
            switch ((BoardEvent)key)
            {
                case BoardEvent.EarnDiamonds:
                    GetScore(data);
                    break;
                default:
                    Debug.LogWarning("invalute board event");
                    break;
            }
        }
        if (key.GetType() == typeof(GameStates))
        {
            switch ((GameStates)key)
            {
                case GameStates.Restart:
                    SaveDataScore();
                    break;
                case GameStates.Gameover:
                    SaveDataScore ();
                    break;
                case GameStates.OnError:
                    SaveDataScore ();
                    break;
                default:
                    Debug.LogWarning("invalute game state");
                    break;
            }
        }
    }
}
