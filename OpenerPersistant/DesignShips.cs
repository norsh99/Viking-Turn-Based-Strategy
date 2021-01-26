using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DesignShips : MonoBehaviour
{


    [SerializeField] private List<UI_PickShipCard> shipCards;
    [SerializeField] private List<Image> readyImages;
    [SerializeField] private List<Color> readyColors;
    [SerializeField] private GameObject submitButton;
    [SerializeField] private GameObject helpScreen;


    private List<UI_PickShipCard> completedShips;

    [SerializeField] private Transform mainContent;
    private Vector3 mainContentStartPos;
    [SerializeField] private TextMeshProUGUI headerText;


    private List<Ship> playerShips1;
    private List<Ship> playerShips2;

    private int playerPick;


    private void Start()
    {
        mainContentStartPos = mainContent.position;
        playerPick = 0;
        playerShips1 = new List<Ship>();
        playerShips2 = new List<Ship>();
        headerText.text = "Player 1 Ships";

        

        completedShips = new List<UI_PickShipCard>();
        shipCards[0].changedStats += CheckCardsStatus1;
        shipCards[1].changedStats += CheckCardsStatus2;
        shipCards[2].changedStats += CheckCardsStatus3;

        ResetAll();
    }

    private void ResetAll()
    {
        completedShips.Clear();
        ResetReadyImages();
        submitButton.SetActive(false);
        for (int i = 0; i < shipCards.Count; i++)
        {
            shipCards[i].ResetStats();
        }
    }

    public List<Ship> GetShips()
    {
        List<Ship> newList = new List<Ship>();
        for (int i = 0; i < shipCards.Count; i++)
        {
            Ship ship = new Ship(shipCards[i].GetFinalShip());
            newList.Add(ship);
        }


        return newList;
    }


    private void CheckIfAllCardsAreReady()
    {
        if (completedShips.Count == 3)
        {
            submitButton.SetActive(true);
        }
        else
        {
            submitButton.SetActive(false);
        }
    }

    private void ResetReadyImages()
    {
        for (int i = 0; i < readyImages.Count; i++)
        {
            readyImages[i].color = readyColors[0];
        }
    }

    private void SetReadyImage(Image image, bool isFinished)
    {
        if (isFinished)
        {
            image.color = readyColors[1];
        }
        else
        {
            image.color = readyColors[0];
        }
    }

    private void CheckCardsStatus1()
    {
        if (shipCards[0].isComplete)
        {
            if (!completedShips.Contains(shipCards[0]))
            {
                completedShips.Add(shipCards[0]);
            }
        }
        else
        {
            if (completedShips.Contains(shipCards[0]))
            {
                completedShips.Remove(shipCards[0]);
            }
        }
        SetReadyImage(readyImages[0], shipCards[0].isComplete);
        CheckIfAllCardsAreReady();
    }
    private void CheckCardsStatus2()
    {
        if (shipCards[1].isComplete)
        {
            if (!completedShips.Contains(shipCards[1]))
            {
                completedShips.Add(shipCards[1]);
            }
        }
        else
        {
            if (completedShips.Contains(shipCards[1]))
            {
                completedShips.Remove(shipCards[1]);
            }
        }
        SetReadyImage(readyImages[1], shipCards[1].isComplete);
        CheckIfAllCardsAreReady();
    }
    private void CheckCardsStatus3()
    {
        if (shipCards[2].isComplete)
        {
            if (!completedShips.Contains(shipCards[2]))
            {
                completedShips.Add(shipCards[2]);
            }
        }
        else
        {
            if (completedShips.Contains(shipCards[2]))
            {
                completedShips.Remove(shipCards[2]);
            }
        }
        SetReadyImage(readyImages[2], shipCards[2].isComplete);
        CheckIfAllCardsAreReady();
    }

    private void SetupNextPlayer()
    {
        mainContent.transform.position = new Vector3(mainContentStartPos.x - 2000, mainContentStartPos.y, mainContentStartPos.z);
        mainContent.DOMoveX(mainContentStartPos.x, .5f).SetEase(Ease.InOutExpo);

        headerText.text = "Player 2 Ships";
        ResetAll();
    }

    private void GoToGame()
    {
        Debug.Log("LALALAAL");
        

        GameManager.instance.LoadGame(playerShips1, playerShips2);
        //GameMaster.current.LoadShipsIn(playerShips1, playerShips2);
        Debug.Log("ggggg");
    }

    //BUTTONS
    public void SubmitButton()
    {
        
        if (playerPick == 0)
        {
            playerShips1 = GetShips();
            submitButton.SetActive(false);
            mainContent.DOMoveX(mainContentStartPos.x + 2000, .5f).SetEase(Ease.InOutExpo).OnComplete(SetupNextPlayer);

        }
        else if (playerPick == 1)
        {
            playerShips2 = GetShips();
            playerPick += 1;
        }

        if (playerPick == 2)
        {
            //Go to game
            mainContent.DOMoveX(mainContentStartPos.x + 2000, .5f).SetEase(Ease.InOutExpo).OnComplete(GoToGame);
        }
        playerPick += 1;
    }

    public void HelpScreen(bool turnOn)
    {
        if (turnOn)
        {
            helpScreen.SetActive(true);

        }
        else
        {
            helpScreen.SetActive(false);
        }
    }

}
