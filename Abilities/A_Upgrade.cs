using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Upgrade")]
public class A_Upgrade : Ability
{

    private GameMaster gm;
    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityOFF()
    {
        gm.TurnOnUpgradeScreen(false);
    }

    public override void TriggerAbilityON()
    {
        gm.TurnOnUpgradeScreen(true);

    }
}
