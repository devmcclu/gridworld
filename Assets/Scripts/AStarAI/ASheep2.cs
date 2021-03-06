﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASheep2 : MonoBehaviour
{
    // List of Grid objects
    public GameObject[,] tileArray;
    // Object to move to
    private GameObject targetNode;
    private Vector2Int targetNodePos;
    // 2D Representation of position
    private Vector2Int pos;
    [SerializeField]
    private float moveTime = 2f;
    private float timer = 0f;
    private bool isFinding = false;
    void Update()
    {
        if (timer < 5f && !isFinding)
        {
            timer+= Time.deltaTime;
        }
        else if (!isFinding)
        {
            isFinding = true;
            Debug.Log("Position: " + pos.x + ", " + pos.y);
            FindTarget();
        }
    }

    public void FindTarget()
    {
        while (targetNode == null)
        {
            int newX = Random.Range(0, tileArray.GetLength(0));
            int newY = Random.Range(0, tileArray.GetLength(1));
            if (tileArray[newX, newY].TryGetComponent(typeof(TileController), out Component component))
            {
                targetNode = tileArray[newX, newY];
            }
        }

        Debug.Log("Target node pos: " + targetNode.transform.localPosition);

        MovementGen();
    }

    void MovementGen()
    {
        Debug.Log("Start Movement gen");

        // Initialize open list
        List<GameObject> openNodes = new List<GameObject>();
        // Initialize closed list
        List<GameObject> closedNodes = new List<GameObject>();
        // Put starting node on open list (g = 0, h = heuristic)
        tileArray[pos.x, pos.y].GetComponent<TileCost>().currentCost = 0;
        tileArray[pos.x, pos.y].GetComponent<TileCost>().heuristic = Heuristic(pos);
        openNodes.Add(tileArray[pos.x, pos.y]);

        GameObject curNode = tileArray[pos.x, pos.y];
        bool targetFound = false;

        // While open list is not empty
        while(openNodes.Count > 0 && !targetFound)
        {
            //  Find the node with the lowest total cost in open
            foreach(GameObject node in openNodes)
            {
                // Take the node off the open list and call it curNode
                if (node.GetComponent<TileCost>().FinalCost() < curNode.GetComponent<TileCost>().FinalCost())
                {
                    curNode = node;
                }
            }
            openNodes.Remove(curNode);
            TileCost curNodeCost = curNode.GetComponent<TileCost>();
            //  find curNodes neighbors and set their parent to be curNode
            foreach(KeyValuePair<GameObject, float> neighbor in curNodeCost.adjacentTiles)
            {
                neighbor.Key.GetComponent<TileCost>().SetParentNode(curNodeCost);
            }

            Debug.Log("Found all neightbors");

            //  for each neighbor
            foreach(KeyValuePair<GameObject, float> neighbor in curNodeCost.adjacentTiles)
            {
                TileCost neighborCost = neighbor.Key.GetComponent<TileCost>();

                // neighbor.g = curNode.g + cost to go from curNode to neighbor
                neighborCost.currentCost = curNodeCost.currentCost + neighbor.Value;
                // neighbor.h = heuristic
                neighborCost.heuristic = Heuristic(neighborCost.GetPos());
                // neighbor.f = neighbor.g + neighbor.h

                // If neighbor is goal, stop search
                if (neighbor.Key.Equals(targetNode))
                {
                    curNode = neighbor.Key;
                    closedNodes.Add(neighbor.Key);
                    targetFound = true;
                    Debug.Log("FOund target node");
                    break;    
                }

                bool isSkipped = false;

                foreach(GameObject node in openNodes)
                {
                    // if a node in the OPen list has a lower f than neighbor
                    if (node.GetComponent<TileCost>().FinalCost() < neighborCost.FinalCost())
                    {
                        Debug.Log("Better Candidate");
                        // skip neighbor
                        isSkipped = true;
                        break;
                    }
                }

                foreach(GameObject node in closedNodes)
                {
                    // if a node in the closed lost has a lower f than neighbor
                    if (node.GetComponent<TileCost>().FinalCost() < neighborCost.FinalCost())
                    {
                        Debug.Log("Better path");
                        // skip neighbor
                        isSkipped = true;
                        break;
                    }
                }
                
                // else
                if (!isSkipped && !openNodes.Contains(neighbor.Key))
                {
                    Debug.Log("New candidate");
                    // add node to open list
                    openNodes.Add(neighbor.Key);
                }
                //  end for
            }            
            // end while
        }     

        // if we have found the goal
        if (closedNodes.Contains(targetNode))
        {
            // retrace path from goal back to start via each node's parent 
            Stack<GameObject> pathToTarget = new Stack<GameObject>();
            pathToTarget.Push(curNode);
            while(!pathToTarget.Contains(tileArray[pos.x, pos.y]))
            {
                GameObject nextNode = curNode.GetComponent<TileCost>().GetParentNode().gameObject;
                pathToTarget.Push(nextNode);
                curNode = nextNode;
            }
            StartCoroutine(StartMovement(pathToTarget));
        }
    }

    IEnumerator StartMovement(Stack<GameObject> path)
    {
        while(path.Count > 0)
        {
            Debug.Log("New place");

            GameObject nextNode = path.Pop();

            Vector3 newPosition = nextNode.transform.localPosition;
            transform.localPosition = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            pos = nextNode.GetComponent<TileCost>().GetPos();
            Debug.Log("New pos: x" + newPosition.x + ", y " + newPosition.y);

            yield return new WaitForSeconds(moveTime);
        }

        Debug.Log("Done");
    }

    float Heuristic(Vector2Int pos)
    {
        return Mathf.Abs(targetNodePos.x - pos.x) + Mathf.Abs(targetNodePos.y - pos.y);
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
