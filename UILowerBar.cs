using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class UILowerBar : MonoBehaviour
{


    [SerializeField] private GameMaster gm;
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private GameObject actionCardPrefab;


    [SerializeField] private Transform playerShipInfoBar;
    [SerializeField] private Transform actionCardLoadInTransform;

    [SerializeField] private List<GameObject> allScreens;


    //Action Cards
    private List<GameObject> allActionCards;
    public ActionCard currentSelectedActionCard;

    //OTHER
    private GameObject currentActiveScreen;
    public bool canClickActionCard;

    //Ship info Selection screen
    [SerializeField] public PlayerCard selectedPlayerCard;
    private void Start()
    {
        currentActiveScreen = allScreens[0];
        canClickActionCard = true;
    }


    public void LoadInPlayerShipCards()
    {
        List<Unit> allUnits = gm.allUnits;

        if (gm.allPlayerCards == null)
        {
            gm.allPlayerCards = new List<PlayerCard>();
        }

        for (int i = 0; i < allUnits.Count; i++)
        {
            GameObject playerCardGO = Instantiate(playerCardPrefab);
            playerCardGO.transform.SetParent(playerShipInfoBar, false);

            PlayerCard playerCard = playerCardGO.GetComponent<PlayerCard>();
            playerCard.LoadInfo(allUnits[i], gm);
            gm.allPlayerCards.Add(playerCard);
        }
    }






    private void LoadIn(Unit unit)
    {
        if (allActionCards != null)
        {
            for (int i = 0; i < allActionCards.Count; i++)
            {
                Destroy(allActionCards[i]);
            }
        }
        else
        {
            allActionCards = new List<GameObject>();
        }
        allActionCards.Clear();



        //LOADING IN ACTION CARDS
        List<Ability> theListOfAbilities = unit.GetAllAvailableAbilities();
        for (int i = 0; i < theListOfAbilities.Count; i++)
        {
            Ability ability = theListOfAbilities[i];


            GameObject actionCard = Instantiate(actionCardPrefab);
            actionCard.transform.SetParent(actionCardLoadInTransform, false);
            actionCard.GetComponent<ActionCard>().LoadInfo(ability, gm);
            allActionCards.Add(actionCard);


            //Test for repiar action
            if (ability.title == "Repair" && unit.hitPoints == unit.maxHitPoints)
            {
                actionCard.SetActive(false); 
            }
        }
    }

    public void RefreshActionCards()
    {
        gm.selectedHex.currentUnitOnHex.RefreshAbilites();

        LoadIn(gm.selectedHex.currentUnitOnHex);

        for (int i = 0; i < allActionCards.Count; i++)
        {
            if (allActionCards[i].GetComponent<ActionCard>().cost > gm.turnActionPoints)
            {
                allActionCards[i].GetComponent<ActionCard>().ColorBorderIfCantPurchase(true);
            }
        }
    }

    public void SwitchScreens(int screenNum)
    {
        allScreens[screenNum].SetActive(true);
        currentActiveScreen.SetActive(false);
        currentActiveScreen = allScreens[screenNum];


        if (screenNum == 1)
        {
            selectedPlayerCard.UpdateCardWithOtherPlayerCard(gm.selectedHex.currentUnitOnHex.playerCard);
            LoadIn(gm.selectedHex.currentUnitOnHex);
        }
    }

    public void SpendActionCard()
    {
        currentSelectedActionCard.TriggerAbilityOFF();
        gm.SpendActionPoints(currentSelectedActionCard.cost, gm.selectedHex.currentUnitOnHex);

        RefreshActionCards();
    }
    public void ColorActionCardsIfCantSpend()
    {
        for (int i = 0; i < allActionCards.Count; i++)
        {
            ActionCard aCard = allActionCards[i].GetComponent<ActionCard>();
            if (aCard.cost > gm.turnActionPoints)
            {
                aCard.ColorBorderIfCantPurchase(true);
            }
        }
    }

    public void AfterActionCardUpdate()
    {
        SpendActionCard();
        canClickActionCard = true;

        ColorActionCardsIfCantSpend();
    }

    public void EndTurnButton()
    {
        if (canClickActionCard)
        {
            gm.EndTurn();
            //gm.SetSelectedHex(gm.selectedHex);
        }
    }
}
