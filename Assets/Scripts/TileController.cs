using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Grass health value
    private int health = 0;
    private int maxHealth = 3;
    // Current sprite
    private Sprite curSprite;
    private SpriteRenderer spriteRenderer;
    // Sprites for grass health
    [SerializeField]
    private Sprite[] spriteList;
    // Timer to add health and check growth
    [SerializeField]
    private float timeToGrow;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set health value
        health = (int)Random.Range(0, 4);
        ChangeSprite();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToGrow)
        {
            Grow();
            timer = 0f;
        }
    }

    void ChangeSprite()
    {
        spriteRenderer.sprite = spriteList[health];
    }

    void CheckGrowth()
    {
        float chance = Random.Range(0, 100);
        if (chance > 50)
        {
            health = 1;
            ChangeSprite();
        }
    }

    void Grow()
    {
        /*
            If health is 0:
                Randomly decide to grow grass
            If has grass:
                Add health
                Change sprite
                if health over max value:
                    reset to dirt
        */
        if (health == 0)
        {
            CheckGrowth();
        }
        else if (health > 0)
        {
            health++;
            if (health > maxHealth)
            {
                health = 0;
            }
            ChangeSprite();
        }
    }

    public void ResetGrass()
    {
        health = 0;
        ChangeSprite();
    }

    public int GetHealth()
    {
        return health;
    }
}
