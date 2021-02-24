﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraSheep : MonoBehaviour
{
    // List of Grid objects
    public GameObject[,] tileArray;
    // Object to move to
    private GameObject targetNode;
    // 2D Representation of position
    private Vector2Int pos;
    [SerializeField]
    private float moveTime = 2f;

    public void FindTarget()
    {
        if (tileArray[pos.x, pos.y].TryGetComponent(typeof(TileController), out Component component))
        {
            targetNode = tileArray[pos.x, pos.y];
        }
        else
        {
            targetNode = tileArray[0, 0];
        }
        // Go through list of grid objects
        for (int col = 0; col < tileArray.GetLength(0); col++)
        {
            for (int row = 0; row < tileArray.GetLength(1); row++)
            {
                if(tileArray[col, row].TryGetComponent(typeof(TileController), out Component component2))
                {
                    // If health of grass is < current object and > 0, set object as target
                    int newHealth = tileArray[col, row].GetComponent<TileController>().GetHealth();
                    int curHealth = targetNode.GetComponent<TileController>().GetHealth();               
                    if (newHealth < curHealth && newHealth > 0)
                    {
                        targetNode = tileArray[col, row];
                        Debug.Log("Target at col: " + col + ", row: " + row);
                    }
                }
            }
        }
        // Move towards target
        Debug.Log("Target node pos: " + targetNode.transform.localPosition);
        MovementGen();
    }

    void MovementGen()
    {
        Debug.Log("Start Movement gen");
        // Movement nodes
        Dictionary<GameObject, float> movementNodes = new Dictionary<GameObject, float>();
        Dictionary<GameObject, float> finalCostNodes = new Dictionary<GameObject, float>();
        KeyValuePair<GameObject, float> startNode = new KeyValuePair<GameObject, float>(tileArray[0, 0], 0);
        // For all grid objects that are grass
        for (int col = 0; col < tileArray.GetLength(0); col++)
        {
            for (int row = 0; row < tileArray.GetLength(1); row++)
            {
                if(tileArray[col, row].TryGetComponent(typeof(TileController), out Component component))
                {
                    // Add to list of movement nodes, Set cost to inf
                    // Set cost of cur tile to 0      
                    if(tileArray[col, row].GetComponent<TileCost>().GetPos().Equals(pos))
                    {
                        movementNodes.Add(tileArray[col, row], 0);
                        startNode = new KeyValuePair<GameObject, float>(tileArray[col, row], 0);
                    }
                    else
                    {
                        movementNodes.Add(tileArray[col, row], System.Int32.MaxValue);
                    }
                }
            }
        }
        Debug.Log("All movement nodes added");
        // While movement nodes not empty
        KeyValuePair<GameObject, float> curNode = startNode;
        //for (int i = 0; i < movementNodes.Keys.Count; i++)
        while (movementNodes.Count > 0)
        {
            Debug.Log("Movement list size: " + movementNodes.Count);
            // curNode = lowest cost node
            foreach (KeyValuePair<GameObject, float> entry in movementNodes)
            {
                if (entry.Value < curNode.Value)
                {
                    curNode = entry;
                }
            }
            // remove curNode from movementNode list
            while (movementNodes.ContainsKey(curNode.Key))
            {
                movementNodes.Remove(curNode.Key);
            }
            Debug.Log("Has current Node: " + movementNodes.ContainsKey(curNode.Key));

            if (!finalCostNodes.ContainsKey(curNode.Key))
            {
                finalCostNodes.Add(curNode.Key, curNode.Value);

                //for each adj node of curNode
                foreach (KeyValuePair<GameObject, float> node in curNode.Key.GetComponent<TileCost>().adjacentTiles)
                {
                    // if cost of curNode + move to adjNode cost < adjNode cost
                    if (movementNodes.ContainsKey(node.Key))
                    {
                        if (curNode.Value + node.Value < movementNodes[node.Key])
                        {
                            // adjNode cost = cost of curNode + move to adjNode cost
                            movementNodes[node.Key] = curNode.Value + node.Value;
                        }
                    }

                }
            }
            curNode = new KeyValuePair<GameObject, float>(tileArray[0, 0], System.Int32.MaxValue);
        }
    
        foreach(KeyValuePair<GameObject, float> entry in finalCostNodes)
        {
            if (entry.Value < curNode.Value)
            {
                curNode = entry;
            }
        }
        
        StartCoroutine(StartMovement(finalCostNodes, curNode));
    }

    IEnumerator StartMovement(Dictionary<GameObject, float> finalCostNodes, KeyValuePair<GameObject, float> curNode)
    {
        Debug.Log("Start moving");
        KeyValuePair<GameObject, float> nextNode = new KeyValuePair<GameObject, float>(tileArray[0, 0], System.Int32.MaxValue);
        // While not to target
        while (curNode.Key != targetNode)
        {
            // From curNode, go to lowest cost node
            finalCostNodes.Remove(curNode.Key);
            foreach (KeyValuePair<GameObject, float> node in finalCostNodes)
            {
                if (node.Value < nextNode.Value && curNode.Key.GetComponent<TileCost>().adjacentTiles.ContainsKey(node.Key))
                {
                    nextNode = node;
                }
            }
            Debug.Log("New place");

            Vector3 newPosition = nextNode.Key.gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(newPosition.x, newPosition.y, transform.localPosition.z);
            pos = nextNode.Key.GetComponent<TileCost>().GetPos();
            Debug.Log("New pos: x" + newPosition.x + ", y " + newPosition.y);
            curNode = nextNode;
            nextNode = new KeyValuePair<GameObject, float>(tileArray[0, 0], System.Int32.MaxValue);
            yield return new WaitForSeconds(moveTime);
        }
        Debug.Log("Done");
        FindTarget();
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
