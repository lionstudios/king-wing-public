using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentScoreLabel : BaseLabel
{
    protected override string GetTextValue()
    {
        return GameManager.Instance.LevelMoney.ToString();
    }
}
