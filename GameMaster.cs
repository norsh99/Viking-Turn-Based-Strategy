using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;
using StylizedWater;
using TMPro;

public class GameMaster : MonoBehaviour
{
    //SPAWNERS
    [SerializeField] HexSpawner hexSpawner;
    [SerializeField] UnitSpawner unitSpawner;


    //UI
    [SerializeField] public UILowerBar UILower;
    [SerializeField] private UITopBar UITopBar;
    [SerializeField] private IntiativeScreen initiativeScreen;
    [SerializeField] private List<UI_Player> uiPlayers;
    [SerializeField] private NewLootPopUp newLootPopUp;
    [SerializeField] public SellResourcesScreen resourcesScreen;
    [SerializeField] public UpgradeScreen upgradeScreen;
    [SerializeField] public WinnerScreen winningScreen;
    [SerializeField] public RepairScreen repairScreen;




    //PREFABS
    [SerializeField] List<Ship> allPlayerShips1;
    [SerializeField] List<Ship> allPlayerShips2;

    [SerializeField] private GameObject cannonBall;
    [SerializeField] private GameObject building;




    List<Hex> prevPathFindingList;

    //PLAYER
    [OdinSerialize, ReadOnly] public List<Player> allPlayers;
    [OdinSerialize, ReadOnly] public List<PlayerCard> allPlayerCards;
    [OdinSerialize, ReadOnly] public List<Unit> allUnits;
    [OdinSerialize] public List<Color> teamColors;




    private List<Hex> highlightedHexes;

    [OdinSerialize, ReadOnly] public Player currentPlayersTurn;

    [OdinSerialize, ReadOnly] public List<Unit> allActivatedUnits;
    [OdinSerialize, ReadOnly] public List<Unit> allAliveUnits;




    //Selectuon
    [OdinSerialize, ReadOnly] public SelectionStatus selectionStatus;
    [OdinSerialize, ReadOnly] public Hex selectedHex;
    [OdinSerialize, ReadOnly] public Hex prevSelectedHex { get; private set; }
    [SerializeField] private Outliner shipOutline;


    //CURRENT TURN VARS
    [OdinSerialize, ReadOnly] public int turnActionPoints;
    [OdinSerialize, ReadOnly] public Unit unitStartedTurn; //When a unit takes an action no other units can be selected and take their actions in a given turn. Reset to null at endTurn()

    //SHOOTING
    public LayerMask layerMask;
    public LayerMask planerReflections;
    private List<Unit> allUnitsThatCanShoot;
    [SerializeField] private List<GameObject> circleGOs;
    private List<Hex> canClickToShootHexs;
    private int shootingAccuracy;
    private float accuracyMod;



    //BUILDINGS
    [SerializeField] private Transform buildingsSpawnTransform;



    //PORTS
    private int maxPorts = 20;
    private int maxPortsPerInitiative = 5;

    public int totalPortsBuilt;



    //ISLANDS
    [OdinSerialize, ReadOnly] public List<Island> allIslands;
    public int totalIslandLand; //Collection of all the land found on islands
    private float percentToWin = 0.6f; //You need 60% to win the game
    public int neededLandToWinGame;


    //LOOT
    public List<Color> lootRarities;
    public List<Item> commonItems;
    public List<Item> unCommonItems;
    public List<Item> rareItems;
    public List<Item> veryRareItems;




    //ABILITY
    public List<Ability> allTempAbilities;


    //PLANAR REFLECTIONS
    [SerializeField] private PlanarReflections planarReflections;


    //TOOL TIP
    public GameObject toolTipRect;
    public TextMeshProUGUI toolTipText;




    //TURN RELATED
    public event Action initiativeCalled;
    public int round;



    //TRAITS
    private float stealthDistance = 35f;
    private int conquerorDistance = 2;
    private int pilliageOffset = 5;



    //SINGLETON / LOADING
    public static GameMaster current;
    public bool setupComplete;
    public LoadingStatus loadingStatus;

    public enum LoadingStatus
    {
        Default,
        ReadInPlayers,
        ResetActionPoints,
        SpawningHexes,
        CreatingIslands,
        LoadingInUnits,
        LoadingPlayerCards,
        AssigningCardsToUnits,
        BuildingPorts,
        InitializeUpgradeData,
        LoadingRefelctions,
        StartingInitiative
    }


