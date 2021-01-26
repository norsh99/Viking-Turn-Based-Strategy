using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cannon Attack")]
public class A_CannonAttack : Ability
{
    private GameMaster gm;

    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityOFF()
    {
        gm.TurnAttackOFF();

    }

    public override void TriggerAbilityON()
    {
        gm.TurnAttackON();
    }
}
