using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLootPopUp : MonoBehaviour
{
    public GameMaster gm;
    public List<UI_LootItem> allItemsBars;

    [OdinSerialize, ReadOnly] private List<Item> allNewItems;

    public void LoadInfo(List<Item> allNewItems)
    {
        this.allNewItems = allNewItems;

        for (int i = 0; i < allItemsBars.Count; i++)
        {
            if (i < allNewItems.Count)
            {
                allItemsBars[i].gameObject.SetActive(true);


                allItemsBars[i].LoadInfo(allNewItems[i], gm.GetColorFromRarity(allNewItems[i].itemRarity));
            }
            else
            {
                allItemsBars[i].gameObject.SetActive(false);

            }
        }
    }

    public void OkButton()
    {
        gm.AddLootToPLayer(allNewItems, gm.currentPlayersTurn);
        gameObject.SetActive(false);
        gm.UILower.canClickActionCard = true;
        gm.TurnOnShipOutline(true);

    }

}
