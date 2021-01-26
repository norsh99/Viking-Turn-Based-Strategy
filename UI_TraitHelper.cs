using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TraitHelper : MonoBehaviour
{

    public List<Trait> allTraits;
    public List<UI_TraitBarHelper> allTraitBars;


    private void OnEnable()
    {
        SetAllUIBars();
    }


    private void SetAllUIBars()
    {
        for (int i = 0; i < allTraitBars.Count; i++)
        {
            if (i < allTraits.Count)
            {
                allTraitBars[i].LoadInfo(allTraits[i]);
                allTraitBars[i].gameObject.SetActive(true);
            }
            else
            {
                allTraitBars[i].gameObject.SetActive(false);

            }

        }
    }


}
