using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.Serialization;
using EPOOutline;

public class Unit : MonoBehaviour
{

    [OdinSerialize, ReadOnly] public Ship ship;
    [OdinSerialize, ReadOnly] public Player playerOwner;
    [SerializeField] private FloatingObject floatingObject;
    [SerializeField] private Rigidbody rigidbody;

    public string unitName;

    //STATS
    public int hitPoints;
    private int moveLimit = 6;
    private int cannonLimit = 8;
    private List<Trait> unitTraits;


    //UPGRADABLE STATS
    public int maxHitPoints; //Player Card graphic can't go more than 6
    public int cannonAmount;
    public int movementDistance;
    public float cannonAccuracyBonus;




    private bool isMoving;
    private bool executeMove;


    private List<Hex> movementPath;
    private int pathIndex;

    private GameMaster gm;
    public PlayerCard playerCard;
    private bool isActivated;
    [SerializeField] private GameObject deathGameObject;
    [SerializeField] private BoxCollider boxCollider;


    public Hex currentHex;



    //HIGHLIGHT COLORS
    [SerializeField] private List<Color> highlights;
    [SerializeField] private Outlinable highlightOutline;
    [SerializeField] private Outlinable stealthHighlight;



    //POP UPS
    [SerializeField] private GameObject stealthPopUp;




    //TEMP ABILITIES
    public List<Ability> tempAbilities;

    public void LoadInfo(Ship ship, GameMaster gm, Player player, Hex hex, string unitName)
    {
        this.playerOwner = player;
        this.ship = ship;
        this.unitName = unitName;
        this.maxHitPoints = ship.maxHitPoints;
        this.gm = gm;
        hitPoints = ship.hitPoints;
        this.movementDistance = ship.movementDistance;
        this.cannonAccuracyBonus = ship.cannonAccuracyBonus;
        unitTraits = ship.allTraits;
        cannonAmount = ship.cannonAmount;
        isMoving = false;
        executeMove = false;

        tempAbilities = new List<Ability>();

        currentHex = hex;

    }

    public void RefreshAbilites()
    {
        AddCombineFireAbility();
        AddRaidPortAbility(currentHex.GetLandNeighbors());
    }

    public void AfterMovingOrUseAbilityTrigger()
    {

        AddRaidPortAbility(currentHex.GetLandNeighbors());
        AddCombineFireAbility();
        gm.UILower.RefreshActionCards();
    }

    private void AddRaidPortAbility(List<Hex> nearbyLand)
    {
        Ability raidPortAbility = gm.FindAblility("Raid Port");
        A_RaidPort raidPort = new A_RaidPort(raidPortAbility, gm);


        //Clear tempAbility of the raid card
        for (int i = 0; i < tempAbilities.Count; i++)
        {
            if (tempAbilities[i].title == "Raid Port")
            {
                tempAbilities.RemoveAt(i);
            }
        }

        for (int i = 0; i < nearbyLand.Count; i++)
        {
            if (nearbyLand[i].building.hasPort)
            {
                raidPort.AddHexAndUnit(nearbyLand[i], this);
                tempAbilities.Add(raidPort);
                break;
            }
        }
    }


    public List<Ability> GetAllAvailableAbilities()
    {
        List<Ability> allAbilities = new List<Ability>(ship.allAbilities);
        for (int i = 0; i < tempAbilities.Count; i++)
        {
            allAbilities.Add(tempAbilities[i]);
        }


        return allAbilities;
    }

    private void AddCombineFireAbility()
    {
        Ability combineFireAbility = gm.FindAblility("Combine Fire");

        List<Unit> nearbyFriendlyUnits = currentHex.GetNearbyUnitNeighbors(playerOwner);

        //Clear tempAbility of the raid card
        for (int i = 0; i < tempAbilities.Count; i++)
        {
            if (tempAbilities[i].title == "Combine Fire")
            {
                tempAbilities.RemoveAt(i);
            }
        }

        if (nearbyFriendlyUnits.Count > 0)
        {
            
            A_CombineFire combineFire = new A_CombineFire(combineFireAbility, nearbyFriendlyUnits, gm);
            tempAbilities.Add(combineFire);

        }
    }


