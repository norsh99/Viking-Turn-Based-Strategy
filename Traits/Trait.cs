using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Default")]
public class Trait : ScriptableObject
{

    public string title;
    public int pickCost;

    


    [LabelWidth(100)]
    [TextArea]
    public string description;


    [HorizontalGroup("Icon", 75)]
    [PreviewField(75)]
    [HideLabel]
    public Sprite icon;
}
