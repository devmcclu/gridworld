using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASheep : MonoBehaviour
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
        Dictionary<GameObject, float[]> openNodes = new Dictionary<GameObject, float[]>();
        Dictionary<GameObject, float[]> closedNodes = new Dictionary<GameObject, float[]>();
        KeyValuePair<GameObject, float[]> startNode = new KeyValuePair<GameObject, float[]>(tileArray[0, 0], new float[2]);

        targetNodePos = targetNode.GetComponent<TileCost>().GetPos();
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
                        float heuristic = Heuristic(tileArray[col, row].GetComponent<TileCost>().GetPos());
                        float[] cost = {0, heuristic};
                        openNodes.Add(tileArray[col, row], cost);
                        startNode = new KeyValuePair<GameObject, float[]>(tileArray[col, row], cost);
                        Debug.Log("Start node added");
                        break;
                    }
                }
            }
        }
        Debug.Log("Start node found");

        KeyValuePair<GameObject, float[]> curNode = startNode;
        TileCost finalNode = curNode.Key.GetComponent<TileCost>();
        // Go through each optimal node path
        while (openNodes.Count > 0)
        {
            bool targetFound = false;
            // Set current node to a node that is open
            var openEnum = openNodes.GetEnumerator();
            openEnum.MoveNext();
            curNode = openEnum.Current;
            // Find the lowest cost node
            foreach (KeyValuePair<GameObject, float[]> entry in openNodes)
            {
                if (entry.Value[0] + entry.Value[1] < curNode.Value[0] + curNode.Value[1] && !closedNodes.ContainsKey(entry.Key))
                {
                    curNode = entry;
                }
            }

            // Remove the lowest code node from open nodes
            openNodes.Remove(curNode.Key);
            Debug.Log("curNode closed");
            closedNodes.Add(curNode.Key, curNode.Value);
            Debug.Assert(!openNodes.ContainsKey(curNode.Key));

            // Set parents of each neighbor
            foreach (KeyValuePair<GameObject, float> entry in curNode.Key.GetComponent<TileCost>().adjacentTiles)
            {
                entry.Key.GetComponent<TileCost>().SetParentNode(curNode.Key.GetComponent<TileCost>());
            }

            Debug.Log("Found all neightbors");

            // Go through each neighbor
            foreach (KeyValuePair<GameObject, float> entry in curNode.Key.GetComponent<TileCost>().adjacentTiles)
            {
                bool skipped = false;

                // Found target node
                if (entry.Key == targetNode)
                {
                    finalNode = entry.Key.GetComponent<TileCost>();
                    targetFound = true;
                    Debug.Log("FOund target node");
                    break;
                }

                // Current neightbor values
                float heuristic = Heuristic(entry.Key.GetComponent<TileCost>().GetPos());
                float[] cost = {curNode.Value[0] + entry.Value, heuristic};

                KeyValuePair<GameObject, float[]> neighborNode = new KeyValuePair<GameObject, float[]>(entry.Key, cost);

                // See if we have better candidate
                foreach (KeyValuePair<GameObject, float[]> opened in openNodes)
                {
                    if (opened.Value[0] + opened.Value[1] < neighborNode.Value[0] + neighborNode.Value[1])
                    {
                        Debug.Log("Better Candidate");
                        skipped = true;
                        break;
                    }
                }

                // See if we have better path
                foreach (KeyValuePair<GameObject, float[]> closed in closedNodes)
                {
                    if (closed.Value[0] + closed.Value[1] < neighborNode.Value[0] + neighborNode.Value[1])
                    {
                        Debug.Log("Better path");
                        skipped = true;
                        break;
                    }
                }

                if (skipped == false)
                {
                    Debug.Log("New candidate");
                    if (openNodes.ContainsKey(neighborNode.Key))
                    {
                        openNodes[neighborNode.Key] = neighborNode.Value;
                    }
                    else
                    {
                        openNodes.Add(neighborNode.Key, neighborNode.Value);
                    }
                }

            }
            if (targetFound)
            {
                break;
            }
            // if (closedNodes.ContainsKey(curNode.Key))
            // {
            //     closedNodes[curNode.Key] = curNode.Value;    
            // }
            // else
            // {
            //     closedNodes.Add(curNode.Key, curNode.Value);
            // }
            // curNode = new KeyValuePair<GameObject, float[]>(startNode.Key, new float[2] {0, System.Int32.MaxValue});
        }

        closedNodes.Clear();
        openNodes.Clear();

        if (finalNode == startNode.Key.GetComponent<TileCost>())
        {
            Debug.Log("Failure, start over");
            FindTarget();
        }
        else
        {
            StartCoroutine(StartMovement(finalNode, startNode.Key.GetComponent<TileCost>()));
        }
    }

    IEnumerator StartMovement(TileCost finalNode, TileCost startNode)
    {
        Debug.Log("Find start node");
        Stack<TileCost> movementNodes = new Stack<TileCost>();
        TileCost newNode = finalNode;

        while(newNode.GetParentNode() != startNode)
        {
            movementNodes.Push(finalNode);
            newNode = finalNode.GetParentNode();
        }

        while(movementNodes.Count > 0)
        {
            Debug.Log("New place");

            TileCost nextNode = movementNodes.Pop();

            Vector3 newPosition = nextNode.gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(newPosition.x, newPosition.y, transform.localPosition.z);
            pos = nextNode.GetPos();
            Debug.Log("New pos: x" + newPosition.x + ", y " + newPosition.y);
            yield return new WaitForSeconds(moveTime);
        }

        Debug.Log("Done");
        FindTarget();
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
