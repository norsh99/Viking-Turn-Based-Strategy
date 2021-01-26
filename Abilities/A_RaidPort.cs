using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Raid Port")]
public class A_RaidPort : Ability
{
    private GameMaster gm;
    private Hex portOnHex;
    private Unit unitThatCanUseThisAbility;

    public A_RaidPort(Ability a_RaidPort, GameMaster gm)
    {
        icon = a_RaidPort.icon;
        cost = a_RaidPort.cost;
        title = a_RaidPort.title;
        description = a_RaidPort.description;
        this.gm = gm;
    }

    public void AddHexAndUnit(Hex portOnHex, Unit unitThatCanUseThisAbility)
    {
        this.portOnHex = portOnHex;
        this.unitThatCanUseThisAbility = unitThatCanUseThisAbility;
    }

    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityOFF()
    {

    }

    public override void TriggerAbilityON()
    {
        gm.ActivateNewLootPopUp(portOnHex.building);
        unitThatCanUseThisAbility.AfterMovingOrUseAbilityTrigger();

    }
}
