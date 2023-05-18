using System;
using UnityEngine;
using Utils;

public class SkipToEndTest : BaseButton
{
    
    public Transform endPoint;

    protected override void OnClick()
    {
        var transformCurrent = GameManager.Instance.lionCharacter.transform.parent.transform;
        Vector3 currentPosition = transformCurrent.position;
        currentPosition.x = endPoint.position.x;
        transformCurrent.position = currentPosition;
        for (int i = 0; i < 40; i++)
        {
            GameManager.Instance.IncrementScore();
        }
        MoneyArgs moneyArgs = new MoneyArgs(40);
        _dispatcher.Send(EventId.AddMoney, moneyArgs);
    }
}