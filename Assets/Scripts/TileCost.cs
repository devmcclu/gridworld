﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCost : MonoBehaviour
{
    // Current cost
    // [SerializeField]
    // private int cost;
    // Where in the array the tile is
    [SerializeField]
    private Vector2Int pos;
    [SerializeField]
    private TileCost parentNode;
    // Dictionary: Adj tile w/cost to move to
    public Dictionary<GameObject, int> adjacentTiles = new Dictionary<GameObject, int>();

    public void SetPos(int newX, int newY)
    {
        pos = new Vector2Int(newX, newY);
    }

    public Vector2Int GetPos()
    {
        return pos;
    }

    public void SetParentNode(TileCost parent)
    {
        parentNode = parent;
    }

    public TileCost GetParentNode()
    {
        return parentNode;
    }
}
