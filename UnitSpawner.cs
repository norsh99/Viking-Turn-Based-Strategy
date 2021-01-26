using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    //[OnValueChanged("LoadInfo")]
    [SerializeField] private GameMaster gm;
    private HexSpawner hexSpawner;
    public void LoandInfo(HexSpawner hexSpawner, Player player1, Player player2)
    {
        this.hexSpawner = hexSpawner;
        ReadPlayerShipsAndSpawn(player1, player2);


    }

    private void ReadPlayerShipsAndSpawn(Player player1, Player player2)
    {
        Hex hexSpawn1 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f + 1)][hexSpawner.hexGrid.leftOffset]; //NORMAL
        Hex hexSpawn2 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f - 1)][hexSpawner.hexGrid.leftOffset]; //NORMAL
        Hex hexSpawn3 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f - 3)][hexSpawner.hexGrid.leftOffset]; //NORMAL





        //Hex hexSpawn4 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f +2)][hexSpawner.hexGrid.leftOffset + 1]; //DEBUG
        //Hex hexSpawn5 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f +4)][hexSpawner.hexGrid.leftOffset + 1]; //DEBUG

        Hex hexSpawn4 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f + 2)][hexSpawner.hexGrid.cols - hexSpawner.hexGrid.rightOffset - hexSpawner.hexGrid.rightInnerOffset + 1]; //NORMAL
        Hex hexSpawn5 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f )][hexSpawner.hexGrid.cols - hexSpawner.hexGrid.rightOffset - hexSpawner.hexGrid.rightInnerOffset + 1]; //NORMAL
        Hex hexSpawn6 = hexSpawner.hexes[Mathf.FloorToInt((float)hexSpawner.hexGrid.rows / 2f - 2)][hexSpawner.hexGrid.cols - hexSpawner.hexGrid.rightOffset - hexSpawner.hexGrid.rightInnerOffset + 1]; //NORMAL


        SpawnShip(player1.GetShip(0), hexSpawn1, player1, "1");
        SpawnShip(player1.GetShip(1), hexSpawn2, player1, "2");
        SpawnShip(player1.GetShip(2), hexSpawn3, player1, "3");





        SpawnShip(player2.GetShip(0), hexSpawn4, player2, "1");
        SpawnShip(player2.GetShip(1), hexSpawn5, player2, "2");
        SpawnShip(player2.GetShip(2), hexSpawn6, player2, "3");



    }

    private void SpawnShip(Ship shipData, Hex hexLocation, Player player, string shipName)
    {
        GameObject newShip = Instantiate(shipData.shipModel);
        newShip.transform.SetParent(this.transform);
        newShip.transform.localPosition = hexLocation.transform.position - new Vector3(0,.6f,0);
        newShip.transform.rotation = Quaternion.identity;


        //Stats
         Unit unit = newShip.GetComponent<Unit>();
        unit.LoadInfo(shipData, gm, player, hexLocation, shipName);


        if (gm.allUnits == null)
        {
            gm.allUnits = new List<Unit>();
        }
        if (gm.allAliveUnits == null)
        {
            gm.allAliveUnits = new List<Unit>();
        }

        gm.allUnits.Add(unit);
        gm.allAliveUnits.Add(unit);

        hexLocation.currentUnitOnHex = unit;
        hexLocation.SetHexPlayerColor();

    }

}
