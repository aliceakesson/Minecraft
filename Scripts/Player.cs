using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Player : MonoBehaviour
{
    static float hunger;
    static float saturation;
    static float exhaustion;
    static float health;

    float deltaTimeCheck = 0f;

    float previousFoodLevel, currentFoodLevel, previousHealthLevel, currentHealthLevel;

    Texture2D hungerFull, hungerHalf, hungerEmpty, healthFull, healthHalf, healthEmpty;
    string hungerIconURL = "Images/bottomInfo/";
    string healthIconsURL = "Images/bottomInfo/";

    public RawImage[] hungerIcons = new RawImage[10];
    public RawImage[] healthIcons = new RawImage[10];

    void Start()
    {

        hunger = 20;
        saturation = 0;
        exhaustion = 0;

        health = 20; 

        currentFoodLevel = hunger;
        currentHealthLevel = health; 

        hungerFull = Resources.Load<Texture2D>(hungerIconURL + "hunger_full");
        hungerHalf = Resources.Load<Texture2D>(hungerIconURL + "hunger_half");
        hungerEmpty = Resources.Load<Texture2D>(hungerIconURL + "hunger_empty");

        healthFull = Resources.Load<Texture2D>(healthIconsURL + "health_full");
        healthHalf = Resources.Load<Texture2D>(healthIconsURL + "health_half");
        healthEmpty = Resources.Load<Texture2D>(healthIconsURL + "health_empty");

    }

    
    void Update()
    {
        deltaTimeCheck += Time.deltaTime;
        if(deltaTimeCheck >= 1f) // varje sekund
        {
            deltaTimeCheck = 0f;

            previousFoodLevel = currentFoodLevel;
            previousHealthLevel = currentHealthLevel;

            /* exhaustion actions tillagda i skripter:
             * hoppa: 0.05 per hopp (charactermovement)
             * förstöra block: 0.005 per block (placeanddestroy)
             * sprinta: 0.1 per meter (?) (charactermovement)
             */

            if(exhaustion >= 4)
            {
                exhaustion = 0;
                saturation--;

                if (saturation < 0)
                    saturation = 0; 
            }

            hunger -= exhaustion;
            hunger += saturation;

            
        }

        if (hunger > 20)
            hunger = 20;
        else if (hunger < 0)
            hunger = 0;

        if (health > 20)
            health = 20;
        else if (health < 0)
            health = 0;


        currentFoodLevel = hunger;
        if (previousFoodLevel != currentFoodLevel)
            UpdateHungerBar(hunger);
        
        currentHealthLevel = health;
        if (previousHealthLevel != currentHealthLevel)
            UpdateHealthBar(health);

        if (saturation > 5f)
            saturation = 5f;

    }

    private void FixedUpdate()
    {
        
    }

    public float Hunger
    {
        get { return hunger; }
        set { hunger = value; }
    }
    public float Saturation
    {
        get { return saturation; }
        set { saturation = value; }
    }
    public float Exhaustion
    {
        get { return exhaustion; }
        set { exhaustion = value; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    void UpdateHungerBar(float points) // ej klar 
    {

        int fullHungerPoints = (int)(points / 2);
        bool halfPoint = false;
        float hungerPoints_decimals = (points / 2) % 1;

        //Debug.Log("foodlevel: " + points);
        //Debug.Log("fullhungerpoints: " + fullHungerPoints);
        //Debug.Log("halfpoint: " + halfPoint);


        if(hungerPoints_decimals == 0)
        {
            halfPoint = false; 
        }
        else if(hungerPoints_decimals <= 0.5f)
        {
            halfPoint = true;  
        }
        else
        {
            halfPoint = false;
            fullHungerPoints++; 
        }

        for(int i = 0; i < fullHungerPoints; i++) 
            hungerIcons[i].texture = hungerFull;

        if (halfPoint == true)
        {
            hungerIcons[fullHungerPoints].texture = hungerHalf;
            if((fullHungerPoints+1) < 10)
            {
                for (int i = fullHungerPoints + 1; i < 10; i++)
                    hungerIcons[i].texture = hungerEmpty;
            }
        }
        else
        {
            for (int i = fullHungerPoints; i < 10; i++)
                hungerIcons[i].texture = hungerEmpty;
        }

        

    }
    void UpdateHealthBar(float points)
    {

        int fullHealthPoints = (int)(points / 2);
        bool halfPoint = false;
        float healthPoints_decimals = (points / 2) % 1;

        //Debug.Log("healthlevel: " + points);
        //Debug.Log("fullhealthpoints: " + fullHungerPoints);
        //Debug.Log("halfpoint: " + halfPoint);


        if (healthPoints_decimals == 0)
        {
            halfPoint = false;
        }
        else if (healthPoints_decimals <= 0.5f)
        {
            halfPoint = true;
        }
        else
        {
            halfPoint = false;
            fullHealthPoints++;
        }

        for (int i = 0; i < fullHealthPoints; i++)
            healthIcons[i].texture = healthFull;

        if (halfPoint == true)
        {
            healthIcons[fullHealthPoints].texture = healthHalf;
            if ((fullHealthPoints + 1) < 10)
            {
                for (int i = fullHealthPoints + 1; i < 10; i++)
                    healthIcons[i].texture = healthEmpty;
            }
        }
        else
        {
            for (int i = fullHealthPoints; i < 10; i++)
                healthIcons[i].texture = healthEmpty;
        }


    }

}
