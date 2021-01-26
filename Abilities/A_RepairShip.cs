using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Repair Ship")]
public class A_RepairShip : Ability
{

    private GameMaster gm;
    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityOFF()
    {

    }

    public override void TriggerAbilityON()
    {
        gm.OpenRepairScreen();
    }
}
