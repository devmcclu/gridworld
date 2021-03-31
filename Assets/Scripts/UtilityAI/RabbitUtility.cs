using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitUtility : MonoBehaviour
{
    // Health value (0 to ???)
    [SerializeField]
    private int health = 4;
    [SerializeField]
    private int maxHealth = 4;
    [SerializeField]
    private int energy = 100;
    [SerializeField]
    private float anxiety = 0f;
    [SerializeField]
    private float foodNeed = 0f;
    [SerializeField]
    private float sleepNeed = 0f;
    [SerializeField]
    private int maxDistance = 10;
    // List of all close by sheep
    [SerializeField]
    private List<GameObject> closeSheep = new List<GameObject>();
    [SerializeField]
    private List<GameObject> closeWolves = new List<GameObject>();
    private GameObject closestWolf;
    private float timer = 0f;
    private float maxTime = 5f;
    
    void Update()
    {
        timer += Time.deltaTime;
        CalculateAnxiety();
        CalculateFoodNeed();
        CalculateSleepNeed();            
    }

    void CalculateAnxiety()
    {
        if(closeWolves.Count > 0)
        {
            closestWolf = closeWolves[0];
            foreach(GameObject wolf in closeWolves)
            {
                if (Vector2.Distance(gameObject.transform.position, wolf.transform.position) < Vector2.Distance(gameObject.transform.position, closestWolf.transform.position))
                {
                    closestWolf = wolf;
                }
            }
            anxiety = (maxDistance - Vector2.Distance(gameObject.transform.position, closestWolf.transform.position)) / maxDistance;
        }
        else
        {
            anxiety = 0;
        }
    }

    void CalculateFoodNeed()
    {
        foodNeed = (maxHealth - health) / maxDistance;
    }

    void CalculateSleepNeed()
    {
        sleepNeed = energy / 100;
    }
}
