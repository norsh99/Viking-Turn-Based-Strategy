using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UI_UpgradeBar : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI uprgadeAmountText;
    [SerializeField] private TextMeshProUGUI goldCostText;
    [SerializeField] private TextMeshProUGUI flagCostText;

    [SerializeField] private GameObject buyButtonGO;
    [SerializeField] private Image iconImage;

    private Action<Image> buyButtonAction;

    private int textAmount;
    private int textAmountLimit;

    private int goldCost;
    private int flagCost;

    private Sprite iconSprite;

    public void LoadInfo(Action<Image> buyButtonAction, int textAmount, int textAmountLimit, Sprite iconSprite, int goldCost, int flagCost)
    {
        this.buyButtonAction = buyButtonAction;
        this.textAmount = textAmount;
        this.textAmountLimit = textAmountLimit;
        this.iconSprite = iconSprite;

        this.goldCost = goldCost;
        this.flagCost = flagCost;



        RefreshBar();
    }

    public void RefreshBar()
    {
        uprgadeAmountText.text = textAmount + " of " + textAmountLimit;



        if (goldCost > 0)
        {
            goldCostText.text = goldCost.ToString();
            flagCostText.text = flagCost.ToString();
        }
        else
        {
            goldCostText.text = "-";
            flagCostText.text = "-";
        }
        

        iconImage.sprite = iconSprite;

        if (textAmount < textAmountLimit)
        {
            buyButtonGO.SetActive(true);
        }
        else
        {
            buyButtonGO.SetActive(false);
        }
    }


    //BUTTON
    public void BuyButton()
    {
        if (buyButtonAction != null)
        {
            //Debug.Log("asfag");
            buyButtonAction(buyButtonGO.GetComponent<Button>().image);
        }

        RefreshBar();
    }

}