    public void LoadShipsIn(List<Ship> playerShips1, List<Ship> playerShips2)
    {
        allPlayerShips1 = new List<Ship>(playerShips1);
        allPlayerShips2 = new List<Ship>(playerShips2);
    }

    public void Start()
    {
        current = this;
        loadingStatus = LoadingStatus.Default;

        LoadIn();
    }


    private void LoadIn()
    {
        LoadShipsIn(GameManager.instance.pShip1, GameManager.instance.pShip2);


        round = 0;

        selectionStatus = SelectionStatus.NoSelection;
        allUnitsThatCanShoot = new List<Unit>();
        loadingStatus = LoadingStatus.ReadInPlayers;
        allPlayers = ReadInPlayers();
        currentPlayersTurn = allPlayers[0];

        loadingStatus = LoadingStatus.ResetActionPoints;
        ResetActionPoints();

        loadingStatus = LoadingStatus.SpawningHexes;
        hexSpawner.SpawnHexes();

        loadingStatus = LoadingStatus.CreatingIslands;
        allIslands = GenerateAllIslands();

        loadingStatus = LoadingStatus.LoadingInUnits;
        unitSpawner.LoandInfo(hexSpawner, allPlayers[0], allPlayers[1]);

        loadingStatus = LoadingStatus.LoadingPlayerCards;
        UILower.LoadInPlayerShipCards();

        loadingStatus = LoadingStatus.AssigningCardsToUnits;
        AssignCardsToShips();

        loadingStatus = LoadingStatus.BuildingPorts;
        RandomBuildPorts();

        loadingStatus = LoadingStatus.InitializeUpgradeData;
        upgradeScreen.InitializeUpgradeData();

        loadingStatus = LoadingStatus.LoadingRefelctions;
        planarReflections.reflectionLayer = planerReflections;


        //START INITIATIVE
        loadingStatus = LoadingStatus.StartingInitiative;
        initiativeScreen.gameObject.SetActive(true);

        setupComplete = true;
    }



    private List<Player> ReadInPlayers()
    {
        List<Player> allNewPlayers = new List<Player>();
        Player player1 = new Player(allPlayerShips1, "Player1", teamColors[0], 0, uiPlayers[0], this);
        Player player2 = new Player(allPlayerShips2, "Player2", teamColors[1], 1, uiPlayers[1], this);

        allNewPlayers.Add(player1);
        allNewPlayers.Add(player2);

        return allNewPlayers;
    }

