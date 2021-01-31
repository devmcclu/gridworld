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
    private int moveToSheepThreshold = 2;
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
                EatGrass();
            }
            // else move towards nearest grass
            else
            {

            }
        }
        // Else if number of sheep below threshold and not next to other sheep
        else if (closeSheep.Count < moveToSheepThreshold && !isNextToSheep)
        {
            // move towards nearest sheep
        }
        // Else if near sheep and health at or above 4
        else if (isNextToSheep && health >= newSheepThreshold)
        {
            MakeSheep();
        }
        // Else move ot new location
        else
        {
            Vector3 newDir = new Vector3((int)Random.Range(-1, 1), (int)Random.Range(-1, 1));
            transform.position += newDir;
        }
    }

    void EatGrass()
    {

    }

    void MakeSheep()
    {
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

    public void SetHealth(int value)
    {
        health = value;
    }
}
