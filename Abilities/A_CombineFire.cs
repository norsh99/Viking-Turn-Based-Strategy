using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Combine Fire")]
public class A_CombineFire : Ability
{
    private GameMaster gm;
    public List<Unit> nearbyFriendlyUnits;


    public A_CombineFire(Ability a_combineFire, List<Unit> nearbyFriendlyUnits, GameMaster gm)
    {

        icon = a_combineFire.icon;
        cost = a_combineFire.cost;
        title = a_combineFire.title;
        description = a_combineFire.description;

        this.nearbyFriendlyUnits = nearbyFriendlyUnits;
        this.gm = gm;

    }
    public override void Initialize(GameMaster gm)
    {
        this.gm = gm;
    }

    public override void TriggerAbilityOFF()
    {
        gm.TurnOffCombineFire();
    }

    public override void TriggerAbilityON()
    {
        gm.TurnCombineFireOn(nearbyFriendlyUnits);

    }

    public List<Unit> GetFriendlyNearbyUnits() { return nearbyFriendlyUnits; }
}
