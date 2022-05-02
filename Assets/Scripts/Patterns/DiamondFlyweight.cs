using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum DiamondState
{
    StartMove,
    EndMove
}

public class ConcreteDiamond : Subject
{
    public static HashSet<Tween> DiamondMoves = new HashSet<Tween>();

    public float duration = 0.8f;

    public void Move(MonoBehaviour diamond, Vector2 targetPosition, TweenCallback onEnd, AnimationCurve ease)
    {
        Tween tween = diamond.transform.DOMove(targetPosition, duration, false);
        tween.onComplete += onEnd;
        tween.onComplete += () => {
            SendMessage(DiamondState.EndMove, tween);
        };
        SendMessage(DiamondState.StartMove, tween);

        if (ease.length > 1)
        {
            tween.SetEase(ease);
        }
        return;
    }
    public void Move(MonoBehaviour diamond, Vector2 targetPosition, TweenCallback onEnd, Ease ease)
    {
        Tween tween = diamond.transform.DOMove(targetPosition, duration, false).SetEase(ease);
        tween.onComplete += onEnd;
        tween.onComplete += () => {
            SendMessage(DiamondState.EndMove, tween);
        };
        SendMessage(DiamondState.StartMove, tween);
        return;
    }
}

static class DiamondFlyweightFactory
{
    private static ConcreteDiamond diamond;
    public static bool IsEnable => diamond != null;

    public static ConcreteDiamond GetDiamond ()
    {
        if (diamond == null)
        {
            return new ConcreteDiamond();
        }

        return diamond;
    }
}
