using System.Collections;
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
    // Estimated distance to goal
    public float Heuristic { get; set; }
    // Current cost from start point
    public float CurrentCost { get; set; }
    // Dictionary: Adj tile w/cost to move to
    public Dictionary<GameObject, float> adjacentTiles = new Dictionary<GameObject, float>();

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

    // F cost in A*
    public float FinalCost()
    {
        return Heuristic + CurrentCost;
    }
}
