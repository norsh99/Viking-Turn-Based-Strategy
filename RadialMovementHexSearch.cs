using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMovementHexSearch
{

    List<Hex> finalAvailableHexes = new List<Hex>();


    public RadialMovementHexSearch(Hex startHex, int distance, TerrainType excludeTerrainType)
    {
        List<Hex> finalAvailableHexes = new List<Hex>();

        FindPath(startHex, distance, excludeTerrainType);
    }


    private void FindPath(Hex startHex, int distance, TerrainType terrainType)
    {
        List<Hex> openHexes = new List<Hex>();
        List<Hex> prefHexes = new List<Hex>();

        List<Hex> closedHexes = new List<Hex>();

        openHexes.Add(startHex);
        for (int i = 0; i < distance + 1; i++)
        {
            for (int m = 0; m < openHexes.Count; m++)
            {
                Hex currentHex = openHexes[m];
                //openHexes.RemoveAt(0);
                if (!closedHexes.Contains(currentHex))
                {
                    closedHexes.Add(currentHex);

                }

                foreach (Hex neighbor in currentHex.GetNeighbors())
                {
                    if (!closedHexes.Contains(neighbor) || !prefHexes.Contains(neighbor) || !openHexes.Contains(neighbor))
                    {
                        if (neighbor.terrainType != terrainType && neighbor.isNotOutOfBounds && neighbor.currentUnitOnHex == null)
                        {
                            prefHexes.Add(neighbor);
                        }
                    }
                }
            } 
            openHexes.Clear();
            openHexes = new List<Hex>(prefHexes);
            prefHexes.Clear();
        }
        closedHexes.RemoveAt(0);

        finalAvailableHexes = closedHexes;
    }

    public List<Hex> GetAllSurroundingHexes()
    {
        return finalAvailableHexes;
    }
}
