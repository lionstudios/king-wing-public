using System.Collections;
using System.Configuration;
using Events.InGame.EventArgs;
using LionStudios.Suite.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ShopBuyButton : BaseButton
{
    [SerializeField] private TextMeshProUGUI _priceTxt;
    [SerializeField] private TextMeshProUGUI _equipTxt;
    [SerializeField] private Image _coinImg;


    public void UpdatePropertyElements(bool isUnlocked, bool isSelected, bool isEquipped, int price)
    {
        // Debug.Log("Unlocked Status: " + isUnlocked + " Selected Status: " + isSelected + " Equipped Status: " +
        //           isEquipped);
        gameObject.SetActive(!isUnlocked && isSelected || isUnlocked && isSelected && !isEquipped);
        _equipTxt.gameObject.SetActive(isUnlocked && isSelected && isEquipped);
        _priceTxt.text = isUnlocked ? "Equip" : price.ToString();
        _coinImg.enabled = !isUnlocked;
        //Debug.Log("Updated log of property");
    }

    protected override void OnClick()
    {
        ShopItem _item = ShopManager.Instance.CurrentSelectedItem;
        UiInteractionEventArgs uiInteractionEventArgs = new UiInteractionEventArgs()
        {
            UiAction = "BuyAttemptClick",
            UiLocation = "ShopScreen",
            UiName = "CoinBtn",
            UiType = "BuySkinButton"
        };
        LionAnalytics.UiInteraction(uiInteractionEventArgs);
        if (_item.IsUnlocked)
        {
            CharacterUpdatedEventArgs characterUpdatedEventArgs = new CharacterUpdatedEventArgs
            {
                CharacterClass = "LionWing",
                CharacterName = _item.itemName,
                CharacterID = _item.id
            };
            LionAnalytics.CharacterUpdated(characterUpdatedEventArgs);
            ShopManager.Instance.EquipItem(_item);
        }
        else
        {
            if (CurrencyManager.Instance.TotalMoney >= _item.price)
            {
                CurrencyManager.Instance.SpendMoney(new MoneyArgs(_item.price));
                _item.Unlock();
                Product received = new Product();
                RealCurrency realCurrency = new RealCurrency("USD", 0f);
                received.AddItem(_item.itemName, _item.id, 1);
                received.realCurrency = realCurrency;
                Product sent = new Product();
                sent.AddVirtualCurrency(new VirtualCurrency("Score", "PlayersScore", _item.price));
                sent.realCurrency = realCurrency;

                int transactionID = Random.Range(246864, 9999999);
                Transaction newTransaction =
                    new Transaction("Skin Transaction", "skin", received, sent, transactionID.ToString())
                    {
                        name = "Skin Transaction",
                        type = "skin",
                        productReceived = received,
                        productSpent = sent,
                        transactionID = transactionID.ToString(),
                        productID = _item.id
                    };

                LionAnalytics.InAppPurchase(newTransaction);
                LionAnalytics.EconomyEvent(newTransaction);
                LionAnalytics.FeatureUnlocked(_item.itemName, "itemPurchase");
                Reward reward = new Reward(_item.itemName, "LionWing", 1);
                LionAnalytics.ItemCollected(reward);
                ShopManager.Instance.EquipItem(_item);
                ItemActionedEventArgs itemActionedEventArgs = new ItemActionedEventArgs
                {
                    Action = "BuyAttemptSuccessClick",
                    ItemId = _item.id,
                    ItemName = _item.itemName,
                    ItemType = "LionWing"
                };
                LionAnalytics.ItemActioned(itemActionedEventArgs);

                CharacterUpdatedEventArgs characterUpdatedEventArgs = new CharacterUpdatedEventArgs
                {
                    CharacterClass = "LionWing",
                    CharacterName = _item.itemName,
                    CharacterID = _item.id
                };
                LionAnalytics.CharacterUpdated(characterUpdatedEventArgs);

                LionAnalytics.PowerUpUsed("Level", "Normal", GameManager.Instance.Attempts, _item.itemName);
            }
            else
            {
                bool canShowDialogue = !PopupManager.Instance.isShowing;
                if (!canShowDialogue) return;
                IEnumerator routine = WarningMessage(1f);
                PopupManager.Instance.ShowPopupTemporarily("Not enough coins", 1f);

                ItemActionedEventArgs itemActionedEventArgs = new ItemActionedEventArgs
                {
                    Action = "BuyAttemptFailedClick",
                    ItemId = _item.id,
                    ItemName = _item.itemName,
                    ItemType = "LionWing"
                };
                LionAnalytics.ItemActioned(itemActionedEventArgs);
                StartCoroutine(routine);
            }
        }
    }

    private IEnumerator WarningMessage(float waitDuration)
    {
        Button currentButton = GetComponent<Button>();
        currentButton.interactable = false;
        yield return new WaitForSeconds(waitDuration);
        currentButton.interactable = true;
    }
}