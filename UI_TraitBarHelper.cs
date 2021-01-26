using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_TraitBarHelper : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image icon;


    private Trait currentTrait;

    public void LoadInfo(Trait trait)
    {
        currentTrait = trait;

        titleText.text = trait.title;
        descriptionText.text = trait.description;
        icon.sprite = trait.icon;


    }



}
