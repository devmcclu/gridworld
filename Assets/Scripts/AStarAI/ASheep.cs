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
        targetNode = tileArray[pos.x, pos.y];
        // Go through list of grid objects
        for (int col = 0; col < tileArray.GetLength(0); col++)
        {
            for (int row = 0; row < tileArray.GetLength(1); row++)
            {
                if(tileArray[col, row].TryGetComponent(typeof(TileController), out Component component))
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
        Dictionary<GameObject, int[]> openNodes = new Dictionary<GameObject, int[]>();
        Dictionary<GameObject, int[]> closedNodes = new Dictionary<GameObject, int[]>();
        KeyValuePair<GameObject, int[]> startNode = new KeyValuePair<GameObject, int[]>(tileArray[0, 0], new int[2]);

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
                        int heuristic = Heuristic(tileArray[col, row].GetComponent<TileCost>().GetPos());
                        int[] cost = {0, heuristic};
                        openNodes.Add(tileArray[col, row], cost);
                        startNode = new KeyValuePair<GameObject, int[]>(tileArray[col, row], cost);
                        break;
                    }
                }
            }
        }
        Debug.Log("Start node added");

        KeyValuePair<GameObject, int[]> curNode = startNode;
        TileCost finalNode = curNode.Key.GetComponent<TileCost>();
        // Go through each optimal node path
        while (openNodes.Count > 0)
        {
            bool targetFound = false;
            // Find the lowest cost node
            foreach (KeyValuePair<GameObject, int[]> entry in openNodes)
            {
                if (entry.Value[0] + entry.Value[1] < curNode.Value[0] + curNode.Value[1])
                {
                    curNode = entry;
                }
            }

            // Remove the lowest code node from open nodes
            openNodes.Remove(curNode.Key);
            Debug.Log("curNode closed");
            // Set parents of each neighbor
            foreach (KeyValuePair<GameObject, int> entry in curNode.Key.GetComponent<TileCost>().adjacentTiles)
            {
                entry.Key.GetComponent<TileCost>().SetParentNode(curNode.Key.GetComponent<TileCost>());
            }

            Debug.Log("Found all neightbors");

            // Go through each neighbor
            foreach (KeyValuePair<GameObject, int> entry in curNode.Key.GetComponent<TileCost>().adjacentTiles)
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
                int heuristic = Heuristic(entry.Key.GetComponent<TileCost>().GetPos());
                int[] cost = {curNode.Value[0] + entry.Value, heuristic};

                KeyValuePair<GameObject, int[]> neighborNode = new KeyValuePair<GameObject, int[]>(entry.Key, cost);

                // See if we have better candidate
                foreach (KeyValuePair<GameObject, int[]> opened in openNodes)
                {
                    if (opened.Value[0] + opened.Value[1] < neighborNode.Value[0] + neighborNode.Value[1])
                    {
                        skipped = true;
                        break;
                    }
                }

                // See if we have better path
                foreach (KeyValuePair<GameObject, int[]> closed in closedNodes)
                {
                    if (closed.Value[0] + closed.Value[1] < neighborNode.Value[0] + neighborNode.Value[1])
                    {
                        skipped = true;
                        break;
                    }
                }

                if (skipped == false)
                {
                    Debug.Log("New candidate");
                    openNodes.Add(neighborNode.Key, neighborNode.Value);
                }

            }
            if (targetFound)
            {
                break;
            }
            closedNodes.Add(curNode.Key, curNode.Value);
            curNode = new KeyValuePair<GameObject, int[]>(startNode.Key, new int[2] {0, System.Int32.MaxValue});
        }
        StartCoroutine(StartMovement(finalNode, startNode.Key.GetComponent<TileCost>()));
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

    int Heuristic(Vector2Int pos)
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
