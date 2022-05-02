using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiamondAnimController : MonoBehaviour, IObserver
{
    public List<Tween> DiamondMoveTweens = new List<Tween>();

    private void Start() {
        DiamondFlyweightFactory.GetDiamond().RegisterObserver(DiamondState.StartMove, this);
        DiamondFlyweightFactory.GetDiamond().RegisterObserver(DiamondState.EndMove, this);

        GameManager.Instance.RegisterObserver (GameStates.Pause, this);
        GameManager.Instance.RegisterObserver (GameStates.Resume, this);
    }

    private void OnDestroy() {
        if (DiamondFlyweightFactory.IsEnable) DiamondFlyweightFactory.GetDiamond().RemoveAllRegister(this);
        if (GameManager.IsEnable) GameManager.Instance.RemoveAllRegister(this);
    }

    private void AddDiamondTween (Tween tween)
    {
        DiamondMoveTweens.Add(tween);
    }

    private void RemoveDiamondTween (Tween tween)
    {
        DiamondMoveTweens.Remove(tween);
    }

    private void OnStartMove(object data)
    {
        if (data == null)
        {
            Debug.LogWarning("Have no tween to add");
            return;
        }
        
        try 
        {
            AddDiamondTween ((Tween)data);
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    private void OnEndMove (object data)
    {
        if (data == null)
        {
            Debug.LogWarning("Have no tween to remove");
            return;
        }

        try 
        {
            RemoveDiamondTween ((Tween)data);
        }
        catch (System.InvalidCastException)
        {
            Debug.LogWarning("faile to cast value");
            return;
        }
    }

    public void PauseDiamondMove ()
    {
        foreach (var tween in DiamondMoveTweens)
        {
            tween.Pause();
        }
    }

    public void ContinueDiamondMove ()
    {
        foreach (var tween in DiamondMoveTweens)
        {
            tween.Play();
        }
    }

    public void OnNotify(object key, object data)
    {
        if (key.GetType() == typeof(DiamondState))
        {
            switch ((DiamondState)key)
            {
                case DiamondState.StartMove:
                    OnStartMove(data);
                    break;
                case DiamondState.EndMove:
                    OnEndMove (data);
                    break;
                default:
                    Debug.LogWarning("invalute diamond state");
                    break;
            }
        }
        else if (key.GetType() == typeof(GameStates))
        {
            switch ((GameStates)key)
            {
                case GameStates.Pause:
                    PauseDiamondMove();
                    break;
                case GameStates.Resume:
                    ContinueDiamondMove();
                    break;
                default:
                    Debug.LogWarning("invalute game state");
                    break;
            }
        }
        
    }
}