    //TRAITS
    public bool DoesContainTrait(string trait)
    {
        if (unitTraits == null)
        {
            unitTraits = new List<Trait>();
        }
        for (int i = 0; i < unitTraits.Count; i++)
        {
            if (unitTraits[i].title == trait)
            {
                return true;
            }
        }
        
        return false;
    }








    //MOVING
    public void LoadInNewPath(List<Hex> thePath)
    {
        movementPath = new List<Hex>(thePath);
        pathIndex = 1;
        movementPath[0].SetHexPlayerColor(true);
        StartCoroutine(MoveUnit());
    }
    IEnumerator MoveUnit()
    {
        yield return new WaitForSeconds(0);
        Vector3 nextHexLocation = movementPath[pathIndex].transform.position;
        float rotationWait = .3f;

        transform.DOLookAt(nextHexLocation, rotationWait, AxisConstraint.Y, Vector3.up).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0);
        transform.DOMove(new Vector3(nextHexLocation.x, transform.position.y, nextHexLocation.z), 1).SetEase(Ease.InOutSine).OnComplete(CheckIfPathComplete);

    }
    private void CheckIfPathComplete()
    {
        pathIndex += 1;
        if (pathIndex >= movementPath.Count)
        {
            //When move is finished
            movementPath[movementPath.Count-1].SetHexPlayerColor();
            currentHex = movementPath[movementPath.Count - 1];
            gm.selectedHex = movementPath[movementPath.Count - 1];
            gm.selectionStatus = SelectionStatus.HexSelected;

            gm.UILower.AfterActionCardUpdate();

            AfterMovingOrUseAbilityTrigger();
            return;
        }
        StartCoroutine(MoveUnit());
    }






    //UPGRADES
    public void UpgradeCannons()
    {
        cannonAmount += 1;
    }
    public void UpgradeMovement()
    {
        movementDistance += 1;
    }
    public int GetUpgradeMoveLimit() { return moveLimit; }
    public int GetUpgradeCannonLimit() { return cannonLimit; }








    //OTHERS
    public void SetPlayerCard(PlayerCard pc)
    {
        playerCard = pc;
    }

    public void Damage()
    {
        Debug.Log("IM HIT!");
        UpdateHitPoints(-1);
        if (hitPoints <= 0)
        {
            //Debug.Log("IM DEAD!");
            Dead();
        }
    }

    public void UpdateHitPoints(int amount)
    {
        hitPoints += amount;
        playerCard.UpdateHealth(hitPoints);
    }


    public void IsActivated(bool isActivated)
    {
        this.isActivated = isActivated;
        UpdateHexSelectionColor();
    }

    public bool GetIsActivated() { return isActivated; }






    //DEAD
    public void Dead()
    {
        floatingObject.enabled = false;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        boxCollider.enabled = false;
        transform.DOMoveY(-7, 10f).SetEase(Ease.InOutSine);
        transform.DORotate(new Vector3(20,0,0), 10f).SetEase(Ease.InOutSine).OnComplete(DestroyUnit);
        deathGameObject.SetActive(true);
        currentHex.currentUnitOnHex = null;
        currentHex.SetHexPlayerColor(true);
        GetComponent<Outlinable>().enabled = false;
        gm.KillUnit(this);

    }
    private void DestroyUnit()
    {
        gameObject.SetActive(false);
    }






    //HIGHLIGHT
    public void ActivateStealth(bool turnOn)
    {
        if (turnOn)
        {
            stealthHighlight.enabled = true;
            highlightOutline.enabled = false;
            stealthPopUp.SetActive(true);
        }
        else
        {
            stealthHighlight.enabled = false;
            stealthPopUp.SetActive(false);

            UpdateHexSelectionColor();
        }
    }


    private void UpdateHexSelectionColor()
    {
        if (isActivated)
        {
            highlightOutline.enabled = true;
            highlightOutline.OutlineParameters.Color = highlights[0];
        }
        else
        {
            highlightOutline.enabled = false;


        }
    }
}
