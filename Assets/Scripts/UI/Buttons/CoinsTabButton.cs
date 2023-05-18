using UnityEngine;

public class CoinsTabButton : BaseButton
{
    public GameObject coinsPanel;
    public GameObject skinsPanel;
    protected override void OnClick()
    {
        coinsPanel.SetActive(true);
        skinsPanel.SetActive(false);
    }
}
