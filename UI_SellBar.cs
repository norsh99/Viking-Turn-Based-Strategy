using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Michsky.UI.ModernUIPack;

public class UI_SellBar : MonoBehaviour
{
    private GameMaster gm;

    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI goldSaleAmountText;
    [SerializeField] private List<TextMeshProUGUI> topInfoText;
    [SerializeField] private List<TextMeshProUGUI> bottomInfoText;

    [SerializeField] private List<Image> itemIcon;
    [SerializeField] private List<Image> backgroundImage;
    [SerializeField] private List<Image> darkOverlayImage;
    [SerializeField] private TooltipContent tooltipContent;

    [SerializeField] private GameObject sellButton;

    [OdinSerialize, ReadOnly] public Item item;
    private SellResourcesScreen sellResourcesScreen;

    private Color sellColor; //Obsolete
    public void LoadInfo(Item item, SellResourcesScreen sellResourcesScreen, GameMaster gm, Color sellColor, bool isPlayersTurn)
    {
        this.item = item;
        this.sellColor = sellColor;
        this.sellResourcesScreen = sellResourcesScreen;
        this.gm = gm;

        amountText.text = item.amount.ToString();
        goldSaleAmountText.text = item.GetPayoutForHowMuchOwned().ToString();

        topInfoText[1].text = item.payPerAmount[0].ToString();
        topInfoText[0].text = item.amountThreshold[0] + " =";

        bottomInfoText[1].text = item.payPerAmount[1].ToString();
        bottomInfoText[0].text = item.amountThreshold[1] + " =";

        tooltipContent.tooltipRect = gm.toolTipRect;
        tooltipContent.descriptionText = gm.toolTipText;
        tooltipContent.description = item.description;



        for (int i = 0; i < itemIcon.Count; i++)
        {
            itemIcon[i].sprite = item.iconImage;

        }
        for (int i = 0; i < backgroundImage.Count; i++)
        {
            backgroundImage[i].color = gm.GetColorFromRarity(item.itemRarity);
        }

        if (!isPlayersTurn)
        {
            sellButton.SetActive(false);
        }
    }


    public void SellButton()
    {
        //Remove from inventory
        gm.currentPlayersTurn.SellItem(item);

        //Change color
        for (int i = 0; i < darkOverlayImage.Count; i++)
        {
            darkOverlayImage[i].gameObject.SetActive(true);
            backgroundImage[i].color = sellColor;
        }

        //Disable Sell Button
        sellButton.SetActive(false);
    }
}
