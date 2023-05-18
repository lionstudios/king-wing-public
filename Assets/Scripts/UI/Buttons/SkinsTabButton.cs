using UnityEngine;

public class SkinsTabButton : BaseButton
{
    public GameObject coinsPanel;
    public GameObject skinsPanel;
    protected override void OnClick()
    {
        skinsPanel.SetActive(true);
        coinsPanel.SetActive(false);
    }
}
