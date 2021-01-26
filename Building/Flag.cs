using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public MeshRenderer flagMeshRenderer;
    public Building building;

    public void UpdateColor(int playerIndex)
    {
        if (playerIndex == 1)
        {
            flagMeshRenderer.material = building.allMaterials[0];

        }
        else
        {
            flagMeshRenderer.material = building.allMaterials[1];

        }

        flagMeshRenderer.sharedMaterial.SetColor(
            name: $"_MainColor",
            value: building.ownerPlayer.teamColor
            );
    }
}
