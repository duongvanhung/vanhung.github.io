using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public sealed class Board : SingleSubject<Board>, IObserver
{
    private Board () {}
    [SerializeField] private Transform emptySquare;
    [SerializeField] private AnimationCurve animationInvert;
    [SerializeField] private AnimationCurve animationDown;

    private const int height = 8;
    private const int weight = 8;
    private Transform[,] grids;
    private Diamond[,] diamonds;
    private HashSet<int> checkColumnDown = new HashSet<int>();
    HashSet<Vector2Int>  destroyDiamonds = new HashSet<Vector2Int>();
    private bool isHandleDown = false;
    
    public int Height { get { return height; } }
    public int Weight { get { return weight; } }
    public Diamond[,] Diamonds => diamonds;

    private void Awake() {
        grids = new Transform[weight, height];
        diamonds = new Diamond[weight, height];    
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!emptySquare)
        {
            Debug.LogWarning("Missing empty square sprite");

            return;
        }

        GameManager.Instance.RegisterObserver (GameStates.DownDiamond, this);
        InputSystem.Instance.RegisterObserver (InputEvent.OnInvert, this);

        DrawGrid();
        grids[0,0].GetComponent<Sprite>();
    }

    private void OnDestroy() {
        if (GameManager.IsEnable) GameManager.Instance.RemoveAllRegister (this);
        if (InputSystem.IsEnable) InputSystem.Instance.RemoveAllRegister (this);
    }


    public void CheckAll ()
    {
        for (int y = 0; y < height; y ++)
        {
            for (int x = 0; x < weight; x ++)
            {
                List<Vector2Int> gathers = CheckGather(new Vector2Int(x, y));

                if (gathers.Count == 0) 
                {
                    continue;
                }
                
                foreach (var gather in gathers)
                {
                   
                    checkColumnDown.Add(gather.x);
                    destroyDiamonds.Add(gather);
                }
            }
        }
        
        Destroy();
        SendMessage (BoardEvent.DownDiamond);
    }
    
    public bool Check (Vector2Int position)
    {

        isHandleDown = false;

        if (diamonds[position.x, position.y] == null) return false;

        List<Vector2Int> gathers = CheckGather(position);

        if (gathers.Count == 0) return false;

        foreach (Vector2Int gather in gathers)
        {
            destroyDiamonds.Add(gather);
            checkColumnDown.Add(gather.x);
        }
        
        return true;
    }    

    private void Destroy ()
    {
        SendMessage (BoardEvent.EarnDiamonds, destroyDiamonds);
        foreach (var element in destroyDiamonds)
        {
            DestroyDiamond(element);
        }

        destroyDiamonds.Clear();
    }

    public void InvertTwoDiamond (Diamond diamond1, Diamond diamond2)
    {
        Vector2 posDiamond1 = new Vector2(diamond1.transform.position.x, diamond1.transform.position.y);
        Vector2 posDiamond2 = new Vector2(diamond2.transform.position.x, diamond2.transform.position.y);
        Vector2 check = new Vector2 (Mathf.Abs(posDiamond2.x - posDiamond1.x), Mathf.Abs(posDiamond2.y - posDiamond1.y));

        if (check != Vector2.up && check != Vector2.down && check != Vector2.left && check != Vector2.right)
        {
            return;
        }
        SendMessage (BoardEvent.Invert);
        
        diamonds[(int)posDiamond1.x, (int)posDiamond1.y] = diamond2;
        diamonds[(int)posDiamond2.x, (int)posDiamond2.y] = diamond1;

        diamond1.Instance.Move(diamond1, posDiamond2, null, animationInvert);
        diamond2.Instance.Move(diamond2, posDiamond1, () => {
            var check1 = Check (new Vector2Int((int)posDiamond1.x, (int)posDiamond1.y));
            var check2 = Check (new Vector2Int((int)posDiamond2.x, (int)posDiamond2.y));
            if (!check1 && !check2)
            {
                ReInvert(diamond1, diamond2, () => {
                    SendMessage (BoardEvent.AllClear);
                });
            }
            else
            {
                Destroy();
                SendMessage (BoardEvent.DownDiamond);
            }
        }, animationInvert);
    }

    public void LoadDiamond (Diamond diamond, int x, int y)
    {
        diamonds[x, y] = diamond;
    }

    void DrawGrid ()
    {
        for (int y = 0; y < height; y ++)
        {
            for (int x = 0; x < weight; x ++)
            {
                grids[x, y] = Instantiate(emptySquare, new Vector2(x, y), Quaternion.identity) as Transform;
                grids[x, y].name = "Board " + x.ToString() + "-" + y.ToString();
                grids[x, y].transform.parent = transform;
            }
        }
    }

    private void ReInvert (Diamond diamond1, Diamond diamond2, TweenCallback onEnd)
    {
        Vector2 posDiamond1 = new Vector2(diamond1.transform.position.x, diamond1.transform.position.y);
        Vector2 posDiamond2 = new Vector2(diamond2.transform.position.x, diamond2.transform.position.y);
        Vector2 check = new Vector2 (Mathf.Abs(posDiamond2.x - posDiamond1.x), Mathf.Abs(posDiamond2.y - posDiamond1.y));

        if (check != Vector2.up && check != Vector2.down && check != Vector2.left && check != Vector2.right)
            return;
        
        diamonds[(int)posDiamond1.x, (int)posDiamond1.y] = diamond2;
        diamonds[(int)posDiamond2.x, (int)posDiamond2.y] = diamond1;

        diamond1.Instance.Move(diamond1, posDiamond2, null, animationInvert);
        diamond2.Instance.Move(diamond2, posDiamond1, onEnd, animationInvert);
    }

    private List<Vector2Int> CheckGather (Vector2Int position)
    {
        if (position.x < 0 || position.x >= weight || position.y < 0 || position.y >= height)
        {
            throw new System.IndexOutOfRangeException("invalute position");
        }

        Diamond diamond = diamonds[position.x, position.y];
        List<Vector2Int> result = new List<Vector2Int>();
        List<Vector2Int> gathersHor = new List<Vector2Int>();
        List<Vector2Int> gathersVer = new List<Vector2Int>();

        result.Add(position);

        //Checked horizontal
        if (position.x < weight - 1)
        {
            for (int i = position.x + 1; i < weight; i ++)
            {
                if (diamonds[i, position.y] == null) break;
                if (diamonds[i, position.y].tag != diamond.tag)
                {
                    break;
                }

                gathersHor.Add(new Vector2Int(i, position.y));
            }
        }
        if (position.x > 0)
        {
            for (int i = position.x - 1; i >= 0; i --)
            {
                if (diamonds[i, position.y] == null) break;
                if (diamonds[i, position.y].tag != diamond.tag)
                {
                    break;
                }

                gathersHor.Add(new Vector2Int(i, position.y));
            }
        }
        if (gathersHor.Count >= 2)
        {
            result.AddRange(gathersHor);
        }

        //Checked vertical
        if (position.y < height - 1)
        {
            for (int i = position.y + 1; i < height; i ++)
            {
                if (diamonds[position.x, i] == null) break;
                if (diamonds[position.x, i].tag != diamond.tag)
                {
                    break;
                }

                gathersVer.Add(new Vector2Int(position.x, i));
            }
        }
        if (position.y > 0)
        {
            for (int i = position.y - 1; i >= 0; i --)
            {
                if (diamonds[position.x, i] == null) break;
                if (diamonds[position.x, i].tag != diamond.tag)
                {
                    break;
                }

                gathersVer.Add(new Vector2Int(position.x, i));
            }
        }
        if (gathersVer.Count >= 2)  result.AddRange(gathersVer);
        
        if (result.Count == 1) result.Clear();
        
        return result;
    }

    private void DownOneColumnDiamond (int column)
    {
        if (column < 0 || column >= weight)
        {
            throw new System.IndexOutOfRangeException("invalute column");
        }

        var columnDiamonds = new List<Diamond>();
        int countNull = 0;

        for (int y = 0; y < height; y ++)
        {
            if (diamonds[column, y] == null) 
            {
                countNull ++;
                continue;
            }
            
            columnDiamonds.Add(diamonds[column, y]);
        }

        List<Diamond> addDiamonds = Spawner.Instance.GenerateColumnDiamonds(column, countNull);
        columnDiamonds.AddRange(addDiamonds);

        columnDiamonds.Sort(new SortDiamondFollowY());
        foreach (var diamond in columnDiamonds)
        {
            DownDiamond(diamond);
        }
    }

    IEnumerator HandleEndOfDown ()
    {
        yield return new WaitForEndOfFrame();
        if (!isHandleDown)
        {
            Destroy();
            HandleOnDown();
            isHandleDown = true;
        }
    }

    private void DownDiamond (Diamond diamond)
    {
        Vector2Int position = new Vector2Int((int)diamond.transform.position.x, (int)diamond.transform.position.y);

        if (position.x < 0 || position.x >= weight || position.y < 0)
        {
            throw new System.IndexOutOfRangeException("invalute position");
        }

        if (position.y == 0) return;

        position.y = position.y >= height ? height: position.y;
        int countRow = 0;

        for (int i = position.y - 1; i >= 0; i --)
        {
            if (diamonds[position.x, i] == null)
            {
                countRow ++;
            }
            else
            {
                break;
            }
        }

        if (countRow == 0) return;

        var targetPosition = new Vector2Int(position.x, position.y - countRow);
        
        MoveDiamond (diamond, targetPosition, () => {
            Check(targetPosition);
            StartCoroutine(HandleEndOfDown());
        });
    }

    private void MoveDiamond (Diamond diamond, Vector2Int position, TweenCallback onEnd)
    {
        if (position.x < 0 || position.x >= weight || position.y < 0 || position.y >= height)
        {
            throw new System.IndexOutOfRangeException("invalute position");
        }

        if (diamond == null) return;

        Vector2Int diamondPos = new Vector2Int((int)diamond.transform.position.x, (int)diamond.transform.position.y);
        
        if (diamondPos.y < height) diamonds[diamondPos.x, diamondPos.y] = null;
        diamonds[position.x, position.y] = diamond;
        
        diamond.Instance.Move(diamond, position, onEnd, animationDown);
    }

    private void DestroyDiamond (Vector2Int position)
    {
        if (position.x < 0 || position.x >= weight || position.y < 0 || position.y >= height)
        {
            throw new System.IndexOutOfRangeException("invalute position");
        }
        if (!diamonds[position.x, position.y]) return;

        DiamondPool.Instance.RemoveObject(diamonds[position.x, position.y].gameObject);
        diamonds[position.x, position.y] = null;
    }

    private void HandleOnDown ()
    {
        if (checkColumnDown.Count == 0) 
        {
            SendMessage(BoardEvent.AllClear);
            return;
        }

        foreach (var elemnet in checkColumnDown)
        {
            DownOneColumnDiamond(elemnet);
        }

        checkColumnDown.Clear();
    }

    private void HandleOnInvert (object data)
    {
        if (data == null)
        {
            Debug.LogWarning("there are no couple diamond choosen");
            return;
        }

        try
        {
            List<Diamond> coupleChoose = (List<Diamond>)data;
            if (coupleChoose.Count != 2)
            {
                Debug.LogWarning("invalute couple diamond choosen");
                return;
            }
            InvertTwoDiamond (coupleChoose[0], coupleChoose[1]);
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    public void OnNotify(object key, object data)
    {
        if (key.GetType() == typeof(GameStates))
        {
            switch ((GameStates)key)
            {
                case GameStates.DownDiamond:
                    HandleOnDown ();
                    break;
                default:
                    Debug.LogWarning ("invalute game state");
                    break;
            }
        }
        if (key.GetType() == typeof(InputEvent))
        {
            switch ((InputEvent)key)
            {
                case InputEvent.OnInvert:
                    HandleOnInvert (data);
                    break;
                default:
                    Debug.LogWarning ("invalute input event");
                    break;
            }
        }
    }
}

public enum BoardEvent
{
    CancelInvert,
    Invert,
    EarnDiamonds,
    DownDiamond,
    AllClear
}
