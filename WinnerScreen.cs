using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinnerScreen : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI winnerPlayerText;
    [SerializeField] private GameObject goldCoins;
    public void LoadInfo(Player winningPlayer)
    {
        winnerPlayerText.text = winningPlayer.playerName;
        winnerPlayerText.color = winningPlayer.teamColor;

        gameObject.SetActive(true);
        goldCoins.SetActive(true);
    }


}
