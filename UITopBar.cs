using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITopBar : MonoBehaviour
{
    [SerializeField] private GameMaster gm;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI actionPointsText;




    public void UpdateHeader(Player player)
    {
        headerText.color = player.teamColor;
        headerText.text = player.playerName;
    }

    public void UpdateActionPoints(string textPoints)
    {
        actionPointsText.text = textPoints;
    }

    public void ResourcesButton(int num)
    {
        if (gm.currentPlayersTurn.playerNumberIndex == num)
        {
            gm.resourcesScreen.gameObject.SetActive(true);
            gm.resourcesScreen.LoadInfo(gm.currentPlayersTurn, true);
            gm.UILower.canClickActionCard = false;
        }
        else
        {
            gm.resourcesScreen.gameObject.SetActive(true);
            gm.resourcesScreen.LoadInfo(gm.allPlayers[num], false);
            gm.UILower.canClickActionCard = false;
        }
        
    }

}
