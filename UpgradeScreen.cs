using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UpgradeScreen : MonoBehaviour
{

    [SerializeField] private GameMaster gm;

    [SerializeField] private TextMeshProUGUI headerText;

    [SerializeField] private List<UI_UpgradeBar> allUpgradeBars;
    [SerializeField] private List<Sprite> allUpgradeSprites;


    private Unit unit;
    private Player activePlayer;




    //UPGRADES
    private UpgradeCosts cannonCost;
    private UpgradeCosts moveCost;


    //Call in start in gamemaster to load for the first time
    public void InitializeUpgradeData()
    {
        //CANNON
        cannonCost = new UpgradeCosts();
        cannonCost.AddDataSet(5, 0); //2
        cannonCost.AddDataSet(7, 1); //3
        cannonCost.AddDataSet(10, 4); //4
        cannonCost.AddDataSet(20, 10); //5
        cannonCost.AddDataSet(50, 15); //6
        cannonCost.AddDataSet(100, 20); //7
        cannonCost.AddDataSet(200, 25); //8
        cannonCost.AddDataSet(-1, -1); //9








        //MOVEMENT
        moveCost = new UpgradeCosts();
        moveCost.AddDataSet(10, 5); //3
        moveCost.AddDataSet(50, 10); //4
        moveCost.AddDataSet(100, 15); //5
        moveCost.AddDataSet(200, 25); //6
        moveCost.AddDataSet(-1, -1); //7






    }

    public void LoadInfo(Unit unitCurrentlySelected)
    {
        unit = unitCurrentlySelected;
        activePlayer = gm.currentPlayersTurn;

        headerText.text = "Ship " + unit.unitName;


        RefreshList();
    }

    private void RefreshList()
    {
        allUpgradeBars[0].LoadInfo(CannonBuyAction, unit.cannonAmount, unit.GetUpgradeCannonLimit(), allUpgradeSprites[0], cannonCost.GetGold(unit.cannonAmount-1), cannonCost.GetFlag(unit.cannonAmount-1));
        allUpgradeBars[1].LoadInfo(MoveBuyAction, unit.movementDistance, unit.GetUpgradeMoveLimit(), allUpgradeSprites[1], moveCost.GetGold(unit.movementDistance - 2), moveCost.GetFlag(unit.movementDistance - 2));
    }



    //CAN'T PURCHASE
    IEnumerator ColorImage(Image imageButton)
    {
        Color redColor = new Color(1, 0, 0);
        Color whiteColor = new Color(1, 1, 1);

        imageButton.color = redColor;
        yield return new WaitForSeconds(.2f);
        imageButton.color = whiteColor;
        yield return new WaitForSeconds(.2f);
        imageButton.color = redColor;
        yield return new WaitForSeconds(.2f);
        imageButton.color = whiteColor;

    }




    //ACTIONS
    private void CannonBuyAction(Image buttonImage)
    {
        //CAN BUY
        if (activePlayer.resources[0] >= cannonCost.GetGold(unit.cannonAmount-1) && activePlayer.resources[1] >= cannonCost.GetFlag(unit.cannonAmount-1))
        {
            activePlayer.UpdateGold(-cannonCost.GetGold(unit.cannonAmount-1));
            unit.UpgradeCannons();
            gm.StartUnitTurn(unit);
            RefreshList();
        }
        //CANT BUY
        else
        {
            StartCoroutine(ColorImage(buttonImage));
        }

    }
    private void MoveBuyAction(Image buttonImage)
    {
        //CAN BUY
        if (activePlayer.resources[0] >= moveCost.GetGold(unit.movementDistance - 2) && activePlayer.resources[1] >= moveCost.GetFlag(unit.movementDistance - 2))
        {
            activePlayer.UpdateGold(-moveCost.GetGold(unit.movementDistance - 2));
            unit.UpgradeMovement();
            gm.StartUnitTurn(unit);
            RefreshList();
        }
        //CANT BUY
        else
        {
            StartCoroutine(ColorImage(buttonImage));
        }
    }


    //BUTTONS
    public void ClosePanelButton()
    {
        gm.TurnOnUpgradeScreen(false);
    }

    

}










public class UpgradeCosts
{
    private List<int[]> upgradeData;

    public UpgradeCosts()
    {
        upgradeData = new List<int[]>();
    }

    public void AddDataSet(int gold, int flag)
    {
        int[] newDataSet = new int[] { gold, flag };
        upgradeData.Add(newDataSet);
    }

    public int GetGold(int index)
    {
        return upgradeData[index][0];
    }
    public int GetFlag(int index)
    {
        return upgradeData[index][1];
    }
}