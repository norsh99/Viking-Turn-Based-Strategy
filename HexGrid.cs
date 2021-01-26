using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexGrid
{

    public int cols;
    [ShowInInspector, ReadOnly] public int maxCol => cols - 1;
    public int rows;
    [ShowInInspector, ReadOnly] public int maxRow => rows - 1;

    public int radius;
    public int height;
    public int rotationY;

    public int perlinScale;
    public float perlinResolution;

    public int leftOffset;
    public int rightOffset;
    public int upOffset;
    public int downOffset;

    public int leftInnerOffset;
    public int rightInnerOffset;
    public int upInnerOffset;
    public int downInnerOffset;





    public float Apothem =>
        Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(radius * 0.5f, 2f));

    public bool IsInBounds(int row, int col) =>
        row * (row - maxRow) <= 0 && col * (col - maxCol) <= 0;

}
