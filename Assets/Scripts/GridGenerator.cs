using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    enum AIType
    {
        Naive,
        Dijkstra,
        AStar,
        Utility
    }

    // Length of grid
    [SerializeField]
    private int rows = 10;
    // Width of grid
    [SerializeField]
    private int cols = 10;
    // Float for spacing objects
    [SerializeField]
    private float tileSize = 1;
    [SerializeField]
    private int rabbitAI = 1;
    [SerializeField]
    private int wolfAI = 1;
    [SerializeField]
    private AIType aiType;
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
        //Initialize type of sheep
        GameObject refSheep = new GameObject();
        GameObject refSnake = new GameObject();
        switch (aiType)
        {
            case AIType.Naive:
                refSheep = (GameObject)Instantiate(Resources.Load("rabbit"));
                break;

            case AIType.Dijkstra:
                refSheep = (GameObject)Instantiate(Resources.Load("rabbitDijkstra"));
                break;

            case AIType.AStar:
                refSheep = (GameObject)Instantiate(Resources.Load("rabbitAStar"));
                break;
            case AIType.Utility:
                refSheep = (GameObject)Instantiate(Resources.Load("rabbitUtility"));
                refSnake = (GameObject)Instantiate(Resources.Load("snakeUtility"));
                break;
        }
        
        float chance;
        // Check if one sheep has spawned for "simpler" AI
        // bool hasSpawned = false;
        // Int for sheep and snakes that have spawned
        int spawnedSheep = 0;
        int spawnedSnakes = 0;
        
        for (int curCol = 0; curCol < cols; curCol++)
        {
            for (int curRow = 0; curRow < rows; curRow++)
            {
                chance = Random.Range(0f, 1f);

                if (chance > .2f)
                {        
                    // Create a new tile
                    GameObject tile = (GameObject)Instantiate(refGrassTile, transform);
                    tile.name = "Tile: " + curCol + ", " + curRow;

                    //Get correct position with spacing
                    float posX = curCol * tileSize;
                    float posY = curRow * -tileSize;

                    //Set the position
                    Vector2 newPos = new Vector2(posX, posY);
                    tile.transform.position = newPos;
                    // Put tile in tileArray
                    tileArray[curCol, curRow] = tile;
                    tile.GetComponent<TileCost>().SetPos(curCol, curRow);

                    // Add adjacent colomn tiles
                    if (curCol > 0)
                    {
                        if (tileArray[curCol - 1, curRow].TryGetComponent(typeof(TileController), out Component component))
                        {
                            tile.GetComponent<TileCost>().adjacentTiles.Add(tileArray[curCol - 1, curRow], 1f);
                            tileArray[curCol - 1, curRow].GetComponent<TileCost>().adjacentTiles.Add(tile, 1f);
                        }
                    }
                    // Add adjacent row tiles
                    if (curRow > 0)
                    {
                        if (tileArray[curCol, curRow - 1].TryGetComponent(typeof(TileController), out Component component))
                        {
                            tile.GetComponent<TileCost>().adjacentTiles.Add(tileArray[curCol, curRow - 1], 1f);
                            tileArray[curCol, curRow - 1].GetComponent<TileCost>().adjacentTiles.Add(tile, 1f);
                        }
                    }
                    // Add adjacent diagonal tiles
                    if (curRow > 0 && curCol > 0)
                    {
                        if (tileArray[curCol - 1, curRow - 1].TryGetComponent(typeof(TileController), out Component component))
                        {
                            tile.GetComponent<TileCost>().adjacentTiles.Add(tileArray[curCol - 1, curRow - 1], 1.5f);
                            tileArray[curCol - 1, curRow - 1].GetComponent<TileCost>().adjacentTiles.Add(tile, 1.5f);
                        }
                    }

                    // Spawn a sheep
                    chance = Random.Range(0f, 1f);
                    if (chance > .7f && spawnedSheep < rabbitAI)
                    {
                        GameObject sheep = (GameObject)Instantiate(refSheep, transform);
                        sheep.transform.position = newPos; //+ new Vector2(0.5f, 0.5f);
                        
                        switch (aiType)
                        {
                            case AIType.Naive:
                                break;

                            case AIType.Dijkstra:
                                sheep.GetComponent<DijkstraSheep2>().SetPos(curCol, curRow);
                                //hasSpawned = true;
                                spawnedSheep++;
                                break;

                            case AIType.AStar:
                                sheep.GetComponent<ASheep2>().SetPos(curCol, curRow);
                                //hasSpawned = true;
                                spawnedSheep++;
                                break;
                            case AIType.Utility:
                                sheep.GetComponent<UtilityAStar>().SetPos(curRow, curCol);
                                //hasSpawned = true;
                                spawnedSheep++;
                                break;
                        }
                    } 
                    else if (chance < .3f && spawnedSnakes < wolfAI)
                    {
                        GameObject snake = (GameObject)Instantiate(refSnake, transform);
                        if (aiType == AIType.Utility)
                        {
                            snake.GetComponent<UtilityAStar>().SetPos(curRow, curCol);
                            spawnedSnakes++;
                        }
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
        Destroy(refSnake);

        float gridL = cols * tileSize;
        float gridW = rows * tileSize;
        transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridL / 2 - tileSize / 2);

        switch (aiType)
        {
            case AIType.Naive:
                break;
            
            case AIType.Dijkstra:
                DijkstraSheep2 curDSheep = FindObjectOfType<DijkstraSheep2>();
                curDSheep.tileArray = tileArray;
                //curDSheep.FindTarget();
                break;

            case AIType.AStar:
                ASheep2 curASheep = FindObjectOfType<ASheep2>();
                curASheep.tileArray = tileArray;
                break;
            case AIType.Utility:
                RabbitUtility[] curUSheepList = FindObjectsOfType<RabbitUtility>();
                // foreach (RabbitUtility curUSheep in curUSheepList)
                // {
                //     Debug.Log("New sheep:" + curUSheep.name);
                //     curUSheep.GetComponent<UtilityAStar>().tileArray = tileArray;
                // }
                for (int i = 1; i < curUSheepList.Length; i++)
                {
                    Debug.Log("New sheep:" + curUSheepList[i].name);
                    curUSheepList[i].GetComponent<UtilityAStar>().tileArray = tileArray;
                    curUSheepList[i].GetComponent<RabbitUtility>().CloseSnakes = FindObjectsOfType<SnakeUtility>();
                }
                break;
        }
    }
}
