using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> resourceTextFields; //Gold, Flags, Wood, Resources

    [SerializeField] ProgressBar islandCounterBar;
    [SerializeField] Image progressBarImage;
    [SerializeField] Image gradientImage;




    private Player player;
    private GameMaster gm;

    public void LoadInfo(Player player, GameMaster gm)
    {
        this.player = player;
        this.gm = gm;
        progressBarImage.color = player.teamColor;
        gradientImage.color = player.teamColor;

        player.changedResources += UpdateUI;
        islandCounterBar.GetComponent<TooltipContent>().description = player.playerName + " Island hexes needed to win. \n" + player.GetLandOwned() + "/" + gm.neededLandToWinGame;

    }




    private void UpdateUI()
    {
        resourceTextFields[0].text = player.resources[0].ToString();
        resourceTextFields[1].text = player.resources[1].ToString();
        resourceTextFields[2].text = player.resources[2].ToString();

        resourceTextFields[3].text = player.GetTotalAmountOfResources().ToString();


        islandCounterBar.currentPercent = player.GetLandOwned();
        islandCounterBar.maxValue = gm.neededLandToWinGame;

        islandCounterBar.GetComponent<TooltipContent>().description = player.playerName + " Island hexes needed to win. \n" + player.GetLandOwned() + "/" + gm.neededLandToWinGame;
    }
}
