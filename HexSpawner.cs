using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSpawner : SerializedMonoBehaviour
{
    [SerializeField] private GameMaster gm;

    [SerializeField] private Hex hexPrefab;
    [SerializeField] private Hex waterPrefab;
    [SerializeField] private Hex waterPrefabCantSelect;

    [SerializeField] private Transform HexLoadInTransform;
    [SerializeField] private Camera mainCam;



    public HexGrid hexGrid;

    [OdinSerialize, ReadOnly] public List<List<Hex>> hexes = new List<List<Hex>>();


    [Button("Spawn Hexes")]
    public void SpawnHexes()
    {
        if (hexes.Count > 0)
        {
            ClearHexes();
        }

        for (int row = 0; row < hexGrid.rows; row++)
        {
            hexes.Add(new List<Hex>());
            for (int col = 0; col < hexGrid.cols; col++)
            {
                float depth = CalculteHeight(row, col);

                //OUT OF BOUNDS WATER
                if (col < hexGrid.leftOffset || col >= hexGrid.cols - hexGrid.rightOffset || row < hexGrid.upOffset || row >= hexGrid.rows - hexGrid.downOffset)
                {
                    Hex newHex = Instantiate(
                    original: waterPrefabCantSelect,
                    position: new Vector3(
                        x: hexGrid.radius * 3 * col + Get_X_Offset(row),
                        y: 0,
                        z: row * hexGrid.Apothem
                        ),
                        rotation: Quaternion.Euler(0, hexGrid.rotationY, 0),
                        parent: HexLoadInTransform
                    );
                    newHex.depth = depth;
                    newHex.hexSpawner = this;
                    newHex.terrainType = TerrainType.Water;
                    newHex.isNotOutOfBounds = false;
                    newHex.transform.localScale = new Vector3(
                        x: newHex.transform.localScale.x * hexGrid.radius,
                        y: newHex.transform.localScale.y * hexGrid.height,
                        z: newHex.transform.localScale.z * hexGrid.radius
                        );
                    newHex.AssignIndex(new Index(row, col));
                    hexes[row].Add(newHex);
                }

                //LAND PREFABS
                else if (depth > 1 && (
                    col >= hexGrid.leftInnerOffset + hexGrid.leftOffset-1 && 
                    col <= hexGrid.cols - (hexGrid.rightOffset + hexGrid.rightInnerOffset) && 
                    row > hexGrid.upOffset + hexGrid.upInnerOffset - 2 && 
                    row <= hexGrid.rows - (hexGrid.downOffset + hexGrid.downInnerOffset)
                    ))
                {
                    Hex newHex = Instantiate(
                    original: hexPrefab,
                    position: new Vector3(
                        x: hexGrid.radius * 3 * col + Get_X_Offset(row),
                        y: -1f,
                        z: row * hexGrid.Apothem
                        ),
                        rotation: Quaternion.Euler(0, hexGrid.rotationY, 0),
                        parent: HexLoadInTransform
                    );
                    newHex.depth = depth;
                    newHex.hexSpawner = this;
                    newHex.terrainType = TerrainType.Land;
                    newHex.isNotOutOfBounds = true;

                    newHex.transform.localScale = new Vector3(
                        x: newHex.transform.localScale.x * hexGrid.radius,
                        y: newHex.transform.localScale.y * hexGrid.height,
                        z: newHex.transform.localScale.z * hexGrid.radius
                        );
                    newHex.AssignIndex(new Index(row, col));
                    hexes[row].Add(newHex);

                    //Build Building
                    gm.CreateBuilding(newHex);
                }


                //WATER PREFAB
                else
                {
                    Hex newHex = Instantiate(
                    original: waterPrefab,
                    position: new Vector3(
                        x: hexGrid.radius * 3 * col + Get_X_Offset(row),
                        y: 0,
                        z: row * hexGrid.Apothem
                        ),
                        rotation: Quaternion.Euler(0, hexGrid.rotationY, 0),
                        parent: HexLoadInTransform
                    );
                    newHex.AssignCamera(mainCam);

                    newHex.depth = depth;
                    newHex.hexSpawner = this;
                    newHex.terrainType = TerrainType.Water;
                    newHex.isNotOutOfBounds = true;

                    newHex.transform.localScale = new Vector3(
                        x: newHex.transform.localScale.x * hexGrid.radius,
                        y: newHex.transform.localScale.y * hexGrid.height,
                        z: newHex.transform.localScale.z * hexGrid.radius
                        );
                    newHex.AssignIndex(new Index(row, col));
                    hexes[row].Add(newHex);
                }
            }
        }
        for (int i = 0; i < hexes.Count; i++)
        {
            for (int m = 0; m < hexes[i].Count; m++)
            {
                hexes[i][m].PopulateNeighbors();
            }
        }
    }

    private float CalculteHeight(int x, int y)
    {
        float xCoord = (float)x / hexGrid.rows /  hexGrid.perlinResolution;
        float yCoord = (float)y / hexGrid.cols / hexGrid.perlinResolution;


        return (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * hexGrid.perlinScale;

    }

    private float Get_X_Offset(int row) => row % 2 == 0 ? hexGrid.radius * 1.5f : 0;

    [Button("Clear Hexes")]
    private void ClearHexes()
    {
        if (hexes == null)
        {
            hexes = new List<List<Hex>>();
            return;
        }
        for (int row = 0; row < hexes.Count; row++)
        {
            for (int col = 0; col < hexes[row].Count; col++)
            {

                if (Application.isEditor)
                {
                    DestroyImmediate(hexes[row][col].gameObject);
                }
                else
                {
                    Destroy(hexes[row][col].gameObject);
                }
            }
        }
        hexes = new List<List<Hex>>();
    }

    public Hex GetNeighborAt(Index hexIndex, HexNeighborDirection direction)
    {
        (int row, int col) offsets = GetOffsetInDirection(hexIndex.row % 2 == 0, direction);
        return GetHexIfInBounds(hexIndex.row + offsets.row, hexIndex.col + offsets.col);
    }

    private Hex GetHexIfInBounds(int row, int col) => 
        hexGrid.IsInBounds(row, col) ? hexes[row][col] : null;

    private (int row, int col) GetOffsetInDirection(bool isEven, HexNeighborDirection direction)
    {
        switch (direction)
        {
            case HexNeighborDirection.Down:
                return (2, 0);
            case HexNeighborDirection.DownRight:
                return isEven ? (1, 1) : (1, 0);
            case HexNeighborDirection.UpRight:
                return isEven ? (-1, 1) : (-1, 0);
            case HexNeighborDirection.Up:
                return (-2, 0);
            case HexNeighborDirection.UpLeft:
                return isEven ? (-1, 0) : (-1, -1);
            case HexNeighborDirection.DownLeft:
                return isEven ? (1, 0) : (1, -1);
        }
        return (0, 0);
    }
}

public enum HexNeighborDirection { Up, UpRight, DownRight, Down, DownLeft, UpLeft}