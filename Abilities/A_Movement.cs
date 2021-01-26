using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement")]
public class A_Movement : Ability
{
    private GameMaster gm;


    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityON()
    { 
        gm.TurnMovementON();
    }

    public override void TriggerAbilityOFF()
    {
        gm.TurnMovementOFF();
    }

}
