using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class UI_LootItem : MonoBehaviour
{

    public TextMeshProUGUI amount;
    public GameObject amountGameObject;
    public Image icon;

    [SerializeField] private Image background;
    [SerializeField] private TooltipContent tooltipContent;


    public void LoadInfo(Item item, Color colorItem)
    {

        if (item.amount > 1)
        {
            amountGameObject.SetActive(true);
            amount.text = item.amount.ToString();
        }
        else
        {
            amountGameObject.SetActive(false);
        }

        tooltipContent.description = item.description;

        icon.sprite = item.iconImage;
        background.color = colorItem;
    }





}
