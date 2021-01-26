using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : MonoBehaviour
{
    private GameMaster gm;


    private int lootLevel;

    public void LoadInfo(GameMaster gm)
    {
        this.gm = gm;
        lootLevel = 1;

        gm.initiativeCalled += UpdateCounted;

    }


    public int GetLootPayout()
    {
        return 3 * lootLevel;
    }

    public void LoadOut()
    {
        gm.initiativeCalled -= UpdateCounted;

    }

    private void UpdateCounted()
    {
        if (lootLevel < 3)
        {
            lootLevel += 1;
        }
    }


}
