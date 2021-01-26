using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UI_PickShipCard : MonoBehaviour
{

    [SerializeField] private GameObject commanderField;
    [SerializeField] private List<GameObject> healthPoints;
    [SerializeField] private List<Sprite> shieldIcons;
    [SerializeField] private List<Image> icons;
    [SerializeField] private List<Ship> defaultShips;



    private int health;

    public bool isComplete;


    private Trait pickedTrait;
    private Trait pickedStance;


    public event Action changedStats;

    public void ResetStats()
    {
        health = 3;
        SetHealthPoints(health);

        pickedTrait = null;
        pickedStance = null;

        isComplete = false;
        commanderField.SetActive(false);
        TurnOffIcons();
    }

    public Ship GetFinalShip()
    {
        Ship newShip = Instantiate(new Ship(defaultShips[0]));


        if (newShip.allTraits == null)
        {
            newShip.allTraits = new List<Trait>();
        }

        newShip.allTraits.Add(pickedTrait);
        newShip.allTraits.Add(pickedStance);
        newShip.hitPoints = health;
        newShip.maxHitPoints = health;


        if (pickedStance.title == "Move")
        {
            newShip.movementDistance = 3;
        }
        if (pickedStance.title == "Attack")
        {
            newShip.cannonAccuracyBonus = .2f; //Incriment by .1f. The math is 1 is no modification and anything below 1 is a percentage of the final shooting radius. Ex: .2 is 80% radius
        }

        return newShip;
    }

    public void ChangedStats()
    {
        if (changedStats != null)
        {
            changedStats();
        }
    }


    public void SetTrait(Trait trait)
    {
        //RESET

        if (pickedTrait != null)
        {
            SetIcon(pickedTrait, false);


            if (pickedTrait.title == "Bulldog")
            {
                health -= 2;
                SetHealthPoints(health);
            }
        }



        //SET
        pickedTrait = trait;




        //UPDATE
        if (trait.title == "Bulldog")
        {
            health += 2;

            SetHealthPoints(health);
        }




        SetIcon(trait, true);
        CheckIfComplete();
        ChangedStats();
    }

    private void TurnOffIcons()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].gameObject.SetActive(false);
        }
    }

    private void SetIcon(Trait trait, bool turnOn)
    {

        if (trait.title == "Defend") {
            icons[0].gameObject.SetActive(turnOn);

            if (pickedTrait != null)
            {
                if (pickedTrait.title ==  "Bulldog")
                {
                    icons[0].gameObject.SetActive(true);
                }
            }
        }

        if (trait.title == "Bulldog")
        {
            icons[0].gameObject.SetActive(turnOn);

            if (pickedStance != null)
            {
                if (pickedStance.title == "Defend")
                {
                    icons[0].gameObject.SetActive(true);
                }
            }
        }
        SetShieldIcon(icons[0]);


        if (trait.title == "Attack")
            icons[1].gameObject.SetActive(turnOn);

        if (trait.title == "Move")
            icons[2].gameObject.SetActive(turnOn);

        if (trait.title == "Stealth")
            icons[3].gameObject.SetActive(turnOn);

        if (trait.title == "Pilliager")
            icons[4].gameObject.SetActive(turnOn);

        if (trait.title == "Conqueror")
            icons[5].gameObject.SetActive(turnOn);
    }

    private void SetShieldIcon(Image image)
    {
        if (health == 4)
        {
            image.sprite = shieldIcons[0];
        }
        else if(health == 5)
        {
            image.sprite = shieldIcons[1];
        }
        else if (health == 6)
        {
            image.sprite = shieldIcons[2];
        }
    }


    private void SetHealthPoints(int amount)
    {
        for (int i = 0; i < healthPoints.Count; i++)
        {
            if (i < amount)
            {
                healthPoints[i].SetActive(true);

            }
            else
            {
                healthPoints[i].SetActive(false);

            }

        }
    }

    public void SetStance(Trait trait)
    {

        //RESET
        if (pickedStance != null)
        {
            SetIcon(pickedStance, false);


            if (pickedStance.title == "Defend")
            {
                health -= 1;
                SetHealthPoints(health);
            }
        }




        //SET
        pickedStance = trait;




        //UPDATE
        if (trait.title == "Defend")
        {
            health += 1;

            SetHealthPoints(health);
        }



        SetIcon(trait, true);
        CheckIfComplete();
        ChangedStats();
    }

    private void CheckIfComplete()
    {
        if (pickedTrait == null || pickedStance == null)
        {
            isComplete = false;
        }
        else
        {
            isComplete = true;
        }
    }
}
