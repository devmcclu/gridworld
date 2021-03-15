using System.Collections;
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
        targetNodePos = targetNode.GetComponent<TileCost>().GetPos();
        Debug.Log("Target node pos: " + targetNodePos);

        MovementGen();
    }

    void MovementGen()
    {        
        for (int i = 0; i < tileArray.GetLength(0); i++)
        {
            for (int j = 0; j < tileArray.GetLength(1); j++)
            {
                if (tileArray[i, j].TryGetComponent(typeof(TileController), out Component component))
                {
                    //Debug.Log("Col: " + i + " Row: " + j);
                    tileArray[i, j].GetComponent<TileCost>().ResetSprite();
                }
            }
        }

        Debug.Log("Start Movement gen");

        // Initialize open list
        List<GameObject> openNodes = new List<GameObject>();
        // Initialize closed list
        List<GameObject> closedNodes = new List<GameObject>();
        // Put starting node on open list (g = 0, h = heuristic)
        TileCost startNode = tileArray[pos.x, pos.y].GetComponent<TileCost>();
        startNode.CurrentCost = 0;
        startNode.Heuristic = Heuristic(pos);
        openNodes.Add(tileArray[pos.x, pos.y]);
        startNode.SetOpened();

        GameObject curNode = tileArray[pos.x, pos.y];
        bool targetFound = false;

        // While open list is not empty
        while(openNodes.Count > 0)
        {
            Debug.Log("OPen Nodes size:" + openNodes.Count);

            if (targetFound)
            {
                break;
            }
            curNode = openNodes[0];
            //  Find the node with the lowest total cost in open
            foreach(GameObject node in openNodes)
            {
                // Take the node off the open list and call it curNode
                if (node.GetComponent<TileCost>().FinalCost() < curNode.GetComponent<TileCost>().FinalCost())
                {
                    curNode = node;
                }
            }

            Debug.Log("curNode: " + curNode.name);

            // Remove current node from openNodes list
            openNodes.Remove(curNode);
            TileCost curNodeCost = curNode.GetComponent<TileCost>();
            curNodeCost.ResetSprite();
            
            //  find curNodes neighbors and set their parent to be curNode
            foreach(KeyValuePair<GameObject, float> neighbor in curNodeCost.adjacentTiles)
            {
                if (!closedNodes.Contains(neighbor.Key))
                {
                    neighbor.Key.GetComponent<TileCost>().SetParentNode(curNodeCost);
                }
            }

            Debug.Log("Found all neightbors");

            // for each neighbor
            foreach(KeyValuePair<GameObject, float> neighbor in curNodeCost.adjacentTiles)
            {
                TileCost neighborCost = neighbor.Key.GetComponent<TileCost>();

                // neighbor.g = curNode.g + cost to go from curNode to neighbor
                neighborCost.CurrentCost = curNodeCost.CurrentCost + neighbor.Value;
                // neighbor.h = heuristic
                neighborCost.Heuristic = Heuristic(neighborCost.GetPos());
                // neighbor.f = neighbor.g + neighbor.h - done in neighbor

                // If neighbor is goal, stop search
                if (neighbor.Key.name == targetNode.name)
                {
                    closedNodes.Add(curNode);
                    curNodeCost.SetClosed(); 
                    curNode = neighbor.Key;
                    //closedNodes.Add(neighbor.Key);
                    //neighborCost.SetClosed();
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
                    neighborCost.SetOpened();
                }
                //  end for
            }     
            // Remove curNode from closed list
            //if (!closedNodes.Contains(curNode))
            //{
            closedNodes.Add(curNode);
            curNodeCost.SetClosed(); 
            //}
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
                Debug.Log("Path size: " + pathToTarget.Count);
            }
            StartCoroutine(StartMovement(pathToTarget));
        }
        else
        {
            Debug.Log("Failure");
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

    float Heuristic(Vector2Int nodePos)
    {
        // Cost to go cardinal
        float D = 1f;
        // Cost to go diagonal
        float D2 = 1.5f;
        // POsition delta to target
        float dx = Mathf.Abs(nodePos.x - targetNodePos.x);
        float dy = Mathf.Abs(nodePos.y - targetNodePos.y);

        return (D * (dx + dy)) + ((D2 - (2 * D)) * Mathf.Min(dx, dy));
        //return dx + dy;
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
