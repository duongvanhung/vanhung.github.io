using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, IObserver
{
    [SerializeField] GameObject destroy;
    [SerializeField] GameObject choose;
    [SerializeField] GameObject[] support;
    [Tooltip("distance to diamond follow Z axis")]
    [SerializeField] int offsetToDiamondFollowZAxis = -1;

    private GreenDestroyDiamond greenDestroyDiamond;

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.Instance.RegisterObserver (InputEvent.DeleteChoose, this);
        InputSystem.Instance.RegisterObserver (InputEvent.OnChoose, this);

        Board.Instance.RegisterObserver (BoardEvent.EarnDiamonds, this);
        Board.Instance.RegisterObserver (BoardEvent.Invert, this);

        Support.Instance.RegisterObserver (SupportEvent.SendSupport, this);

        if (!destroy)
        {
            Debug.LogWarning("misisng destroy effect");
        }
        else
        {
            if (!destroy.gameObject.TryGetComponent<GreenDestroyDiamond>(out greenDestroyDiamond))
            {
                Debug.LogWarning("missing green destroy script in destroy effect");
            }
        }

        if (!choose)
        {
            Debug.LogWarning("missing choose effect");
        }

        if (support.Length < 2)
        {
            Debug.LogWarning("missing support effect");
        }
    }

    private void OnDestroy() {
        if (InputSystem.IsEnable) InputSystem.Instance.RemoveAllRegister(this);
        if (Board.IsEnable) Board.Instance.RemoveAllRegister (this);
        if (Support.IsEnable) Support.Instance.RemoveAllRegister (this);
    }

    private void HandleDeleteChoose ()
    {
        if (!choose) return;
        choose.SetActive(false);
    }

    private void HandleOnChoose(object data)
    {
        if (!choose) return;

        if (data == null)
        {
            Debug.LogWarning ("There are no choosen diamond");
            return;
        }

        try
        {
            List<Diamond> choosenDiamond = (List<Diamond>)data;
            Vector3 position = new Vector3 (choosenDiamond[0].transform.position.x, 
                choosenDiamond[0].transform.position.y, offsetToDiamondFollowZAxis);
            choose.transform.position = position;
            choose.SetActive(true);
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    private void HandleEarnDiamonds(object data)
    {
        if (!greenDestroyDiamond) return;

        if (data == null)
        {
            Debug.LogWarning ("There are no position of diamond earned");
            return;
        }

        try
        {
            HashSet<Vector2Int> positions = (HashSet<Vector2Int>)data;

            foreach (var position in positions)
            {
                GreenDestroyPool.Instance.GetObject(greenDestroyDiamond).transform.position 
                = new Vector3(position.x, position.y, offsetToDiamondFollowZAxis);
            }
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    private void HandleSendSupport (object data)
    {
        if (support.Length != 2) return;

        if (data == null)
        {
            Debug.LogWarning("therer are no couple diamond support");
            return;
        }

        try
        {
            Diamond[] coupleSupport = (Diamond[])data;
            Vector3 position1 = new Vector3(coupleSupport[0].transform.position.x, 
                coupleSupport[0].transform.position.y, offsetToDiamondFollowZAxis);
            Vector3 position2 = new Vector3(coupleSupport[1].transform.position.x, 
                coupleSupport[1].transform.position.y, offsetToDiamondFollowZAxis);    
            
            support[0].transform.position = position1;
            support[1].transform.position = position2;

            support[0].SetActive(true);
            support[1].SetActive(true);
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    private void HandleInvert ()
    {
        if (support.Length != 2) return;

        support[0].SetActive(false);
        support[1].SetActive(false);
    }

    public void OnNotify(object key, object data)
    {
        
        if (key.GetType() == typeof(InputEvent))
        {
            switch ((InputEvent)key)
            {
                case InputEvent.DeleteChoose:
                    HandleDeleteChoose();
                    break;
                case InputEvent.OnChoose:
                    HandleOnChoose (data);
                    break;
                default:
                    Debug.LogWarning ("invalute input event");
                    break;
            }
        }
        else if (key.GetType() == typeof(BoardEvent))
        {
            switch ((BoardEvent)key)
            {
                case BoardEvent.EarnDiamonds:
                    HandleEarnDiamonds (data);
                    break;
                case BoardEvent.Invert:
                    HandleInvert ();
                    break;
                default:
                    Debug.LogWarning ("invalute board event");
                    break;
            }
        }
        else if (key.GetType() == typeof(SupportEvent))
        {
            switch ((SupportEvent)key)
            {
                case SupportEvent.SendSupport:
                    HandleSendSupport (data);
                    break;
                default:
                    Debug.LogWarning ("invalute board event");
                    break;
            }
        }
    }
}
