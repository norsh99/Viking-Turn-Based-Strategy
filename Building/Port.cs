using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour
{
    public Building building;
    public LootableObject lootObject;


    public void LoadInfo()
    {
        lootObject.LoadInfo(building.gm);
    }
    public void LoadOut()
    {
        lootObject.LoadOut();
    }
}
