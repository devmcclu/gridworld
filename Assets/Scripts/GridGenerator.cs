using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Length of grid
    [SerializeField]
    private int rows = 10;
    // Width of grid
    [SerializeField]
    private int cols = 10;
    // Float for spacing objects
    [SerializeField]
    private float tileSize = 1;
    // Public facing array of grid objects
    public GameObject[,] tileArray;

    // Start is called before the first frame update
    void Start()
    {
        tileArray = new GameObject[cols, rows];
        // Place a tile at each (int) coordinate
        GenerateGrid();
        // Randomaly decide if a sheep will spawn
    }

    void GenerateGrid()
    {
        // Get a tile to reference
        GameObject refGrassTile = (GameObject)Instantiate(Resources.Load("GrassTile"));
        GameObject refWaterTile = (GameObject)Instantiate(Resources.Load("WaterTile"));
        //GameObject refSheep = (GameObject)Instantiate(Resources.Load("rabbit"));
        GameObject refSheep = (GameObject)Instantiate(Resources.Load("rabbitDijkstra"));
        float chance;
        bool hasSpawned = false;
        
        for (int curCol = 0; curCol < cols; curCol++)
        {
            for (int curRow = 0; curRow < rows; curRow++)
            {
                chance = Random.Range(0f, 1f);

                if (chance > .2f)
                {        
                    // Create a new tile
                    GameObject tile = (GameObject)Instantiate(refGrassTile, transform);

                    //Get correct position with spacing
                    float posX = curCol * tileSize;
                    float posY = curRow * -tileSize;

                    //Set the position
                    Vector2 newPos = new Vector2(posX, posY);
                    tile.transform.position = newPos;
                    // Put tile in tileArray
                    tileArray[curCol, curRow] = tile;
                    tile.GetComponent<TileCost>().SetPos(curRow, curCol);

                    if (curCol > 0)
                    {
                        if (tileArray[curCol - 1, curRow].TryGetComponent(typeof(TileController), out Component component))
                        {
                            tile.GetComponent<TileCost>().adjacentTiles.Add(tileArray[curCol - 1, curRow], 1);
                            tileArray[curCol - 1, curRow].GetComponent<TileCost>().adjacentTiles.Add(tile, 1);
                        }
                    }
                    if (curRow > 0)
                    {
                        if (tileArray[curCol, curRow - 1].TryGetComponent(typeof(TileController), out Component component))
                        {
                            tile.GetComponent<TileCost>().adjacentTiles.Add(tileArray[curCol, curRow - 1], 1);
                            tileArray[curCol, curRow - 1].GetComponent<TileCost>().adjacentTiles.Add(tile, 1);
                        }
                    }

                    chance = Random.Range(0f, 1f);
                    if (chance > .7f && !hasSpawned)
                    {
                        GameObject sheep = (GameObject)Instantiate(refSheep, transform);
                        sheep.transform.position = newPos; //+ new Vector2(0.5f, 0.5f);
                        sheep.GetComponent<DijkstraSheep>().SetPos(curRow, curCol);
                        hasSpawned = true;
                    } 
                }  
                else
                {
                    // Create a new tile
                    GameObject tile = (GameObject)Instantiate(refWaterTile, transform);

                    //Get correct position with spacing
                    float posX = curCol * tileSize;
                    float posY = curRow * -tileSize;

                    //Set the position
                    Vector2 newPos = new Vector2(posX, posY);
                    tile.transform.position = newPos;
                    // Put tile in tileArray
                    tileArray[curCol, curRow] = tile;
                }             
            }
        }

        Destroy(refGrassTile);
        Destroy(refWaterTile);
        Destroy(refSheep);

        float gridW = cols * tileSize;
        float gridL = rows * tileSize;
        transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridL / 2 - tileSize / 2);

        DijkstraSheep curSheep = FindObjectOfType<DijkstraSheep>();
        curSheep.tileArray = tileArray;
        curSheep.FindTarget();
    }
}
