using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    // Health value (0 to ???)
    [SerializeField]
    private int health = 4;
    // Current sprite
    // Threshold value to make new sheep
    [SerializeField]
    private int newSheepThreshold = 4;
    // Threshold value for moving towards sheep
    [SerializeField]
    private int moveToSheepThreshold = 1;
    // Threshold value for health to move towards grass
    [SerializeField]
    private int moveToGrassThreshold = 3;
    // Threshold value for health to be below to move towards grass
    private int healthToGrassThreshold = 3;
    // Timer to do action
    [SerializeField]
    private float timeToMove = 2;
    private float timer;
    // List of all close by sheep
    private List<GameObject> closeSheep = new List<GameObject>();
    // List of all close by grass tiles
    private List<GameObject> closeGrass = new List<GameObject>();
    private bool isNextToSheep;
    private bool isOnGrass;
    [SerializeField]
    private float maxDistance = 9;
    [SerializeField]
    private float minDistance = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToMove)
        {
            Decide();
            timer = 0f;
        }
    }

    void Decide()
    {
        // If number of grass above threshold value and health below value
        if (closeGrass.Count >= moveToGrassThreshold && health <= healthToGrassThreshold)
        {
            // eat grass if on grass
            if(isOnGrass)
            {
                //GameObject currentTile;
                foreach (GameObject tile in closeGrass)
                {
                    if (Vector3.Distance(transform.position, tile.transform.position) < .1f)
                    {
                        EatGrass(tile);
                        break;
                    }
                }
            }
            // else move towards nearest grass
            else
            {
                Debug.Log("Moving towards GRass");

                float distance = Vector3.Distance(transform.position,
                                    closeGrass[0].transform.position);
                GameObject nearestTile = closeGrass[0];
                foreach (GameObject tile in closeGrass)
                {
                    if (Vector3.Distance(transform.position, tile.transform.position)
                        < distance)
                    {
                        distance = Vector3.Distance(transform.position, tile.transform.position);
                        nearestTile = tile;
                    }
                }

                Vector3 newDir = (transform.position - nearestTile.transform.position).normalized;
                transform.position += newDir;
            }
        }
        // Else if number of sheep below threshold and not next to other sheep
        else if (closeSheep.Count > moveToSheepThreshold && !isNextToSheep)
        {
            // move towards nearest sheep
            Debug.Log("Moving towards sheep");
            float distance = Vector3.Distance(transform.position,
                                    closeSheep[0].transform.position);
            GameObject nearestSheep = closeGrass[0];
            foreach (GameObject tile in closeGrass)
            {
                if (Vector3.Distance(transform.position, tile.transform.position)
                    < distance)
                {
                    distance = Vector3.Distance(transform.position, tile.transform.position);
                    nearestSheep = tile;
                }
            }

            Vector3 newDir = (transform.position - nearestSheep.transform.position).normalized;
            transform.position += newDir;
        }
        // Else if near sheep and health at or above 4
        else if (isNextToSheep && health >= newSheepThreshold)
        {
            MakeSheep();
        }
        // Else move ot new location
        else
        {
            Debug.Log("Wondering");
            Vector3 newDir = new Vector3((int)Random.Range(-1, 1), (int)Random.Range(-1, 1));
            transform.position += newDir;
        }
        CheckPosition();
    }

    void EatGrass(GameObject tile)
    {
        Debug.Log("Eating grass");
        health++;
        tile.GetComponent<TileController>().ResetGrass();
        closeGrass.Remove(tile);
    }

    void MakeSheep()
    {
        Debug.Log("Making sheep");
        // Create new sheep with 1/4 of current health
        GameObject newSheep = (GameObject)Instantiate(Resources.Load("rabbit"),transform.parent);
        newSheep.GetComponent<SheepController>().SetHealth(this.health / 4);
        // Make health half of current health
        health /= 2;
    }

    // Count number of sheep and grass near current sheep
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(typeof(SheepController), out Component component))
        {
            closeSheep.Add(other.gameObject);
        }
        else if (other.TryGetComponent(typeof(TileController), out Component component2))
        {
            closeGrass.Add(other.gameObject);
        }  
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(typeof(SheepController), out Component component))
        {
            closeSheep.Remove(other.gameObject);
        }
        else if (other.TryGetComponent(typeof(TileController), out Component component2))
        {
            closeGrass.Remove(other.gameObject);
        }  
    }

    void CheckPosition()
    {
        float xOffset = 0;
        float yOffset = 0;
        if (transform.position.x > maxDistance)
        {
            //xOffset = transform.position.x - maxDistance;
            xOffset = maxDistance - transform.position.x;
        }
        else if (transform.position.x < minDistance)
        {
            xOffset = -transform.position.x;
        }
        if (transform.position.y > minDistance)
        {
            yOffset = -transform.position.y;
        }
        else if (transform.position.y < -maxDistance)
        {
            yOffset = -(transform.position.y + maxDistance);
        }

        transform.position += new Vector3(xOffset, yOffset);
    }

    public void SetHealth(int value)
    {
        health = value;
    }
}
