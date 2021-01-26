
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using System.Collections;

public class IntiativeScreen : MonoBehaviour
{
    [SerializeField] private GameMaster gm;
    [SerializeField] private TextMeshProUGUI teamSelectText;
    [SerializeField] private List<GameObject> screens;

    [SerializeField] private GameObject newRoundButton;


    //Screen 1
    [SerializeField] private SliderManager spendGoldSlider;
    [SerializeField] private TextMeshProUGUI currentPlayerPickText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI playerGoldAmountOwnedText;


    //SCREEN 2
    [SerializeField] private InitiativeScreenBar playerBar1;
    [SerializeField] private InitiativeScreenBar playerBar2;
    [SerializeField] private List<GameObject> screen2screens;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private TextMeshProUGUI bottomResultsText1;
    [SerializeField] private TextMeshProUGUI bottomResultsText2;
    [SerializeField] private TextMeshProUGUI awardedGoldText;

    [SerializeField] private GameObject playRoundButton;

    private Player playerThatWon;
    private Player playerThatLost;
    private InitWinner initWinner;







    //DATA
    private Player currentPlayerSelect;
    private int playerCount;
    private int spendGoldAmount;
    private List<int> playerGoldSubmit;
    public Player prevWinner;



    private Player newStartPlayer;
    private void OnEnable()
    {
        //Reset list
        if (playerGoldSubmit == null)
        {
            playerGoldSubmit = new List<int>();
        }
        else
        {
            playerGoldSubmit.Clear();
        }


        //If round is the first round (0)
        playerCount = 0;
        if (gm.round == 0)
        {
            GoToScreen(screens, 0);
            newRoundButton.SetActive(false);
            PickNewIniativeWinner();
        }
        //All other rounds besides first round
        else
        {
            GoToScreen(screens, 1);
            ResetScreen();
        }

        
    }



    //SCREEN 0
    private void PickNewIniativeWinner()
    {
        if (RandNum(0, 1) == 0)
        {
            AssignText(gm.allPlayers[0].playerName);
            newStartPlayer = gm.allPlayers[0];
            prevWinner = gm.allPlayers[0];
        }
        else
        {
            AssignText(gm.allPlayers[1].playerName);
            newStartPlayer = gm.allPlayers[1];
            prevWinner = gm.allPlayers[1];


        }

    }

    private void AssignText(string name)
    {

        teamSelectText.DOText(name, 2f, true, ScrambleMode.All).OnComplete(ActivateButton);

    }

    private int RandNum(int num1, int num2)
    {
        return UnityEngine.Random.Range(num1, num2 + 1);
    }

    private void ActivateButton()
    {
        newRoundButton.SetActive(true);
    }










    //SCREEN 1

    private void ResetScreen()
    {
        currentPlayerSelect = gm.allPlayers[playerCount];
        UpdatePlayerText(currentPlayerSelect);
        UpdateSliderMaxValueAndCurrentValue(currentPlayerSelect);
        UpdateGoldFromSlider();

    }
    public void UpdateGoldFromSlider()
    {
        spendGoldAmount = Mathf.RoundToInt(spendGoldSlider.mainSlider.value);
        goldText.text = spendGoldAmount.ToString();
    }
    private void UpdateSliderMaxValueAndCurrentValue(Player player)
    {
        spendGoldSlider.mainSlider.value = 0;
        spendGoldSlider.mainSlider.maxValue = player.resources[0];


    }
    private void UpdatePlayerText(Player player)
    {
        currentPlayerPickText.text = player.playerName;
        currentPlayerPickText.color = player.teamColor;

        playerGoldAmountOwnedText.text = player.resources[0].ToString();
    }
    public void SubmitGoldButton()
    {
        playerGoldSubmit.Add(spendGoldAmount);

        playerCount += 1;

        if (playerGoldSubmit.Count >= gm.allPlayers.Count)
        {
            //Go to results screen
            GoToScreen(screens, 2);
            GoToScreen(screen2screens, 0);

        }
        else
        {
            ResetScreen();
        }

    }







    //Screen 2

