using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ShopItemSwitch : EventArgs
{
    public readonly ShopItem equipped;
    public readonly ShopItem selected;
    public readonly bool explicitEquippedUse;

    public ShopItemSwitch(ShopItem currentEquipped, ShopItem currentSelected, bool useEquipped)
    {
        equipped = currentEquipped;
        selected = currentSelected;
        explicitEquippedUse = useEquipped;
    }
}

public class ShopManager : MonoSingleton<ShopManager>
{
    private const string SELECTED_ITEM_KEY = "ShopItemSelected";
    private const string EQUIPPED_ITEM_KEY = "ShopItemEquipped";

    [SerializeField] private ShopItem[] shopItems;

    [SerializeField] private ShopItem defaultSelected;
    [SerializeField] private ShopItem defaultEquipped;


    [SerializeField] private Transform container;

    [SerializeField] private ShopItemView itemViewPrefab;

    [SerializeField] private ShopBuyButton _buyButton;

    public ShopBuyButton shopBuyButton => _buyButton;

    private readonly Dictionary<string, ShopItem> _itemsById = new Dictionary<string, ShopItem>();
    public ShopItem CurrentSelectedItem => _itemsById[PlayerPrefs.GetString(SELECTED_ITEM_KEY, defaultSelected.id)];
    public ShopItem CurrentEquippedItem => _itemsById[PlayerPrefs.GetString(EQUIPPED_ITEM_KEY, defaultEquipped.id)];

    private Dispatcher _dispatcher;

    protected override void OnAwake()
    {
        base.OnAwake();
        foreach (ShopItem shopItem in shopItems)
            _itemsById.Add(shopItem.id, shopItem);
    }

    private void Start()
    {
        _dispatcher = GameManager.Dispatcher;
        container.DestroyChildrenImmediate();

        foreach (var shopItem in shopItems)
        {
            ShopItemView shopItemView = Instantiate(itemViewPrefab, container);
            shopItemView.Init(shopItem);
        }
        shopBuyButton.UpdatePropertyElements(CurrentEquippedItem.IsUnlocked, CurrentEquippedItem == CurrentSelectedItem,
            true, CurrentEquippedItem.price);
        shopBuyButton.UpdatePropertyElements(CurrentSelectedItem.IsUnlocked, true,
            CurrentSelectedItem == CurrentEquippedItem, CurrentSelectedItem.price);
    }

    public void EquipItem(ShopItem item)
    {
        PlayerPrefs.SetString(EQUIPPED_ITEM_KEY, item.id);
        ShopItemSwitch switchArgs = new ShopItemSwitch(CurrentEquippedItem, CurrentSelectedItem, true);
        _dispatcher.Send(EventId.ShopItemEquippedChange, switchArgs);
        shopBuyButton.UpdatePropertyElements(item.IsUnlocked, item == CurrentSelectedItem,
            true, item.price);
    }

    public void SelectItem(ShopItem item)
    {
        PlayerPrefs.SetString(SELECTED_ITEM_KEY, item.id);
        ShopItemSwitch switchArgs = new ShopItemSwitch(CurrentEquippedItem, CurrentSelectedItem, false);
        _dispatcher.Send(EventId.ShopItemSelectedChange, switchArgs);
        shopBuyButton.UpdatePropertyElements(item.IsUnlocked, true,
            item == CurrentEquippedItem, item.price);
    }
}