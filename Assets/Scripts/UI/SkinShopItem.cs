using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkinShopItem : ShopItem
{
    [SerializeField] public Material skin;
    [SerializeField] public GameObject trail;
}
