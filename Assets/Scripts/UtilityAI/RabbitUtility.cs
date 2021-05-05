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
    private int maxEnergy = 100;
    [SerializeField]
    private int energyRechargeRate = 5;
    [SerializeField]
    private float sleepTime = 2.5f;
    [SerializeField]
    private float anxiety = 0f;
    [SerializeField]
    private float foodNeed = 0f;
    [SerializeField]
    private float sleepNeed = 0f;
    [SerializeField]
    private int maxDistance = 10;
    // List of all close by rabbits
    [SerializeField]
    private List<GameObject> closeRabbits = new List<GameObject>();
    // List of all nearby wolves
    [SerializeField]
    private List<GameObject> closeWolves = new List<GameObject>();
    // List of all grass nearby
    [SerializeField]
    private List<GameObject> closeGrass = new List<GameObject>();
    private GameObject closestWolf;
    private GameObject closestGrass;
    private float timer = 0f;
    private float maxTime = 5f;
    private UtilityAStar aStar;

    enum State
    {
        RunAway,
        GoEat,
        Sleeping,
        Wondering
    }

    private State newState;
    private State currentState = State.Wondering;

    
    void Start()
    {
        aStar = GetComponent<UtilityAStar>();    
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= maxTime)
        {
            CalculateAnxiety();
            CalculateFoodNeed();
            CalculateSleepNeed();
            // Set a new state
            if (anxiety > foodNeed && anxiety > sleepNeed)
            {
                newState = State.RunAway;
            }
            else if (foodNeed > anxiety && foodNeed > sleepNeed)
            {
                newState = State.GoEat;
            }
            else if (sleepNeed > anxiety && sleepNeed > foodNeed)
            {
                newState = State.Sleeping;
            }
            else
            {
                newState = State.Wondering;
            }
            timer = 0f;

            // Check if the state has changed since last sense loop
            if (newState != currentState)
            {
                switch (newState)
                {
                    // Run away from the closest wolf
                    case State.RunAway:
                        currentState = State.RunAway;
                        Debug.Log("Must run");
                        StopAllCoroutines();
                        aStar.StopAllCoroutines();
                        MoveAwayFromWolf();
                        break;
                    // Find food to gain health
                    case State.GoEat:
                        currentState = State.GoEat;
                        Debug.Log("Must eat");
                        StopAllCoroutines();
                        aStar.StopAllCoroutines();
                        MoveToGrass();
                        break;
                    // Sleep to gain energy
                    case State.Sleeping:
                        currentState = State.Sleeping;
                        Debug.Log("Must sleep");
                        aStar.StopAllCoroutines();
                        StartCoroutine(RechargeEnergy());
                        break;
                    // Wonder aimlessly
                    case State.Wondering:
                        currentState = State.Wondering;
                        Debug.Log("What do");
                        StopAllCoroutines();
                        aStar.StopAllCoroutines();
                        break;
                }
            }
        }            
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
        sleepNeed = (100 - energy) / 100;
    }

    IEnumerator RechargeEnergy()
    {
        while (energy < maxEnergy)
        {
            Debug.Log("Get that sleep");
            if (energy + energyRechargeRate <= maxEnergy)
            {
                energy += energyRechargeRate;        
            }
            else
            {
                energy = maxEnergy;
            }
            yield return new WaitForSeconds(sleepTime);
        }
    }

    void MoveToGrass()
    {
        Debug.Log("Moving towards GRass");

        float distance = Vector3.Distance(transform.position, closeGrass[0].transform.position);
        closestGrass = closeGrass[0];
        foreach (GameObject tile in closeGrass)
        {
            if (Vector3.Distance(transform.position, tile.transform.position)
                < distance)
            {
                distance = Vector3.Distance(transform.position, tile.transform.position);
                closestGrass = tile;
            }
        }

        aStar.MoveToPosition(closestGrass.GetComponent<TileCost>().GetPos());
    }

    void MoveAwayFromWolf()
    {

    }
}
