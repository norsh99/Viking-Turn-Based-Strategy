using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameMaster gm;



    //OBJECTS
    public Flag flag;
    public Port port;
    [SerializeField] private GameObject baseHexGameObject;
    [SerializeField] private GameObject explosion;



    //Class Vars
    [SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] public List<Material> allMaterials;
    [SerializeField] public List<Material> baseMaterial;

    [ReadOnly] public Player ownerPlayer;

    //HEX
    [OdinSerialize, ReadOnly] public Hex currentHex;
    public bool hasFlag;


    //PORT
    public bool hasPort;

    public void Instantiate(GameMaster gm)
    {
        this.gm = gm;
        ownerPlayer = null;
    }

    public void BuildPort()
    {
        ActivateFlag(false);
        gm.totalPortsBuilt += 1;
        hasPort = true;
        port.LoadInfo();
    }
    public void DestroyPort()
    {
        gm.totalPortsBuilt -= 1;
        hasPort = false;
        port.LoadOut();
        explosion.SetActive(true);
        StartCoroutine(GameObjectTimer(explosion, 2f));
        if (hasFlag)
        {
            ActivateFlag(true);
        }
        else
        {
            port.gameObject.SetActive(false);
            baseHexGameObject.SetActive(false);
        }
    }



    //FLAG
    public void BuildFlag(Player player)
    {
        hasFlag = true;
        ownerPlayer = player;

        if (!hasPort)
        {
            ActivateFlag(true);
        }
        flag.UpdateColor(player.playerNumberIndex);
        UpdateBaseColor(player.playerNumberIndex);
    }

    public void ActivateFlag(bool activateFlag)
    {
        flag.gameObject.SetActive(activateFlag);
        port.gameObject.SetActive(!activateFlag);
        baseHexGameObject.SetActive(!activateFlag);
    }






    //EXPLOSION
    IEnumerator GameObjectTimer(GameObject GO, float timer)
    {
        yield return new WaitForSeconds(timer);
        GO.SetActive(false);
    }
    

    public void UpdateBaseColor(int playerIndex)
    {
        if (playerIndex == 1)
        {
            baseMeshRenderer.material = baseMaterial[0];
            baseMeshRenderer.material.color = ownerPlayer.teamColor;
        }
        else
        {
            baseMeshRenderer.material = baseMaterial[1];
            baseMeshRenderer.material.color = ownerPlayer.teamColor;
        }
    }

}
