using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private List<Hex> finalPath;

    public Pathfinding(Hex startHex, Hex targetHex)
    {
        FindPath(startHex, targetHex);
    }
    public List<Hex> GetFinalPath()
    {
        return finalPath;
    }

    private void FindPath(Hex startHex, Hex targetHex)
    {
        List<Hex> openHexes = new List<Hex>();
        List<Hex> closedHexes= new List<Hex>();

        openHexes.Add(startHex);
        while (true)
        {
            Hex currentHex = openHexes[0];
            openHexes.RemoveAt(0);
            closedHexes.Add(currentHex);
            if (currentHex == targetHex)
            {
                TracePathAndReturnPathToList(currentHex, startHex);
                break;
            }

            foreach (Hex neighbor in currentHex.GetNeighbors())
            {
                if (!closedHexes.Contains(neighbor))
                {
                    if (!openHexes.Contains(neighbor) && neighbor.terrainType != TerrainType.Land && neighbor.currentUnitOnHex == null)
                    {
                        neighbor.parentHex = currentHex;
                        openHexes.Add(neighbor);
                    }
                }
            }
        }
    }
    private void TracePathAndReturnPathToList(Hex endHex, Hex startHex)
    {
        List<Hex> finalHexPath = new List<Hex>();
        Hex currentHex = endHex;
        while (true)
        {
            finalHexPath.Add(currentHex);
            if (currentHex == startHex)
            {
                break;
            }
            currentHex = currentHex.parentHex;
        }
        finalHexPath.Reverse();
        finalPath = finalHexPath;
    }
}
