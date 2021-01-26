using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class InitiativeScreenBar : MonoBehaviour
{

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private Image progressBarImage;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI goldSpentText;

    private float animateGold;


    private bool runUpdate;

    public void LoadInfo(Player player, int goldSpent, int maxGoldSpent, float timer)
    {
        runUpdate = true;


        playerNameText.text = player.playerName;
        playerNameText.color = player.teamColor;

        goldSpentText.text = "";


        progressBar.currentPercent = 0;
        progressBar.maxValue = maxGoldSpent;

        progressBarImage.color = player.teamColor;
        progressBarImage.gameObject.transform.localPosition = new Vector3(0,0,0);
        animateGold = 0;


        DOTween.To(() => animateGold, x => animateGold = x, goldSpent, timer).SetEase(Ease.InOutSine).OnComplete(AsssignGoldSpentText);
        
    }

    private void AsssignGoldSpentText()
    {
        goldSpentText.text = animateGold.ToString();
    }
    private void Update()
    {
        if (runUpdate)
        {
            progressBar.currentPercent = animateGold;
        }
    }

    public void ResetBarToZero()
    {
        runUpdate = false;
        progressBarImage.fillAmount = 0;
    }


}
