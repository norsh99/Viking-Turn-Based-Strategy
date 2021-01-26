using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hex : SerializedMonoBehaviour
{
    [OdinSerialize] private GameObject highlighter;
    [SerializeField] private TextMeshPro textID;
    [SerializeField] private GameObject shield;
    [SerializeField] private SpriteFaceToCamera spriteFaceToCamera;



    public bool selected { get; private set; } = false;
    public bool isNotOutOfBounds;
    [OdinSerialize,ReadOnly] public Unit currentUnitOnHex { get; set; }
    [OdinSerialize, ReadOnly] public TerrainType terrainType { get; set; }

    public Hex parentHex { get; set; } //For Pathfinding

    public float depth;

    public HexSpawner hexSpawner;

    [OdinSerialize, ReadOnly] private Index hexIndex;
    [OdinSerialize, ReadOnly] List<Hex> allNeighbors;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private GameObject teamColorHighlighter;


    //BUILDING
    public Building building;

    //ISLAND
    public Island island;



    IEnumerable<(Hex neighbor, HexNeighborDirection direction)> NeighborsWithDirection()
    {
        foreach (HexNeighborDirection direction in EnumArray<HexNeighborDirection>.Values)
        {
            Hex neighbor = hexSpawner.GetNeighborAt(hexIndex, direction);
            yield return (neighbor, direction);
        }
    }

    public void AssignCamera(Camera cam)
    {
        spriteFaceToCamera.cameraToLookAt = cam;
    }

    public List<Hex> GetNeighbors()
    {
        return allNeighbors;
    }
    public List<Hex> GetLandNeighbors()
    {
        List<Hex> allLandNeighbors = new List<Hex>();
        for (int i = 0; i < allNeighbors.Count; i++)
        {
            if (allNeighbors[i].terrainType == TerrainType.Land)
            {
                allLandNeighbors.Add(allNeighbors[i]);
            }
            
        }
        return allLandNeighbors;
    }

    public List<Unit> GetNearbyUnitNeighbors(Player playerOwner)
    {
        List<Unit> newList = new List<Unit>();
        for (int i = 0; i < GetNeighbors().Count; i++)
        {
            if (GetNeighbors()[i].currentUnitOnHex != null)
            {
                if (GetNeighbors()[i].currentUnitOnHex.playerOwner == playerOwner)
                {
                    newList.Add(GetNeighbors()[i].currentUnitOnHex);
                }
            }
        }
        return newList;
    }


    //private void Awake() => hexSpawner = GameObject.FindObjectOfType<HexSpawner>();

    public void AssignIndex(Index index) => hexIndex = index;

    public void ToggleSelect() => (selected ? (Action)NewDeselect : (Action)NewSelect)();
    public void IslandSelect()
    {
        Select();
    }
    public void SelectOn()
    {
        NewSelect();
    }
    public void SelectOff()
    {
        NewDeselect();
    }
    private void NewSelect()
    {
        selected = true;
        highlighter.SetActive(true);
    }
    private void NewDeselect()
    {
        selected = false;
        highlighter.SetActive(false);
    }

    private void Select()
    {
        selected = true;

        foreach (var (neighbor, direction) in NeighborsWithDirection())
        {
            if (neighbor == null || !neighbor.selected)
            {
                UpdateEdge(direction);
            }
        }
        UpdateNeighbors();
    }

    public void PopulateNeighbors()
    {
        if (allNeighbors == null)
        {
            allNeighbors = new List<Hex>();
        }
        foreach (HexNeighborDirection direction in EnumArray<HexNeighborDirection>.Values)
        {
            Hex neighbor = hexSpawner.GetNeighborAt(hexIndex, direction);
            if (neighbor != null)
            {
                allNeighbors.Add(neighbor);
            }
        }
    }

    public void UpdateEdge(HexNeighborDirection direction) =>
        meshRenderer.material.SetFloat(
            name: $"_Edge{(int)direction}",
            value: Mathf.Abs(meshRenderer.material.GetFloat($"_Edge{(int)direction}") - 1).Floor()
            );

    

    private void Deselect()
    {
        selected = false;

        for (int i = 0; i < 6; i++)
        {
            meshRenderer.material.SetFloat($"_Edge{i}", 0f);
        }
        UpdateNeighbors();
    }

    public void UpdateNeighbors()
    {
        foreach (var (neighbor, direction) in NeighborsWithDirection())
        {
            if (neighbor != null && neighbor.selected)
            {
                neighbor.UpdateEdge(direction.Opposite());
            }
        }
    }

    public void SetHexPlayerColor(bool turnOff = false)
    {
        if (currentUnitOnHex != null)
        {
            teamColorHighlighter.SetActive(true);
            teamColorHighlighter.GetComponent<SpriteRenderer>().color = currentUnitOnHex.playerOwner.teamColor;
            textID.text = currentUnitOnHex.unitName;
            textID.color = currentUnitOnHex.playerOwner.teamColor;
            shield.SetActive(true);

        }

        if (turnOff)
        {
            teamColorHighlighter.SetActive(false);
            teamColorHighlighter.GetComponent<SpriteRenderer>().color = Color.gray;
            shield.SetActive(false);


        }
    }

    public void SetHexPerimeterColor(Color newColor)
    {
        meshRenderer.material.SetColor($"_EdgeColor", newColor);
    }

}

[System.Serializable]
public struct Index
{
    public int row;
    public int col;

    public Index(int _row, int _col)
    {
        row = _row;
        col = _col;
    }
}

public enum TerrainType { Water, Land}