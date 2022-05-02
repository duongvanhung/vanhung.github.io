using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Support : SingleSubject<Support>, IObserver
{
    private Support () {}
    [Tooltip("time count to support player")] [Min(0)]
    [SerializeField] float duration = 2f;

    float count = 0;
    bool isStart = false;
    bool isStop = false;
    Diamond[] supportCouple;

    private void Start() {
        GameManager.Instance.RegisterObserver(GameStates.OnWaitInput, this);
        GameManager.Instance.RegisterObserver (GameStates.Pause, this);
        GameManager.Instance.RegisterObserver (GameStates.Resume, this);
        GameManager.Instance.RegisterObserver (GameStates.OnInvertDiamond, this);
    }

    private void Update() {
        try
        {
            if (!isStart || isStop) return;

            count += Time.deltaTime;
            
            if (count < duration) return;
            
            if (GameManager.Instance.GameState != GameStates.OnWaitInput) return;

            count = 0;
            isStart = false;

            if (supportCouple == null) return;
            SendMessage (SupportEvent.SendSupport, supportCouple);
        }
        catch (System.Exception)
        {
            GameManager.Instance.SendMessage (GameStates.OnError);
        }
    }

    private void OnDestroy() {
        if (GameManager.IsEnable) GameManager.Instance.RemoveAllRegister (this);
    }

    public Diamond[] GetSupport (Diamond[,] diamonds)
    {
        if (diamonds == null) return null;
        
        Diamond[] result = new Diamond[2];
        
        for (int y = 0; y < Board.Instance.Height; y ++)
        {
            for (int x = 0; x < Board.Instance.Weight; x ++)
            {
                try
                {
                    if (diamonds[x, y] == null) {
                        Debug.LogWarning("missing diamond in board");
                        continue;
                    }

                    Vector2Int diamond2;
                    if (!Check(diamonds, new Vector2Int(x, y), out diamond2)) continue;
                    
                    result[0] = diamonds[x, y];
                    result[1] = diamonds[diamond2.x, diamond2.y];

                    return result;
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogWarning("Invalute diamonds array");
                    return null;
                }
            }
        }

        return null;
    }

    public bool Check (Diamond[,] diamonds, Vector2Int position, out Vector2Int resultPosition)
    {
        if (CheckDownLeftAndDownDownLeft(diamonds, position) 
        || CheckUpLeftAndUpUpLeft(diamonds, position)
        || CheckUpLeftAndDownLeft(diamonds, position)) 
        {
            resultPosition = CaculatePosition(position, PositionState.Left);
            return true;
        }
        if (CheckDownLeftAndDownLeftLeft(diamonds, position)
        || CheckDownRightAndDownRightRight(diamonds, position)
        || CheckDownLeftAndDownRight(diamonds, position)) 
        {
            resultPosition = CaculatePosition(position, PositionState.Down);
            return true;
        }
        if (CheckDownRightAndDownDownRight(diamonds, position)
        || CheckUpRightAndUpUpRight(diamonds, position)
        || CheckUpRightAndDownRight(diamonds, position)) 
        {
            resultPosition = CaculatePosition(position, PositionState.Right);
            return true;
        }
        if (CheckUpLeftAndUpRight(diamonds, position)
        || CheckUpLeftAndUpLeftLeft(diamonds, position)
        || CheckUpRightAndUpRightRight(diamonds, position)) 
        {
            resultPosition = CaculatePosition(position, PositionState.Up);
            return true;
        }
        
        resultPosition = new Vector2Int (-1, -1);
        return false;
    }

    private bool CheckUpLeftAndDownLeft(Diamond[,] diamonds, Vector2Int position)
    {
        // int xUpLeft = position.x - 1;
        // int yUpLeft = position.y + 1;
        // int xDownLeft = position.x - 1;
        // int yDownLeft = position.y - 1;
        Vector2Int upLeft = CaculatePosition(position, PositionState.UpLeft);
        Vector2Int downLeft = CaculatePosition(position, PositionState.DownLeft);

        // if (upLeft.x < 0 || upLeft.y >= Board.Instance.Height || downLeft.x < 0 || downLeft.y < 0) return false; 
        if (CheckOutOfRange(downLeft, PositionState.DownLeft) || CheckOutOfRange(upLeft, PositionState.UpLeft)) 
            return false;
        // if (diamonds[upLeft.x, upLeft.y] == null || diamonds[downLeft.x, downLeft.y] == null)
        // {
        //     Debug.LogWarning("missing diamond in board");
        //     return false;
        // }

        // if (diamonds[upLeft.x, upLeft.y].tag == diamonds[position.x, position.y].tag 
        // && diamonds[downLeft.x, downLeft.y].tag == diamonds[position.x, position.y].tag)
        // {
        //     return true;    
        // }
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upLeft.x, upLeft.y], diamonds[downLeft.x, downLeft.y]))
            return true;

        return false;
    }

    private bool CheckUpLeftAndUpRight(Diamond[,] diamonds, Vector2Int position)
    {
        // int xUpLeft = position.x - 1;
        // int yUpLeft = position.y + 1;
        // int xUpRight = position.x + 1;
        // int yUpRight = position.y + 1;
        Vector2Int upLeft = CaculatePosition(position, PositionState.UpLeft);
        Vector2Int upRight = CaculatePosition(position, PositionState.UpRight);

        // if (upLeft.x < 0 || upLeft.y >= Board.Instance.Height || upRight.x >= Board.Instance.Weight 
        // || upRight.y >= Board.Instance.Height) return false; 
        if (CheckOutOfRange(upLeft, PositionState.UpLeft) || CheckOutOfRange(upRight, PositionState.UpRight)) 
            return false;

        // if (diamonds[upLeft.x, upLeft.y] == null || diamonds[upRight.x, upRight.y] == null)
        // {
        //     Debug.LogWarning("missing diamond in board");
        //     return false;
        // }

        // if (diamonds[upLeft.x, upLeft.y].tag == diamonds[position.x, position.y].tag 
        // && diamonds[upRight.x, upRight.y].tag == diamonds[position.x, position.y].tag)
        // {
        //     return true;    
        // }
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upLeft.x, upLeft.y], diamonds[upRight.x, upRight.y]))
            return true;

        return false;
    }

    private bool CheckUpRightAndDownRight(Diamond[,] diamonds, Vector2Int position)
    {
        // int xDownRight = position.x + 1;
        // int yDownRight = position.y - 1;
        // int xUpRight = position.x + 1;
        // int yUpRight = position.y + 1;
        Vector2Int downRight = CaculatePosition (position, PositionState.DownRight);
        Vector2Int upRight = CaculatePosition (position, PositionState.UpRight);

        // if (downRight.x >= Board.Instance.Weight || downRight.y < 0
        // || upRight.x >= Board.Instance.Weight || upRight.y >= Board.Instance.Height) return false; 
        if (CheckOutOfRange(downRight, PositionState.DownRight) || CheckOutOfRange(upRight, PositionState.UpRight)) 
            return false;
        
        // if (diamonds[xDownRight, yDownRight] == null || diamonds[xUpRight, yUpRight] == null)
        // {
        //     Debug.LogWarning("missing diamond in board");
        //     return false;
        // }

        // if (diamonds[xDownRight, yDownRight].tag == diamonds[position.x, position.y].tag 
        // && diamonds[xUpRight, yUpRight].tag == diamonds[position.x, position.y].tag)
        // {
        //     return true;    
        // }
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downRight.x, downRight.y], diamonds[upRight.x, upRight.y]))
            return true;

        return false;
    }

    private bool CheckDownLeftAndDownRight(Diamond[,] diamonds, Vector2Int position)
    {
        // int xDownRight = position.x + 1;
        // int yDownRight = position.y - 1;
        // int xDownLeft = position.x - 1;
        // int yDownLeft = position.y - 1;        
        Vector2Int downRight = CaculatePosition (position, PositionState.DownRight);
        Vector2Int downLeft = CaculatePosition (position, PositionState.DownLeft);

        // if (downRight.x >= Board.Instance.Weight || downRight.y < 0
        // || downLeft.x >= Board.Instance.Weight || downLeft.y >= Board.Instance.Height) return false; 
        if (CheckOutOfRange(downLeft, PositionState.DownLeft) || CheckOutOfRange(downRight, PositionState.DownRight)) 
            return false;
        // if (diamonds[xDownRight, yDownRight] == null || diamonds[xDownLeft, yDownLeft] == null)
        // {
        //     Debug.LogWarning("missing diamond in board");
        //     return false;
        // }

        // if (diamonds[xDownRight, yDownRight].tag == diamonds[position.x, position.y].tag 
        // && diamonds[xDownLeft, yDownLeft].tag == diamonds[position.x, position.y].tag)
        // {
        //     return true;    
        // }
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downRight.x, downRight.y], diamonds[downLeft.x, downLeft.y]))
            return true;

        return false;
    }

    private bool CheckUpLeftAndUpLeftLeft (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int upLeft = CaculatePosition (position, PositionState.UpLeft);
        Vector2Int upLeftLeft = CaculatePosition (position, PositionState.UpLeftLeft);

        if (CheckOutOfRange(upLeft, PositionState.UpLeft) || CheckOutOfRange(upLeftLeft, PositionState.UpLeftLeft)) return false;

        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upLeft.x, upLeft.y], diamonds[upLeftLeft.x, upLeftLeft.y]))
            return true;

        return false;
    }

    private bool CheckDownLeftAndDownLeftLeft (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int downLeft = CaculatePosition (position, PositionState.DownLeft);
        Vector2Int downLeftLeft = CaculatePosition (position, PositionState.DownLeftleft);

        if (CheckOutOfRange(downLeft, PositionState.DownLeft) || CheckOutOfRange(downLeftLeft, PositionState.DownLeftleft)) 
            return false;

        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downLeft.x, downLeft.y], diamonds[downLeftLeft.x, downLeftLeft.y]))
            return true;

        return false;
    }

    private bool CheckUpRightAndUpRightRight (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int upRight = CaculatePosition (position, PositionState.UpRight);
        Vector2Int upRightRight = CaculatePosition (position, PositionState.UpRightRight);

        if (CheckOutOfRange(upRight, PositionState.UpRight) || CheckOutOfRange(upRightRight, PositionState.UpRightRight)) 
            return false;
        
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upRight.x, upRight.y], diamonds[upRightRight.x, upRightRight.y]))
            return true;

        return false;
    }

    private bool CheckDownRightAndDownRightRight (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int downRight = CaculatePosition (position, PositionState.DownRight);
        Vector2Int downRightRight = CaculatePosition (position, PositionState.DownRightRight);
    
        if (CheckOutOfRange(downRight, PositionState.DownRight) || CheckOutOfRange(downRightRight, PositionState.DownDownRight)) 
            return false;
        
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downRight.x, downRight.y], diamonds[downRightRight.x, downRightRight.y]))
            return true;

        return false;
    }

    private bool CheckUpLeftAndUpUpLeft (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int upLeft = CaculatePosition (position, PositionState.UpLeft);
        Vector2Int upUpLeft = CaculatePosition (position, PositionState.UpUpLeft);
    
        if (CheckOutOfRange(upLeft, PositionState.UpLeft) || CheckOutOfRange(upUpLeft, PositionState.UpUpLeft)) 
            return false;
    
        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upLeft.x, upLeft.y], diamonds[upUpLeft.x, upUpLeft.y]))
            return true;

        return false;
    }

    private bool CheckDownLeftAndDownDownLeft (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int downLeft = CaculatePosition (position, PositionState.DownLeft);
        Vector2Int downDownLeft = CaculatePosition (position, PositionState.DownDownLeft);
    
        if (CheckOutOfRange(downLeft, PositionState.DownLeft) || CheckOutOfRange(downDownLeft, PositionState.DownDownLeft)) 
            return false;

        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downLeft.x, downLeft.y], diamonds[downDownLeft.x, downDownLeft.y]))
            return true;

        return false;
    }

    private bool CheckUpRightAndUpUpRight (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int upRight = CaculatePosition (position, PositionState.UpRight);
        Vector2Int upUpRight = CaculatePosition (position, PositionState.UpUpRight);
    
        if (CheckOutOfRange(upRight, PositionState.UpRight) || CheckOutOfRange(upUpRight, PositionState.UpUpRight)) 
            return false;

        if (CanBlowUp(diamonds[position.x, position.y], diamonds[upRight.x, upRight.y], diamonds[upUpRight.x, upUpRight.y]))
            return true;

        return false;
    }

    private bool CheckDownRightAndDownDownRight (Diamond[,] diamonds, Vector2Int position)
    {
        Vector2Int downRight = CaculatePosition (position, PositionState.DownRight);
        Vector2Int downDownRight = CaculatePosition (position, PositionState.DownDownRight);
    
        if (CheckOutOfRange(downRight, PositionState.DownRight) || CheckOutOfRange(downDownRight, PositionState.DownDownRight)) 
            return false;

        if (CanBlowUp(diamonds[position.x, position.y], diamonds[downRight.x, downRight.y], diamonds[downDownRight.x, downDownRight.y]))
            return true;

        return false;
    }

    private bool CheckOutOfRange (Vector2Int position, PositionState state)
    {
        switch (state)
        {
            case PositionState.UpLeft:
                return (position.x < 0 || position.y >= Board.Instance.Height);
            case PositionState.DownLeft:
                return (position.x < 0 || position.y < 0);
            case PositionState.UpRight:
                return (position.x >= Board.Instance.Weight || position.y >= Board.Instance.Height);
            case PositionState.DownRight:
                return (position.x >= Board.Instance.Weight || position.y < 0);
            case PositionState.UpLeftLeft:
                return (position.x < 0 || position.y >= Board.Instance.Height);
            case PositionState.DownLeftleft:
                return (position.x < 0 || position.y < 0);
            case PositionState.UpRightRight:
                return (position.x >= Board.Instance.Weight || position.y >= Board.Instance.Height);
            case PositionState.DownRightRight:
                return (position.x >= Board.Instance.Weight || position.y < 0);
            case PositionState.UpUpLeft:
                return (position.x < 0 || position.y >= Board.Instance.Height);
            case PositionState.DownDownLeft:
                return (position.x < 0 || position.y < 0);
            case PositionState.UpUpRight:
                return (position.x >= Board.Instance.Weight || position.y >= Board.Instance.Height);
            case PositionState.DownDownRight:
                return (position.x >= Board.Instance.Weight || position.y < 0);
            default:
                Debug.LogWarning("invalute state position");
                return false;
        }
    }

    private Vector2Int CaculatePosition (Vector2Int position, PositionState state)
    {
        switch (state)
        {
            case PositionState.Left:
                return new Vector2Int (position.x - 1, position.y);
            case PositionState.Right:
                return new Vector2Int (position.x + 1, position.y);
            case PositionState.Up:
                return new Vector2Int (position.x, position.y + 1);
            case PositionState.Down:
                return new Vector2Int (position.x, position.y - 1);
            case PositionState.UpLeft:
                return new Vector2Int (position.x - 1, position.y + 1);
            case PositionState.DownLeft:
                return new Vector2Int (position.x - 1, position.y - 1);
            case PositionState.UpRight:
                return new Vector2Int (position.x + 1, position.y + 1);
            case PositionState.DownRight:
                return new Vector2Int (position.x + 1, position.y - 1);
            case PositionState.UpLeftLeft:
                return new Vector2Int (position.x - 2, position.y + 1);
            case PositionState.DownLeftleft:
                return new Vector2Int (position.x - 2, position.y - 1);
            case PositionState.UpRightRight:
                return new Vector2Int (position.x + 2, position.y + 1);
            case PositionState.DownRightRight:
                return new Vector2Int (position.x + 2, position.y - 1);
            case PositionState.UpUpLeft:
                return new Vector2Int (position.x - 1, position.y + 2);
            case PositionState.DownDownLeft:
                return new Vector2Int (position.x - 1, position.y - 2);
            case PositionState.UpUpRight:
                return new Vector2Int (position.x + 1, position.y + 2);
            case PositionState.DownDownRight:
                return new Vector2Int (position.x + 1, position.y - 2);
            default:
                Debug.LogWarning("invalute state position");
                return position;
        }
        
    }

    private bool CanBlowUp (Diamond origin, Diamond diamondCheck1, Diamond diamondCheck2)
    {
        if (!origin || !diamondCheck1 || !diamondCheck2)
        {
            Debug.LogWarning("missing diamond in board");
            return false;
        }

        if (origin.tag == diamondCheck1.tag && origin.tag == diamondCheck2.tag) return true;

        return false;
    }

    private void HandleOnWaitInput ()
    {
        supportCouple = GetSupport(Board.Instance.Diamonds);
        if (supportCouple == null)
        {
            SendMessage (SupportEvent.NoExistSupport);
        }
        else
        {
            isStart = true;
            Debug.Log("There is brow up at: " + supportCouple[0].transform.position + " " 
            + supportCouple[1].transform.position);
        }
    }

    private void HandleOnPause ()
    {
        isStop = true;
    }

    private void HandleOnResume ()
    {
        isStop = false;
    }

    private void HandleOnInvertDiamond ()
    {
        supportCouple = null;
        count = 0;
    }

    public void OnNotify(object key, object data)
    {
        if (key.GetType() == typeof(GameStates))
        {
            switch ((GameStates)key)
            {
                case GameStates.OnWaitInput:
                    HandleOnWaitInput();
                    break;
                case GameStates.Pause:
                    HandleOnPause();
                    break;
                case GameStates.Resume:
                    HandleOnResume();
                    break;
                case GameStates.OnInvertDiamond:
                    HandleOnInvertDiamond();
                    break;
                default:
                    Debug.LogWarning ("invalute game state");
                    break;
            }
        }
    }

    private enum PositionState
    {
        Left,
        Right,
        Up,
        Down,
        UpLeft,
        DownLeft,
        UpRight,
        DownRight,
        UpLeftLeft,
        DownLeftleft,
        UpRightRight,
        DownRightRight,
        UpUpLeft,
        DownDownLeft,
        UpUpRight,
        DownDownRight
    }
}

public enum SupportEvent
{
    SendSupport,
    NoExistSupport
}