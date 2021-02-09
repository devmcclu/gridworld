using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraSheep : MonoBehaviour
{
    // List of Grid objects
    // Object to move to


    void FindTarget()
    {
        // Go through list of grid objects
        // If health of grass is < current object and > 0, set object as target
        // Move towards target
    }

    void MovementGen()
    {
        // For all grid objects that are grass
            // Add to list of movement nodes
            // Set cost to inf
        // Set cost of cur tile to 0
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