    public void SeeResultsButton()
    {

        AnimateInStats();
        GoToScreen(screen2screens, 1);
        playRoundButton.SetActive(false);
    }
    private void AnimateInStats()
    {
        resultsText.text = "Lets see who won.";
        initWinner = InitWinner.Default;
        float timer = 2;


        int maxGoldSpent = 0;
        Player playerSpentMost = null;
        for (int i = 0; i < playerGoldSubmit.Count; i++)
        {
            if (playerGoldSubmit[i] > maxGoldSpent)
            {
                maxGoldSpent = playerGoldSubmit[i];
                playerSpentMost = gm.allPlayers[i];
            }
            else if (playerGoldSubmit[i] == maxGoldSpent && maxGoldSpent != 0)
            {
                //Both players tied - Tie goes to prev winner
                playerSpentMost = null;
            }
        }

        if (maxGoldSpent == 0)
        {
            //Both players didnt bid
            timer = .5f;

            playerThatWon = prevWinner;
            initWinner = InitWinner.NoBid;
        }
        else if (playerSpentMost == null)
        {
            //Tied bid
            playerThatWon = prevWinner;
            initWinner = InitWinner.Tied;
        }
        else
        {
            playerThatWon = playerSpentMost;
            initWinner = InitWinner.Player;
        }




        int loserIndex = 0;
        if (playerThatWon.playerNumberIndex == 0)
        {
            loserIndex = 1;
        }
        playerThatLost = gm.allPlayers[loserIndex];


        playerBar1.LoadInfo(gm.allPlayers[0], playerGoldSubmit[0], maxGoldSpent, timer);
        playerBar2.LoadInfo(gm.allPlayers[1], playerGoldSubmit[1], maxGoldSpent, timer);
        StartCoroutine(LoadFinalTextAndButton(timer));
    }


    IEnumerator LoadFinalTextAndButton(float timer)
    {
        yield return new WaitForSeconds(timer);


        if (initWinner == InitWinner.NoBid)
        {
            resultsText.text = "No one bid. Bid goes to previous winner.";
            bottomResultsText1.text = playerThatWon.playerName + " wins initiative.";
            bottomResultsText2.text = "No gold awarded:";
            awardedGoldText.text = "0";
        }
        else if (initWinner == InitWinner.Tied)
        {
            resultsText.text = "It's a tie! Bid goes to previous winner.";
            bottomResultsText1.text = playerThatWon.playerName + " wins initiative.";
            bottomResultsText2.text = playerThatLost.playerName + " awarded:";
            awardedGoldText.text = playerGoldSubmit[playerThatWon.playerNumberIndex].ToString();
        }
        else
        {
            resultsText.text = playerThatWon.playerName + " has won!";
            bottomResultsText1.text = playerThatWon.playerName + " wins initiative.";
            bottomResultsText2.text = playerThatLost.playerName + " awarded:";
            awardedGoldText.text = playerGoldSubmit[playerThatWon.playerNumberIndex].ToString();
        }
        playRoundButton.SetActive(true);

    }

    public void Screen2PlayRoundButton()
    {
        playerThatWon.UpdateGold(-playerGoldSubmit[playerThatWon.playerNumberIndex]);
        playerThatLost.UpdateGold(playerGoldSubmit[playerThatWon.playerNumberIndex]);
        newStartPlayer = playerThatWon;
        prevWinner = playerThatWon;

        playerBar1.ResetBarToZero();
        playerBar2.ResetBarToZero();


        NewRoundButton();
    }





    //OTHER
    private void GoToScreen(List<GameObject> theList, int screenNum)
    {
        for (int i = 0; i < theList.Count; i++)
        {
            if (i == screenNum)
            {
                theList[i].SetActive(true);
            }
            else
            {
                theList[i].SetActive(false);

            }
        }
    }



    //BUTTONS
    public void NewRoundButton()
    {
        gameObject.SetActive(false);
        gm.currentPlayersTurn = newStartPlayer;
        gm.TurnOnShipOutline(true);


        //Reset all players
        gm.ResetAllActivations();
        gm.AssignPlayerTurn();


        //Add round
        gm.round += 1;
    }
}

public enum InitWinner { NoBid, Tied, Player, Default }