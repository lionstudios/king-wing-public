using System;
using Events.InGame.EventArgs;
using LionStudios.Suite.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Image iconImg;

    [SerializeField] private TextMeshProUGUI priceLbl;
    [SerializeField] private GameObject selectedImg;
    [SerializeField] private GameObject EquippedImg;


    [SerializeField] private ShopItem _item;

    private Dispatcher _dispatcher;

    protected virtual void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _dispatcher.Unsubscribe(EventId.ShopItemSelectedChange, UpdateContentOnSelectionChange);
        _dispatcher.Unsubscribe(EventId.ShopItemEquippedChange, UpdateContentOnEquipChange);
    }

    public virtual void Init(ShopItem item)
    {
        _item = item;
        priceLbl.text = _item.price.ToString();
        _dispatcher = GameManager.Dispatcher;
        _dispatcher.Subscribe(EventId.ShopItemSelectedChange, UpdateContentOnSelectionChange);
        _dispatcher.Subscribe(EventId.ShopItemEquippedChange, UpdateContentOnEquipChange);
        ShopItemSwitch switchArgs = new ShopItemSwitch(ShopManager.Instance.CurrentEquippedItem,
            ShopManager.Instance.CurrentSelectedItem, true);

        _dispatcher.Send(EventId.ShopItemEquippedChange, switchArgs);
        _dispatcher.Send(EventId.ShopItemSelectedChange, switchArgs);
    }

    private void UpdateContentOnSelectionChange(EventArgs args)
    {
        ShopItemSwitch switchArgs = (ShopItemSwitch)args;
        iconImg.sprite = _item.icon;
        selectedImg.SetActive(switchArgs.selected == _item);
    }

    private void UpdateContentOnEquipChange(EventArgs args)
    {
        ShopItemSwitch switchArgs = (ShopItemSwitch)args;
        iconImg.sprite = _item.icon;
        EquippedImg.SetActive(switchArgs.equipped == _item);
    }

    private void OnClick()
    {
        ItemActionedEventArgs itemActionedEventArgs = new ItemActionedEventArgs
        {
            Action = "Clicked",
            ItemId = _item.id,
            ItemName = _item.itemName,
            ItemType = "Wings"
        };
        LionAnalytics.ItemActioned(itemActionedEventArgs);
        ShopManager.Instance.SelectItem(_item);
        ShopManager.Instance.shopBuyButton.UpdatePropertyElements(_item.IsUnlocked, true,
            ShopManager.Instance.CurrentEquippedItem == _item, _item.price);
    }
}