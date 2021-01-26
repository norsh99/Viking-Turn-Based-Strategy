
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Michsky.UI.ModernUIPack;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private Color greyOutColor;
    [SerializeField] private Color whiteOutColor;



    [SerializeField] private Image borderHighlight;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image shipImage;
    [SerializeField] private GameObject deadImage;

    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private TooltipContent tooltipContent;

    [SerializeField] private List<Image> healthBarCannonImages;
    [SerializeField] private List<Image> traitImages;

    public Unit unit;
    public GameMaster gm;

    //SHAREABLE Vals
    public string numText;
    public int health;
    public int maxHealth;

    public bool isActivated;
    public Color selectedTeamColor;
    public PlayerCard refPlayerCard;


    public void UpdateCardWithOtherPlayerCard(PlayerCard playerCard)
    {
        LoadInfo(playerCard.unit, playerCard.gm);
        SetHasBeenActivated(playerCard.isActivated);
        refPlayerCard = playerCard;
    }

    [Button("Refresh Player Card")]
    public void LoadInfo(Unit unit, GameMaster gm)
    {

        selectedTeamColor = unit.playerOwner.teamColor;
        numText = unit.unitName;
        this.gm = gm;
        this.unit = unit;
        isActivated = false;
        health = unit.hitPoints;
        this.maxHealth = unit.maxHitPoints;
        if (health > healthBarCannonImages.Count)
        {
            health = healthBarCannonImages.Count;
        }
        ChangeColorTheme(selectedTeamColor);
        InitialHealthSetup(maxHealth, selectedTeamColor);
        number.text = numText;

        //TRAITS
        SetAllUITrait();
        SetTraitColor(selectedTeamColor);



        tooltipContent.description = unit.playerOwner.playerName + " Ship " + unit.unitName;
    }

    [Button("Set Has Been Activated")]
    public void SetHasBeenActivated(bool hasBeenActivated)
    {
        isActivated = hasBeenActivated;
        if (!hasBeenActivated)
        {
            ResetColorTheme();
        }
        else
        {
            GreyOutPlayerCard();
        }
    }

    [Button("Set Unit Health")]
    public void UpdateHealth(int amount)
    {
        health = amount;
        SetUnitHealth();
    }


    public void SetDead()
    {
        deadImage.SetActive(true);
    }

    private void GreyOutPlayerCard()
    {
        borderHighlight.color = greyOutColor;
        backgroundImage.color = whiteOutColor;
        shipImage.color = selectedTeamColor;
        Color aColor = whiteOutColor;
        aColor.a = 0.1f;
        number.color = aColor;

        SetUnitHealth();

    }
    private void ResetColorTheme()
    {
        ChangeColorTheme(selectedTeamColor);
        SetUnitHealth();
    }

    

    private void SetUnitHealth()
    {
        if (isActivated)
        {
            SetPlayerHealthColor(health, whiteOutColor);
            SetTraitColor(whiteOutColor);
        }
        else
        {
            SetPlayerHealthColor(health, selectedTeamColor);
            SetTraitColor(selectedTeamColor);
        }
    }

    private void ChangeColorTheme(Color playerColor)
    {
        borderHighlight.color = playerColor;
        backgroundImage.color = playerColor;
        shipImage.color = playerColor;
        Color aColor = playerColor;
        aColor.a = 0.1f;
        number.color = aColor;

        SetTraitColor(playerColor);

    }

    private void SetPlayerHealthColor(int healthAmount, Color colorHealth)
    {
        for (int i = 0; i < healthBarCannonImages.Count; i++)
        {
            if (i < healthAmount)
            {
                healthBarCannonImages[i].color = colorHealth;
            }
            else
            {
                healthBarCannonImages[i].color = greyOutColor;

            }
        }
    }

    private void InitialHealthSetup(int amount, Color teamColor)
    {
        for (int i = 0; i < healthBarCannonImages.Count; i++)
        {
            if (i < amount)
            {
                healthBarCannonImages[i].gameObject.SetActive(true);
            }
            else
            {
                healthBarCannonImages[i].gameObject.SetActive(false);

            }
            healthBarCannonImages[i].color = teamColor;
        }
    }

    public void SetHasStartedTurn()
    {
        borderHighlight.color = Color.white;
    }

    private void SetAllUITrait()
    {
        for (int i = 0; i < traitImages.Count; i++)
        {
            if (i < unit.ship.allTraits.Count)
            {
                traitImages[i].sprite = unit.ship.allTraits[i].icon;
                traitImages[i].gameObject.SetActive(true);
            }
            else
            {
                traitImages[i].gameObject.SetActive(false);

            }

        }
    }

    private void SetTraitColor(Color setColor)
    {
        for (int i = 0; i < traitImages.Count; i++)
        {
            traitImages[i].color = setColor;
        }
    }






    //BUTTON
    public void ClickedHexButton()
    {
        if (unit.currentHex == null)
        {
            Debug.Log("yeah its null");
            return;
        }
        gm.SetSelectedHex(unit.currentHex);
    }



}
