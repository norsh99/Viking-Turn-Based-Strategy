using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellResourcesScreen : MonoBehaviour
{

    [SerializeField] private GameMaster gm;
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private GameObject noResourcesText;
    [SerializeField] private TextMeshProUGUI bannerText;

    [SerializeField] private Transform uiLoadInTransform;
    private List<GameObject> allSellBars;

    //SELL COLOR
    public Color sellColor;

    public void LoadInfo(Player player, bool isPlayersTurn)
    {
        List<Item> allResources = player.GetResources();
        gm.TurnOnShipOutline(false);

        bannerText.text = player.playerName + " Resources";


        if (allSellBars != null)
        {
            for (int i = 0; i < allSellBars.Count; i++)
            {
                Destroy(allSellBars[i]);
            }
        }
        else
        {
            allSellBars = new List<GameObject>();
        }
        allSellBars.Clear();



        //LOADING IN SELL BARS
        for (int i = 0; i < allResources.Count; i++)
        {

            GameObject sellBar = Instantiate(barPrefab);
            sellBar.transform.SetParent(uiLoadInTransform, false);

            sellBar.GetComponent<UI_SellBar>().LoadInfo(allResources[i], this, gm, sellColor, isPlayersTurn);

            allSellBars.Add(sellBar);
        }
        if (allResources.Count == 0)
        {
            noResourcesText.SetActive(true);
        }
        else
        {
            noResourcesText.SetActive(false);

        }
    }


    public void ClosePanelButton()
    {
        gameObject.SetActive(false);
        gm.UILower.canClickActionCard = true;
        gm.TurnOnShipOutline(true);

    }
}
