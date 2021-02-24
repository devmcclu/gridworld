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
        
        // Go through each optimal node path
        while (openNodes.Count > 0)
        {
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

            foreach (KeyValuePair<GameObject, int> entry in curNode.Key.GetComponent<TileCost>().adjacentTiles)
            {
                
            }
        }
        //StartCoroutine(StartMovement(finalCostNodes, curNode));
    }

    IEnumerator StartMovement(Dictionary<GameObject, int> finalCostNodes, KeyValuePair<GameObject, int> curNode)
    {
        Debug.Log("Start moving");
        KeyValuePair<GameObject, int> nextNode = new KeyValuePair<GameObject, int>(tileArray[0, 0], System.Int32.MaxValue);
        // While not to target
        while (curNode.Key != targetNode)
        {
            // From curNode, go to lowest cost node
            finalCostNodes.Remove(curNode.Key);
            foreach (KeyValuePair<GameObject, int> node in finalCostNodes)
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
            nextNode = new KeyValuePair<GameObject, int>(tileArray[0, 0], System.Int32.MaxValue);
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
