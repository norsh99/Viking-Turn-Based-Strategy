using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Michsky.UI.ModernUIPack;

public class ActionCard : MonoBehaviour
{
    private GameMaster gm;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI APText;
    [SerializeField] private Image iconImage;

    [SerializeField] private Image border;
    [SerializeField] private GameObject apIcon;

    [SerializeField] private List<Color> selectionColors;
    [SerializeField] private TooltipContent tooltipContent;


    private Ability ability;
    public int cost;

    private bool isSelected;
    public void LoadInfo(Ability ability, GameMaster gm)
    {
        this.ability = ability;
        this.gm = gm;

        titleText.text = ability.title;
        cost = ability.cost;
        iconImage.sprite = ability.icon;


        if (cost > 0)
        {
            APText.text = cost.ToString();

        }
        else
        {
            APText.text = "";
            apIcon.SetActive(false);
        }

        tooltipContent.description = ability.title + "\n" + ability.description;

        ability.Initialize(gm);
    }


    public void ColorBorderIfCantPurchase(bool colorRed)
    {
        if (colorRed)
        {
            border.color = selectionColors[2];

        }
        else
        {
            border.color = selectionColors[0];
        }
    }

    public void TriggerAbilityON()
    {
        gm.UILower.currentSelectedActionCard = this;

        ability.TriggerAbilityON();
        border.color = selectionColors[1];
        isSelected = true;
    }
    public void TriggerAbilityOFF()
    {
        isSelected = false;
        ability.TriggerAbilityOFF();
        gm.UILower.canClickActionCard = true;
        gm.UILower.RefreshActionCards();
    }
    public void ButtonPress()
    {
        if (gm.turnActionPoints >= cost)
        {
            if (!isSelected && gm.UILower.canClickActionCard)
            {
                TriggerAbilityON();
                gm.UILower.canClickActionCard = false;
            }
            else if(gm.UILower.currentSelectedActionCard == this)
            {
                TriggerAbilityOFF();
            }
        }
        else
        {
            //Display reason why cant click
        }
        
    }


}

