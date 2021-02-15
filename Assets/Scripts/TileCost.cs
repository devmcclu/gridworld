using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCost : MonoBehaviour
{
    // Current cost
    [SerializeField]
    private int cost;
    // Where in the array the tile is
    [SerializeField]
    private Vector2Int pos;
    // Dictionary: Adj tile w/cost to move to
    Dictionary<GameObject, int> adjacentTiles = new Dictionary<GameObject, int>();
    
    void Start()
    {

    }

    public void SetPos(int newX, int newY)
    {
        pos = new Vector2Int(newX, newY);
    }

    public Vector2Int GetPos()
    {
        return pos;
    }
}
