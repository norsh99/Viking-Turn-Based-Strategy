using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

public class Island
{
    private GameMaster gm;

    public List<Hex> allHexes;
    public List<Hex> allLandNextToWaterHexes;


    public List<int> playerOwnershipCount;

    public Player playerOwnedIsland; //Who owns the island

    public int totalPortsBuilt;

    private float percentToTakeOverEmptyIsland = 50f;
    private float percentToTakeOverOccupiedIsland = 100f;
    public void LoadInfo(List<Hex> allHexes, int numberOfPlayers, GameMaster gm)
    {
        playerOwnershipCount = new List<int>();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerOwnershipCount.Add(0);
        }

        this.allHexes = allHexes;
        this.gm = gm;

        allLandNextToWaterHexes = GetAllHexesNextToWater(allHexes);
        SelectPerimeter();

    }


    private List<Hex> GetAllHexesNextToWater(List<Hex> theList)
    {
        List<Hex> newList = new List<Hex>();
        for (int i = 0; i < theList.Count; i++)
        {
            foreach (Hex neighbor in theList[i].GetNeighbors())
            {
                if (neighbor.terrainType == TerrainType.Water)
                {
                    newList.Add(theList[i]);
                    break;
                }
            }
        }
        return newList;
    }


    private void SelectPerimeter()
    {
        for (int i = 0; i < allHexes.Count; i++)
        {
            allHexes[i].IslandSelect();
        }
    }
    private void UpdatePerimeterColor(Player ownerPlayer)
    {
        for (int i = 0; i < allHexes.Count; i++)
        {
            allHexes[i].SetHexPerimeterColor(ownerPlayer.teamColor);
        }
    }

    private void ResetCount()
    {
        for (int i = 0; i < playerOwnershipCount.Count; i++)
        {
            playerOwnershipCount[i] = 0;
        }
    }

    private void UpdateIslandOwnership()
    {
        for (int i = 0; i < playerOwnershipCount.Count; i++)
        {
            //IF NO ONE OWNS THE ISLAND
            if (playerOwnedIsland == null)
            {
                int ownershipRatio = Mathf.FloorToInt(((float)playerOwnershipCount[i] / allLandNextToWaterHexes.Count)*100f);
                if (ownershipRatio > percentToTakeOverEmptyIsland)
                {
                    //Island is now owned by that player
                    playerOwnedIsland = gm.allPlayers[i];
                    //Color island in that players color
                    UpdatePerimeterColor(playerOwnedIsland);
                    //Update player with new island stat
                    gm.allPlayers[i].UpdateIslands(this);
                    break;
                }
            }


            //IF SOMEONE OWNS THE ISLAND
            else
            {
                int ownershipRatio = Mathf.FloorToInt(((float)playerOwnershipCount[i] / allLandNextToWaterHexes.Count) * 100f);
                if (ownershipRatio >= percentToTakeOverOccupiedIsland)
                {
                    Player prevPlayer = playerOwnedIsland;

                    //Remove stat from previous player
                    playerOwnedIsland.RemoveIsland(this);

                    //Island is now owned by that player
                    playerOwnedIsland = gm.allPlayers[i];

                    //Color island in that players color
                    UpdatePerimeterColor(playerOwnedIsland);
                    
                    //Update player with new island stat
                    gm.allPlayers[i].UpdateIslands(this);
                    break;
                }
            }
            
        }

    }

    public void UpdateCount()
    {
        ResetCount();
        for (int i = 0; i < allHexes.Count; i++)
        {
            if (allHexes[i].building.hasFlag == true)
            {
                playerOwnershipCount[allHexes[i].building.ownerPlayer.playerNumberIndex] += 1;
            }
        }

        UpdateIslandOwnership();
    }

    public int GetHowManyMorePortsCanBeBuiltOnIsland()
    {
        int maxCanBuild = 0;
        if (allLandNextToWaterHexes.Count <= 3)
        {
            maxCanBuild = 1;
        }
        else if (allLandNextToWaterHexes.Count <= 5)
        {
            maxCanBuild = 2;
        }
        else if (allLandNextToWaterHexes.Count <= 7)
        {
            maxCanBuild = 3;
        }
        else
        {
            maxCanBuild = 4;
        }

        return maxCanBuild - totalPortsBuilt;
    }

}
