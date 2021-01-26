using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Item")]

public class Item : ScriptableObject
{
    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string itemName;




    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public ItemRarity itemRarity;



    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public int basePay;




    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    [TextArea]
    public string description;



    [HorizontalGroup("Game Data", 75)]
    [PreviewField(75)]
    [HideLabel]
    public Sprite iconImage;


    [BoxGroup("Pay Per Amount")]
    public List<int> payPerAmount;
    [BoxGroup("Amount Threshold")]
    public List<int> amountThreshold;


    [HideInEditorMode]
    public int amount;


    public Item(Item item, int amount)
    {
        itemName = item.itemName;
        itemRarity = item.itemRarity;
        basePay = item.basePay;
        description = item.description;
        iconImage = item.iconImage;
        payPerAmount = item.payPerAmount;
        amountThreshold = item.amountThreshold;
        this.amount = amount;
    }

    public int GetPayoutForHowMuchOwned()
    {
        int newPayRate = basePay;
        if (payPerAmount != null)
        {
            for (int i = 0; i < amountThreshold.Count; i++)
            {
                if (amount >= amountThreshold[i])
                {
                    newPayRate = payPerAmount[i];
                }
            }
        }
        newPayRate = newPayRate * amount;
        return newPayRate;
    }
}


public enum ItemRarity
{
    Gold,
    Common,
    UnCommon,
    Rare,
    VeryRare
}