    private void AssignCardsToShips()
    {
        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].SetPlayerCard(allPlayerCards[i]);
        }
    }

    public void SetSelectedHex(Hex newSelection) 
    {
        switch (selectionStatus)
        {
            case SelectionStatus.NoSelection:
                if (newSelection.currentUnitOnHex != null)
                {
                    if (newSelection.currentUnitOnHex.GetIsActivated())
                    {
                        break;
                    }

                    if (newSelection.currentUnitOnHex.playerOwner != currentPlayersTurn)
                    {
                        break;
                    }

                    if (unitStartedTurn != null)
                    {
                        if (newSelection.currentUnitOnHex != unitStartedTurn)
                        {
                            break;
                        }
                    }


                    HexHighlight(newSelection);
                    selectionStatus = SelectionStatus.HexSelected;
                    OpenUnitActionScreen();

                }

                break;

            case SelectionStatus.HexSelected:

                if (newSelection.currentUnitOnHex == null)
                {
                    HexHighlight(selectedHex);
                    selectionStatus = SelectionStatus.NoSelection;

                    UILower.SwitchScreens(0);

                }
                else
                {
                    if (newSelection.currentUnitOnHex.GetIsActivated())
                    {
                        break;
                    }

                    if (newSelection == selectedHex)
                    {
                        selectionStatus = SelectionStatus.NoSelection;
                        UILower.SwitchScreens(0);
                        HexHighlight(newSelection);
                    }

                    else
                    {
                        if (newSelection.currentUnitOnHex.playerOwner != currentPlayersTurn)
                        {
                            break;
                        }


                        if (unitStartedTurn == null)
                        {
                            HexHighlight(newSelection);
                            UILower.selectedPlayerCard.UpdateCardWithOtherPlayerCard(selectedHex.currentUnitOnHex.playerCard);
                            UILower.RefreshActionCards();
                        }
                        
                    }
                }

                break;
            case SelectionStatus.MoveSelActivated:
                if (highlightedHexes.Contains(newSelection))
                {
                    MoveUnit(selectedHex, newSelection);
                    RadialHighlight_ON_OFF(false);
                    newSelection.currentUnitOnHex = selectedHex.currentUnitOnHex;
                    selectedHex.currentUnitOnHex = null;
                    newSelection.SelectOn();
                    selectedHex.SelectOff();
                    prevSelectedHex = selectedHex;
                    selectedHex = newSelection;
                    selectionStatus = SelectionStatus.PauseSelection;

                }
                break;

            case SelectionStatus.AttackSelActivted:
                if (canClickToShootHexs == null)
                {
                    canClickToShootHexs = new List<Hex>();
                }

                if (canClickToShootHexs.Contains(newSelection))
                {
                    StartCoroutine(CreateCannonBall(allUnitsThatCanShoot, newSelection.transform, .4f, shootingAccuracy, accuracyMod));
                   
                    UILower.AfterActionCardUpdate();
                }

                break;

            default:
                break;
        }
        
    }
    IEnumerator DelayAfterActionCardUpdate(float timer)
    {
        yield return new WaitForSeconds(timer);
        UILower.AfterActionCardUpdate();

    }

    private void OpenUnitActionScreen()
    {
        if (unitStartedTurn == null || unitStartedTurn == selectedHex.currentUnitOnHex)
        {
            UILower.SwitchScreens(1);
        }
    }

    private void HexHighlight(Hex newSelection)
    {
        prevSelectedHex = selectedHex;
        selectedHex = newSelection;

        selectedHex.ToggleSelect();
        selectedHex.currentUnitOnHex.RefreshAbilites();

        //UILower.TurnOnOff(selectedHex.selected);
        if (newSelection == prevSelectedHex)
        {
            selectedHex = null;
            prevSelectedHex = null;
        }



        if (prevSelectedHex != newSelection)
        {
            if (prevSelectedHex != null)
            {
                prevSelectedHex.ToggleSelect();
            }
        }
    }

    private List<Hex> PathFindBetweenTwoHexes(Hex startHex, Hex finalHex)
    {
        Pathfinding newPathfinding = new Pathfinding(startHex, finalHex);
        prevPathFindingList = newPathfinding.GetFinalPath();
        return prevPathFindingList;
    }

    public void PathFindCurrentandPreHexes()
    {
        if (selectedHex != null && prevSelectedHex != null)
        {
            PathFindBetweenTwoHexes(selectedHex, prevSelectedHex);
        }
    }

    public void FindNeighbors()
    {
        foreach (Hex neighbor in selectedHex.GetNeighbors())
        {
            neighbor.ToggleSelect();
        }
    }





    //MOVEMENT SECTION
    public void TurnMovementON()
    {
        Unit selectedUnit = selectedHex.currentUnitOnHex;
        selectionStatus = SelectionStatus.MoveSelActivated;

        RadialMovementHexSearch radMov = new RadialMovementHexSearch(selectedHex, selectedUnit.movementDistance, TerrainType.Land);
        highlightedHexes = radMov.GetAllSurroundingHexes();
        RadialHighlight_ON_OFF(true);
    }

    public void TurnMovementOFF()
    {
        RadialHighlight_ON_OFF(false);
        selectionStatus = SelectionStatus.HexSelected;


    }

    private void RadialHighlight_ON_OFF(bool turnOn)
    {
        for (int i = 0; i < highlightedHexes.Count; i++)
        {
            if (turnOn)
            {
                highlightedHexes[i].SelectOn();
            }
            else
            {
                highlightedHexes[i].SelectOff();

            }
        }
        if (!turnOn)
        {
            highlightedHexes.Clear();
        }

    }








    //SHOOTING SECTION
    public void TurnAttackON()
    {
        selectionStatus = SelectionStatus.AttackSelActivted;
        allUnitsThatCanShoot.Clear();
        allUnitsThatCanShoot.Add(selectedHex.currentUnitOnHex);
        HighlightAvailableEnemies(true, 1 - selectedHex.currentUnitOnHex.cannonAccuracyBonus);
        shootingAccuracy = 2;
        accuracyMod = 1;
    }
    public void TurnAttackOFF()
    {
        selectionStatus = SelectionStatus.HexSelected;
        HighlightAvailableEnemies(false);
        HideRadiusOfDamage();
    }

    public void MoveUnit(Hex startHex, Hex endHex)
    {
        List<Hex> path = PathFindBetweenTwoHexes(startHex, endHex);

        //startHex.currentUnitOnHex.MoveDoPath(path);
        Unit unit = startHex.currentUnitOnHex;
        unit.LoadInNewPath(path);
    }
    
    IEnumerator CreateCannonBall(List<Unit> launchUnits, Transform destinationTransfrom, float shootDelay, int accuracy, float accuracyMod) //Accuracy: The higher the accuracy the better chance at hitting your target. AccuracyMod: 1 is normal .1 subracted makes teh radius smaller
    {

        for (int i = 0; i < launchUnits.Count; i++)
        {
            for (int m = 0; m < launchUnits[i].cannonAmount; m++)
            {
                yield return new WaitForSeconds(shootDelay);
                accuracyMod = 1 - launchUnits[i].cannonAccuracyBonus;

                ShootCannonBallv2(launchUnits[i].currentHex.transform, destinationTransfrom, accuracy, accuracyMod);
            }
        }
        

    }

    private void ShootCannonBallv2(Transform launchTransform, Transform destinationTransfrom, int accuracy, float accuracyMod)
    {
        GameObject newCannonBall = Instantiate(cannonBall);
        float dist = Vector3.Distance(launchTransform.position, destinationTransfrom.position) * accuracyMod;
        newCannonBall.GetComponent<Cannon>().CannonLaunch(launchTransform, DecideBombLocation(destinationTransfrom.position, dist, accuracy), currentPlayersTurn);
        newCannonBall.transform.SetParent(this.transform);
        newCannonBall.transform.rotation = Quaternion.identity;
    }

    private int RandumNum(int startNum, int endNum)
    {
        return UnityEngine.Random.Range(startNum, endNum + 1);
    }
    private float AccuracyCalc(int accuracy, float maxNum)
    {
        float totalAccuracy = maxNum;
        for (int i = 0; i < accuracy; i++)
        {
            totalAccuracy -= RandumNum(0, Mathf.FloorToInt(totalAccuracy));
        }
        if (totalAccuracy < 0)
        {
            totalAccuracy = 0;
        }

        return totalAccuracy;
    }
    private Vector3 DecideBombLocation(Vector3 startingPos, float distance, int accuracy)
    {
        float newScaleVal = (distance / 30) * 8;
        float randomX = AccuracyCalc(accuracy, newScaleVal);
        float randomZ = AccuracyCalc(accuracy, newScaleVal);

        if (RandumNum(0, 1) == 1)
            randomX  = randomX * -1;

        if (RandumNum(0, 1) == 1)
            randomZ = randomZ * -1;

        Vector3 newPos = new Vector3(startingPos.x + randomX, startingPos.y, startingPos.z + randomZ);

        return newPos;
    }
    private void HighlightAvailableEnemies(bool enable, float accuracyMod = 1)
    {
        if (canClickToShootHexs != null)
            canClickToShootHexs.Clear();
        for (int i = 0; i < allAliveUnits.Count; i++)
        {
            if (enable)
            {
                if (allAliveUnits[i].playerOwner != currentPlayersTurn)
                {
                    if (!Physics.Linecast(selectedHex.gameObject.transform.position, allAliveUnits[i].currentHex.transform.position, layerMask))
                    {
                        float dist = Vector3.Distance(selectedHex.transform.position, allAliveUnits[i].currentHex.transform.position);
                        if (allAliveUnits[i].DoesContainTrait("Stealth") && dist > stealthDistance)
                        {
                            allAliveUnits[i].ActivateStealth(true);
                        }
                        else
                        {
                            //If there isn't any land in the way and no stealth
                            ShowRadiusOfDamage(i, allAliveUnits[i].currentHex.transform.position, accuracyMod);
                            AddToCanShootList(allAliveUnits[i].currentHex);
                        }
                    }
                }
            }
            else
            {
                allAliveUnits[i].ActivateStealth(false);
            }
        }
    }

    private void AddToCanShootList(Hex addHex)
    {
        if (canClickToShootHexs == null)
        {
            canClickToShootHexs = new List<Hex>();
        }

        canClickToShootHexs.Add(addHex);

    }

    private void ShowRadiusOfDamage(int iteration, Vector3 position, float accuracyMod)
    {
        GameObject circle = circleGOs[iteration];

        float dist = Vector3.Distance(selectedHex.transform.position, position) * accuracyMod;

        //Position
        circle.transform.position = position;

        //Scale
        float scaleVal = dist / 30f;
        Vector3 prevScale = new Vector3(1,1,1);
        circle.transform.localScale = new Vector3(0, 0, 0);
        circle.transform.DOScale(prevScale * scaleVal, 1);



        circle.SetActive(true);
    }
    private void HideRadiusOfDamage()
    {
        for (int i = 0; i < circleGOs.Count; i++)
        {
            circleGOs[i].SetActive(false);
        }
    }


    //COMINE FIRE
    public void TurnCombineFireOn(List<Unit> nearbyUnits)
    {
        selectionStatus = SelectionStatus.AttackSelActivted;
        allUnitsThatCanShoot.Clear();
        allUnitsThatCanShoot = nearbyUnits;
        allUnitsThatCanShoot.Insert(0, selectedHex.currentUnitOnHex);
        shootingAccuracy = allUnitsThatCanShoot.Count + 1;
        accuracyMod = 1 - (.1f * nearbyUnits.Count);
        accuracyMod -= selectedHex.currentUnitOnHex.cannonAccuracyBonus;
        HighlightAvailableEnemies(true, accuracyMod);
    }
    public void TurnOffCombineFire()
    {
        selectionStatus = SelectionStatus.HexSelected;
        HighlightAvailableEnemies(false);
        HideRadiusOfDamage();
        shootingAccuracy = 2;
    }








    //BUILDING

    public void CreateBuilding(Hex spawnHex)
    {
        GameObject buidingObject = Instantiate(building, buildingsSpawnTransform);
        buidingObject.transform.position = spawnHex.transform.position + new Vector3(0,1,0);
        buidingObject.transform.rotation = Quaternion.identity;

        Building buildingComp = buidingObject.GetComponent<Building>();
        buildingComp.Instantiate(this);
        spawnHex.building = buildingComp;
    }









    //PORTS
    private void RandomBuildPorts()
    {
        //Randomly talk to islands and see if there is room to build a port there. If so then build on that island, else move on to the next random island.
        List<Island> randomListOfIslands = new List<Island>(allIslands);
        int maxPortsBuiltSoFar = maxPortsPerInitiative;
        while (true)
        {
            //IF MAX PORTS HAVE BEEN BUILT OR EXHAUSTED THE LIST, BREAK OUT OF LOOP
            if (maxPortsBuiltSoFar == 0 || randomListOfIslands.Count == 0 || totalPortsBuilt >= maxPorts)
            {
                break;
            }

            //RANDOM SELECT ISLAND
            int randumNum = RandumNum(0, randomListOfIslands.Count-1);
            if (randomListOfIslands[randumNum].GetHowManyMorePortsCanBeBuiltOnIsland() > 0)
            {

                //RANDOM SELECT HEX ON ISLAND
                int randumNumHex = RandumNum(0, randomListOfIslands[randumNum].allLandNextToWaterHexes.Count-1);
                Hex randHex = randomListOfIslands[randumNum].allLandNextToWaterHexes[randumNumHex];

                //IF HEX HAS NO BUILDING
                if (randHex.building.hasPort == false)
                {
                    randHex.building.BuildPort();
                    maxPortsBuiltSoFar -= 1;
                }
            }
            randomListOfIslands.Remove(randomListOfIslands[randumNum]);
        }

    }





    //FLAGS

    private void CreateFlagsOnHexes_EndTurn()
    {
        List<Hex> neighboringHexes = selectedHex.GetNeighbors();

        if (selectedHex.currentUnitOnHex.DoesContainTrait("Conqueror"))
        {
            RadialMovementHexSearch radMov = new RadialMovementHexSearch(selectedHex, conquerorDistance, TerrainType.Water);
            neighboringHexes = radMov.GetAllSurroundingHexes();
        }

        Hex newFlaggedHex = null;
        for (int i = 0; i < neighboringHexes.Count; i++)
        {
            if (neighboringHexes[i].terrainType == TerrainType.Land)
            {



                //IF HEX ISN't TAKEN
                if (neighboringHexes[i].building.hasFlag == false)
                {
                    neighboringHexes[i].building.BuildFlag(currentPlayersTurn);
                    currentPlayersTurn.UpdateFlaggedLand(1);
                }
                //IF TAKEN BUT OWNED BY THE OPPONENT
                else if(neighboringHexes[i].building.ownerPlayer != currentPlayersTurn)
                {
                    //Remove flag points from opposing player 
                    neighboringHexes[i].building.ownerPlayer.UpdateFlaggedLand(-1);
                    neighboringHexes[i].building.BuildFlag(currentPlayersTurn);
                    neighboringHexes[i].building.ownerPlayer.UpdateFlaggedLand(1);

                }
                newFlaggedHex = neighboringHexes[i];
            }
        }

        if (newFlaggedHex != null)
        {
            newFlaggedHex.island.UpdateCount();
        }
    }



    //ISLANDS
    private List<Island> GenerateAllIslands()
    {
        int totalLands = 0;
        List<Island> allIslands = new List<Island>();
        for (int i = 0; i < hexSpawner.hexes.Count; i++)
        {
            for (int m = 0; m < hexSpawner.hexes[i].Count; m++)
            {
                if (hexSpawner.hexes[i][m].terrainType == TerrainType.Land && hexSpawner.hexes[i][m].island == null)
                {
                    Island newIsland = CreateIsland(hexSpawner.hexes[i][m]);
                    totalLands += newIsland.allLandNextToWaterHexes.Count;
                    allIslands.Add(newIsland);
                }
            }
        }


        totalIslandLand += totalLands; //Collection of all the land found on islands
        neededLandToWinGame = Mathf.FloorToInt((float)totalLands * percentToWin);
        //Debug.Log("Needed Land to Win: " + neededLandToWinGame);
        return allIslands;
    }
    private Island CreateIsland(Hex startHex)
    {
        Island island = new Island();
        List<Hex> allIslandHexes = new List<Hex>();


        //Find all neighboring hexes till there are not to form an island
        List<Hex> openHexes = new List<Hex>();

        openHexes.Add(startHex);
        while(true)
        {


            Hex currentHex = openHexes[0];
            openHexes.RemoveAt(0);
            allIslandHexes.Add(currentHex);
            currentHex.island = island;

            foreach (Hex neighbor in currentHex.GetNeighbors())
            {
                if (!allIslandHexes.Contains(neighbor) && !openHexes.Contains(neighbor))
                {
                    if (neighbor.terrainType == TerrainType.Land)
                    {
                        openHexes.Add(neighbor);
                    }
                }
            }

            if (openHexes.Count == 0)
            {
                break;
            }
        }
        island.LoadInfo(allIslandHexes, allPlayers.Count, this);
        return island;
    }



    //ITEMS - LOOT


    public Color GetColorFromRarity(ItemRarity itemRarity)
    {
        switch (itemRarity)
        {
            case ItemRarity.Gold:
                return lootRarities[0];

            case ItemRarity.Common:
                return lootRarities[1];

            case ItemRarity.UnCommon:
                return lootRarities[2];

            case ItemRarity.Rare:
                return lootRarities[3];

            case ItemRarity.VeryRare:
                return lootRarities[4];

            default:
                return lootRarities[1];
        }
    }

    private ItemRarity RaityTable()
    {
        int randNum = RandumNum(0, 100);
        ItemRarity itemRare = ItemRarity.Gold;

        float offset = 0;
        if (selectedHex.currentUnitOnHex.DoesContainTrait("Pillager"))
        {
            offset = pilliageOffset;
        }


        //COMMON
        if (randNum <= 40 - offset)
        {
            itemRare = ItemRarity.Common;
        }
        //UNCOMMON
        else if(randNum <= 75 - offset)
        {
            itemRare = ItemRarity.UnCommon;
        }
        //RARE
        else if (randNum <= 90 - offset)
        {
            itemRare = ItemRarity.Rare;
        }
        //VERY RARE
        else
        {
            itemRare = ItemRarity.VeryRare;
        }
        return itemRare;
    }

    private Item GetItemFromTable(ItemRarity itemRarity)
    {
        List<Item> theList = new List<Item>();
        int amount = 1;
        switch (itemRarity)
        {
            case ItemRarity.Gold:
                theList.Add(commonItems[0]);
                amount = RandumNum(1, 5);
                break;
            case ItemRarity.Common:
                theList = commonItems;
                amount = RandumNum(1, 5);

                break;
            case ItemRarity.UnCommon:
                theList = unCommonItems;
                amount = RandumNum(1, 2);
                break;
            case ItemRarity.Rare:
                theList = rareItems;
                break;
            case ItemRarity.VeryRare:
                theList = veryRareItems;
                break;
            default:
                break;
        }
        int randNum = RandumNum(0, theList.Count-1);
        Item newItem = new Item(theList[randNum], amount);
        return newItem;
    }
    private List<Item> GenerateNewItems(int amount)
    {
        List<Item> listOfItems = new List<Item>();
        for (int i = 0; i < amount; i++)
        {
            Item item = GetItemFromTable(RaityTable());
            int itemInList = FindItemInListReturnIndex(listOfItems, item.itemName);
            if (itemInList >= 0)
            {
                //Consolidate item with existing item
                listOfItems[itemInList].amount += item.amount;
            }
            else
            {
                listOfItems.Add(item);
            }
        }
        return listOfItems;
    }
    public void ActivateNewLootPopUp(Building building)
    {
        newLootPopUp.gameObject.SetActive(true);
        TurnOnShipOutline(false);
        newLootPopUp.LoadInfo(GenerateNewItems(building.port.lootObject.GetLootPayout()));
        UILower.AfterActionCardUpdate();

        //selectedHex.currentUnitOnHex.AfterMovingOrUseAbilityTrigger();

        building.DestroyPort();
        UILower.RefreshActionCards();
    }

    public int FindItemInListReturnIndex(List<Item> theList, string itemName)
    {
        for (int i = 0; i < theList.Count; i++)
        {
            if (theList[i].itemName == itemName)
            {
                return i;
            }
        }
        return -1;
    }
    public void RemoveItemFromList(List<Item> theList, string itemName)
    {
        for (int i = 0; i < theList.Count; i++)
        {
            if (theList[i].itemName == itemName)
            {
                theList.RemoveAt(i);
            }
        }
    }

    public void AddLootToPLayer(List<Item> incomingItems, Player player)
    {
        player.AddLootToPLayer(incomingItems);
    }





    //UPGRADE
    public void TurnOnUpgradeScreen(bool turnOn)
    {
        if (turnOn)
        {
            upgradeScreen.gameObject.SetActive(true);
            upgradeScreen.LoadInfo(selectedHex.currentUnitOnHex);
            TurnOnShipOutline(false);
        }
        else
        {
            UILower.RefreshActionCards();
            UILower.canClickActionCard = true;
            upgradeScreen.gameObject.SetActive(false);
            TurnOnShipOutline(true);
        }

    }



    //REPAIR
    public void OpenRepairScreen()
    {
        repairScreen.LoadInfo(currentPlayersTurn, selectedHex.currentUnitOnHex);
        TurnOnShipOutline(false);
    }




    //ABILITIES
    public Ability FindAblility(string textName)
    {
        for (int i = 0; i < allTempAbilities.Count; i++)
        {
            if (allTempAbilities[i].title == textName)
            {
                return allTempAbilities[i];
            }
        }
        return null;
    }



    //WIN STATE
    public void CheckIfWinner()
    {
        int playerCount1 = 0;
        int playerCount2 = 0;

        for (int i = 0; i < allAliveUnits.Count; i++)
        {
            if (allAliveUnits[i].playerOwner.playerNumberIndex == 0)
            {
                playerCount1 += 1;
            }
            else
            {
                playerCount2 += 2;
            }
        }

        if (playerCount1 == 0)
        {
            winningScreen.LoadInfo(allPlayers[1]);
            TurnOnShipOutline(false);
            return;
        }
        if(playerCount2 == 0)
        {
            winningScreen.LoadInfo(allPlayers[0]);
            TurnOnShipOutline(false);
            return;

        }
        if (allPlayers[0].GetLandOwned() >= neededLandToWinGame)
        {
            winningScreen.LoadInfo(allPlayers[0]);
            TurnOnShipOutline(false);
            return;

        }
        if (allPlayers[1].GetLandOwned() >= neededLandToWinGame)
        {
            winningScreen.LoadInfo(allPlayers[1]);
            TurnOnShipOutline(false);
            return;

        }
    }




    //KILL UNIT
    public void KillUnit(Unit deadUnit)
    {
        deadUnit.playerCard.SetDead();
        allAliveUnits.Remove(deadUnit);

        //Checking if unit already activated first before dying, if so remove from that list.
        for (int i = 0; i < allActivatedUnits.Count; i++)
        {
            if (allActivatedUnits[i] == deadUnit)
            {
                allActivatedUnits.RemoveAt(i);
            }
        }


        CheckIfWinner();
    }



    //OUTLINE
    public void TurnOnShipOutline(bool turnOn)
    {
        shipOutline.enabled = turnOn;
    }



    //TURN RELATED

    public void StartUnitTurn(Unit unit)
    {
        unit.playerCard.SetHasStartedTurn();
        unitStartedTurn = unit;
    }
    public void EndTurn()
    {
        //Build Flags on Neighboring Hexes
        CreateFlagsOnHexes_EndTurn();

        //Activate player
        ActivateUnit(selectedHex.currentUnitOnHex, true);
        //Transition to next player
        ChangePlayer();

        //Reset AP to player default max amount
        ResetActionPoints();


        //Check if all players have been activated, if so then go to initiative.
        CheckIfAllUnitsHaveBeenActivated();

        //Make unit that started turn null
        unitStartedTurn = null;

        selectionStatus = SelectionStatus.NoSelection;
        UILower.SwitchScreens(0);
        HexHighlight(selectedHex);


    }

    public void InitiativeCalled()
    {
        if (initiativeCalled != null)
        {
            initiativeCalled();
        }
    }

    public void AssignPlayerTurn()
    {
        UITopBar.UpdateHeader(currentPlayersTurn);
    }

    private void ChangePlayer()
    {
        if (currentPlayersTurn == allPlayers[0])
        {
            if (CheckIfOtherTeamHasUnitsToActivate(allPlayers[1]))
            {
                currentPlayersTurn = allPlayers[1];
            }
        }
        else
        {
            if (CheckIfOtherTeamHasUnitsToActivate(allPlayers[0]))
            {
                currentPlayersTurn = allPlayers[0];
            }
        }
        AssignPlayerTurn();
    }

    private bool CheckIfOtherTeamHasUnitsToActivate(Player otherPlayer)
    {
        for (int i = 0; i < allAliveUnits.Count; i++)
        {
            if (allAliveUnits[i].playerOwner == otherPlayer)
            {
                if (!allAliveUnits[i].GetIsActivated())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ActivateUnit(Unit unit, bool activate)
    {
        if (activate)
        {
            allActivatedUnits.Add(unit);

        }
        unit.IsActivated(activate);
        unit.playerCard.SetHasBeenActivated(activate);

    }

    public void ResetAllActivations()
    {
        for (int i = 0; i < allActivatedUnits.Count; i++)
        {
            ActivateUnit(allActivatedUnits[i], false);
        }
        allActivatedUnits.Clear();
    }

    private void CheckIfAllUnitsHaveBeenActivated()
    {
        if (allActivatedUnits.Count == allAliveUnits.Count)
        {
            //Activate initiative screen
            initiativeScreen.gameObject.SetActive(true);
            TurnOnShipOutline(false);
            //Spawn more ports
            RandomBuildPorts();

            round += 1;
            InitiativeCalled();
        }
    }

    //ACTION POINTS

    private void UpdateUI()
    {
        UITopBar.UpdateActionPoints(turnActionPoints.ToString());
    }
    private void ResetActionPoints()
    {
        turnActionPoints = 20;
        UpdateUI();
    }
    public void SpendActionPoints(int amount, Unit unit)
    {
        turnActionPoints = turnActionPoints - amount;
        StartUnitTurn(unit);
        UpdateUI();
    }



}


public enum SelectionStatus
{
    NoSelection,
    HexSelected,
    MoveSelActivated,
    AttackSelActivted,
    PauseSelection


}