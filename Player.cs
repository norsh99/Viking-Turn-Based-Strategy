using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    private GameMaster gm;
    private List<Ship> allShips;

    public string playerName;

    public Color teamColor;

    public int playerNumberIndex; //Is player 1, player 2, etc.

    public event Action changedResources;

    //Player Assets
    public List<int> resources { get; } //Gold, Flags, Wood
    [OdinSerialize, ReadOnly] private List<Island> ownedIslands;
    [OdinSerialize, ReadOnly] private List<Item> allOtherResources;


    public Player(List<Ship> ships, string playerName, Color teamColor, int playerIndex, UI_Player uiPlayer, GameMaster gm)
    {
        ownedIslands = new List<Island>();
        allOtherResources = new List<Item>();

        allShips = ships;
        this.teamColor = teamColor;
        playerNumberIndex = playerIndex;

        this.playerName = playerName;
        this.gm = gm;
        resources = new List<int>() { 20, 0, 0 }; //Starting Resources //Normal: 20, 0, 0



        uiPlayer.LoadInfo(this, gm);
        ChangedResources();
    }

    public void ChangedResources()
    {
        if (changedResources != null)
        {
            changedResources();
        }
    }


    public Ship GetShip(int index) { return allShips[index]; }

    public List<Ship> GetAllShips() { return allShips; }

    public void UpdateGold(int amount) { resources[0] += amount; ChangedResources(); }
    public void UpdateFlaggedLand(int amount) { resources[1] += amount; ChangedResources(); }
    public void UpdateWood(int amount) { resources[2] += amount; ChangedResources(); }
    //public void UpdateResources(int amount) { resources[3] += amount; ChangedResources(); }
    public void UpdateIslands(Island newIsland) { ownedIslands.Add(newIsland); ChangedResources(); gm.CheckIfWinner(); }
    public void RemoveIsland(Island removeIsland) { ownedIslands.Remove(removeIsland); ChangedResources(); }

    public float GetLandOwned()
    {
        int totalLand = 0;
        if (ownedIslands.Count < 1)
        {
            return 0;
        }

        for (int i = 0; i < ownedIslands.Count; i++)
        {
            totalLand += ownedIslands[i].allHexes.Count;
        }

        return totalLand;
    }

    public int GetTotalAmountOfResources()
    {
        int amount = 0;
        for (int i = 0; i < allOtherResources.Count; i++)
        {
            amount += allOtherResources[i].amount;
        }
        return amount;
    }

    public List<Item> GetResources()
    {
        return allOtherResources;
    }


    public void AddLootToPLayer(List<Item> incomingItems)
    {
        for (int i = 0; i < incomingItems.Count; i++)
        {
            //IF Gold
            if (incomingItems[i].itemName == "Gold")
            {
                UpdateGold(incomingItems[i].amount);
            }
            //IF Wood
            else if (incomingItems[i].itemName == "Wood")
            {
                UpdateWood(incomingItems[i].amount);
            }
            else
            {
                int itemIndex = gm.FindItemInListReturnIndex(GetResources(), incomingItems[i].itemName);
                if (itemIndex >= 0)
                {
                    //Consolidate
                    GetResources()[itemIndex].amount += incomingItems[i].amount;

                }
                else
                {
                    GetResources().Add(incomingItems[i]);
                }
            }
        }
        ChangedResources();
    }

    public void SellItem(Item item)
    {
        UpdateGold(item.GetPayoutForHowMuchOwned());

        gm.RemoveItemFromList(GetResources(), item.itemName);
        ChangedResources();
    }
}
