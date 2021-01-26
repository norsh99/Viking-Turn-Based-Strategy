using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RepairScreen : MonoBehaviour
{

    [SerializeField] private GameMaster gm;
    [SerializeField] private PlayerCard playerCard;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private Image woodImage;
    [SerializeField] private TextMeshProUGUI costText;


    private Player player;
    private Unit unit;

    private int requiredWood = 10;
    public void LoadInfo(Player player, Unit unit)
    {
        this.player = player;
        this.unit = unit;
        costText.text = requiredWood.ToString();

        playerCard.UpdateCardWithOtherPlayerCard(unit.playerCard);

        buyButton.SetActive(true);

        gameObject.SetActive(true);
    }


    //CAN'T PURCHASE
    IEnumerator ColorImage(Image imageButton, Image woodImage)
    {
        Color redColor = new Color(1, 0, 0);
        Color whiteColor = new Color(1, 1, 1);

        imageButton.color = redColor;
        woodImage.color = redColor;
        yield return new WaitForSeconds(.2f);
        imageButton.color = whiteColor;
        woodImage.color = whiteColor;

        yield return new WaitForSeconds(.2f);
        imageButton.color = redColor;
        woodImage.color = redColor;

        yield return new WaitForSeconds(.2f);
        imageButton.color = whiteColor;
        woodImage.color = whiteColor;
    }



    //BUTTONS
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        gm.UILower.RefreshActionCards();
        gm.UILower.canClickActionCard = true;
        gm.TurnOnShipOutline(true);

    }

    public void RepairShip()
    {
        if (player.resources[2] >= requiredWood && unit.hitPoints < unit.maxHitPoints)
        {
            unit.UpdateHitPoints(1);
            player.UpdateWood(-requiredWood);
            playerCard.UpdateCardWithOtherPlayerCard(unit.playerCard);
            gm.UILower.selectedPlayerCard.UpdateCardWithOtherPlayerCard(unit.playerCard);
            gm.StartUnitTurn(unit);
        }
        else
        {
            Debug.Log("CLEARRR");
            StartCoroutine(ColorImage(buyButton.GetComponent<Image>(), woodImage));
        }

        if (unit.hitPoints == unit.maxHitPoints)
        {
            buyButton.SetActive(false);
        }
    }
}
