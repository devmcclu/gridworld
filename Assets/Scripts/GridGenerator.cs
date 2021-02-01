using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Length of grid
    [SerializeField]
    private int rows = 5;
    // Width of grid
    [SerializeField]
    private int cols = 5;
    // Float for spacing objects
    [SerializeField]
    private float tileSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Place a tile at each (int) coordinate
        GenerateGrid();
        // Randomaly decide if a sheep will spawn
    }

    void GenerateGrid()
    {
        // Get a tile to reference
        GameObject refTile = (GameObject)Instantiate(Resources.Load("GrassTile"));
        GameObject refSheep = (GameObject)Instantiate(Resources.Load("rabbit"));
        float chance;
        for (int curRow = 0; curRow < rows; curRow++)
        {
            for (int curCol = 0; curCol < cols; curCol++)
            {
                // Create a new tile
                GameObject tile = (GameObject)Instantiate(refTile, transform);

                //Get correct position with spacing
                float posX = curCol * tileSize;
                float posY = curRow * -tileSize;

                //Set the position
                Vector2 newPos = new Vector2(posX, posY);
                tile.transform.position = newPos;

                chance = Random.Range(0f, 1f);
                if (chance > .7f)
                {
                    GameObject sheep = (GameObject)Instantiate(refSheep, transform);
                    sheep.transform.position = newPos; //+ new Vector2(0.5f, 0.5f);
                }                
            }
        }

        Destroy(refTile);
        Destroy(refSheep);

        float gridW = cols * tileSize;
        float gridL = rows * tileSize;
        transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridL / 2 - tileSize / 2);
    }
}
