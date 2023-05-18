using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMoneyLabel : BaseLabel
{
    protected override string GetTextValue()
    {
        return CurrencyManager.Instance.TotalMoney.ToString();
    }
}
