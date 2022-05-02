using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Spawner : SingletonMono<Spawner>
{
    private Spawner () {}
    [SerializeField] public GameObject[] diamondPrefab;
    [Tooltip("Sum have to equal 1")]
    [SerializeField] private float[] ratioDiamond; 

    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        if (!CheckStart())
        {
            return;
        }

        board = Board.Instance;
        
        InitDiamonds();

        Invoke("StartCheck", 1);
    }

    private void StartCheck() {
        board.CheckAll();
    }

    public List<Diamond> GenerateColumnDiamonds (int column, int amount)
    {
        if (column < 0 || column >= board.Weight) throw new System.IndexOutOfRangeException("Invalute column");
        if (amount <= 0) return null;

        var spawnDiamonds = new List<Diamond>();

        for (int i = 0; i < amount; i++)
        {
            int x = column;
            int y = board.Height + i;

            int id = GetIDDiamondFollowRatio();
            Diamond diamond;
            if (!DiamondPool.Instance.GetObject(diamondPrefab[id]).TryGetComponent<Diamond>(out diamond))
            {
                Debug.LogWarning("Missing Diamond class in Diamond prefab");
                return null;
            }

            diamond.transform.position = new Vector2(x, y);
            diamond.name = diamond.tag;

            spawnDiamonds.Add (diamond);
        }

        return spawnDiamonds;
    }

    private bool CheckStart ()
    {
        if (diamondPrefab.Length == 0)
        {
            Debug.LogWarning("Warning!!! have no diamond prefab");
            return false;
        }

        if (ratioDiamond.Length != diamondPrefab.Length)
        {
            Debug.LogWarning("invalue ratio diamond");
            return false;
        }
        else
        {
            float sumRatio = 0;

            foreach (float f in ratioDiamond)
            {
                sumRatio += f;
            }

            if ((int)sumRatio != 1)
            {
                Debug.LogWarning("invalue ratio diamond");
                return false;
            }
        }

        return true;
    }

    private void InitDiamonds ()
    {
        int height = board.Height;
        int weight = board.Weight;

        for (int y = 0; y < height; y ++)
        {
            for (int x = 0; x < weight ; x ++)
            {
                int id = GetIDDiamondFollowRatio();

                Diamond diamond;
                if (!DiamondPool.Instance.GetObject(diamondPrefab[id]).TryGetComponent<Diamond>(out diamond))
                {
                    Debug.LogWarning("Missing Diamond class in Diamond prefab");
                    return;
                }
       
                diamond.transform.position = new Vector2(x, y);
                diamond.name = diamond.tag;

                //Load to board
                board.LoadDiamond(diamond, x, y);
            }
        }
    }

    private int GetIDDiamondFollowRatio ()
    {
        float currentRatio = 0;
        float random = Random.Range(0f, 1f);

        for (int i = 0; i < ratioDiamond.Length; i++)
        {
            currentRatio += ratioDiamond[i];

            if (random <= currentRatio)
            {
                return i;
            }
        }

        return -1;
    }
}
