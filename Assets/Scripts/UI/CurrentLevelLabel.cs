using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelLabel : BaseLabel
{
    protected override string GetTextValue()
    {
        return $"Level: {LevelManager.Instance.CurrentLevel}";
    }
}
