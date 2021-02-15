using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraSheep : MonoBehaviour
{
    // List of Grid objects
    private GameObject[,] tileArray;
    // Object to move to
    private GameObject targetNode;
    // 2D Representation of position
    private Vector2Int pos;

    void FindTarget()
    {
        // Go through list of grid objects
        for (int col = 0; col < tileArray.GetLength(0); col++)
        {
            for (int row = 0; row < tileArray.GetLength(1); row++)
            {
                // If health of grass is < current object and > 0, set object as target
                int newHealth = tileArray[col, row].GetComponent<TileController>().GetHealth();
                int curHealth = targetNode.GetComponent<TileController>().GetHealth();
                if (newHealth < curHealth && newHealth > 0)
                {
                    targetNode = tileArray[col, row];
                }
            }
        }
        // Move towards target
        MovementGen();
    }

    void MovementGen()
    {
        // Movement nodes
        Dictionary<GameObject, int> movementNodes = new Dictionary<GameObject, int>();
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
                    }
                    else
                    {
                        movementNodes.Add(tileArray[col, row], System.Int32.MaxValue);
                    }
                }
            }
        }
        // While movement nodes not empty
            // curNode = lowest cost node
            // remove curNode from movementNode list
            //for each adj node of curNode
                // if cost of curNode + move to adjNode cost < adjNode cost
                    // adjNode cost = cost of curNode + move to adjNode cost
        // While not to target
            // From curNode, go to lowest cost node
    }
}
