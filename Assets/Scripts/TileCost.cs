using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField]
    private GameObject closedSprite;
    [SerializeField]
    private GameObject openedSprite;
    [SerializeField]
    private TextMeshPro costText;
    // Estimated distance to goal
    public float Heuristic { get; set; }
    // Current cost from start point
    public float CurrentCost { get; set; }
    // Dictionary: Adj tile w/cost to move to
    public Dictionary<GameObject, float> adjacentTiles = new Dictionary<GameObject, float>();


    void Start()
    {
        ResetSprite();    
    }

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
        float cost = Heuristic + CurrentCost;
        costText.text = cost.ToString();
        return cost;
    }

    public void SetOpened()
    {
        openedSprite.SetActive(true);
        closedSprite.SetActive(false);
    }

    public void SetClosed()
    {
        openedSprite.SetActive(false);
        closedSprite.SetActive(true);
    }

    public void ResetSprite()
    {
        openedSprite.SetActive(false);
        closedSprite.SetActive(false);
    }
}
