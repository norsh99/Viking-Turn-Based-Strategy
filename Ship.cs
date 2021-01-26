using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "ShipData", menuName = "Create Ship/ Ship Data")]
public class Ship : ScriptableObject
{
    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string shipName;

    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    [TextArea]
    public string description;

    [HorizontalGroup("Game Data", 75)]
    [PreviewField(75)]
    [HideLabel]
    public GameObject shipModel;

    [VerticalGroup("Game Data/Stats")]
    public int hitPoints;
    [VerticalGroup("Game Data/Stats")]
    public int maxHitPoints;
    [VerticalGroup("Game Data/Stats")]
    public int movementDistance;
    [VerticalGroup("Game Data/Stats")]
    public int cannonAmount;
    [VerticalGroup("Game Data/Stats")]
    public float cannonAccuracyBonus;

    [BoxGroup("Abilities")]
    public List<Ability> allAbilities;


    [BoxGroup("Traits")]
    public List<Trait> allTraits;






    public Ship(Ship ship)
    {
        shipName = ship.shipName;
        description = ship.description;
        shipModel = ship.shipModel;
        hitPoints = ship.hitPoints;
        maxHitPoints = ship.maxHitPoints;
        movementDistance = ship.movementDistance;
        cannonAmount = ship.cannonAmount;
        cannonAccuracyBonus = ship.cannonAccuracyBonus;
        allAbilities = ship.allAbilities;
        allTraits = ship.allTraits;
    }
}
