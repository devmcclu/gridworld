using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeUtility : MonoBehaviour
{
    private UtilityAStar aStar;
    private float timer = 0f;
    private float maxTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        aStar = GetComponent<UtilityAStar>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= maxTime)
        {
            timer = 0;
        }
    }
}
