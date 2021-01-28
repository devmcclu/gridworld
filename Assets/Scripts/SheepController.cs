using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    // Health value (0 to ???)
    // Current sprite
    // Threshold value to make new sheep
    // Threshold value for moving towards grass
    // Threshold value for moving towards sheep
    // Threshold value for health to move towards grass

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        Look around radius of 2 tiles away
        Count number of sheep around
        Count amount of grass around
        If number of grass above threshold value and health below value: 
            eat grass if on grass
            else move towards nearest grass
        Else if number of sheep below threshold and not next to other sheep:
            move towards nearest sheep
        Else if near sheep and health at or above 4:
            Create new sheep with 1/4 of current health
            Make health half of current health
        Else:
            Move to new location
        */ 
    }
}
