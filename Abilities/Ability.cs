using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite icon;
    public string title;
    public int cost;

    [LabelWidth(100)]
    [TextArea]
    public string description;


    public abstract void Initialize(GameMaster gm);
    public abstract void TriggerAbilityON();
    public abstract void TriggerAbilityOFF();

}
