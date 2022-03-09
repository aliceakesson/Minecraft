using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ljusaste
 * light intensity: 1.5f
 * sky exposure = 1.1f; 
 * 
 * mörkaste 
 * light intensity: 0f
 * sky exposure: 0.65f
 * 
 * start
 * light intensity: (18 * 60 + 47) * (1.5 / ((20 * 60) - (18 * 60 + 47) + (11 * 60 + 32))) = 2.20980392157
 * sky exposure: (18 * 60 + 47) * ((1.1 - 0.65) / ((20 * 60) - (18 * 60 + 47) + (11 * 60 + 32))) = 0.66294117647
 * 
 * spel startar vid 6:00 (0 minuter)
 */

/*	
 *	start
 *	light intensity = 1.1f
 *	sky exposure = 1.1f
 */
public class TimeAndSpawn : MonoBehaviour
{

    // mobs 
    float timer;
    int secondsPassed = 0;

    int startSpawnTimeInSeconds = 10 * 60 + 48;
    int stopSpawnTimeInSeconds = 19 * 60;

    //int burnTimeInSeconds = 19 * 60 + 30; 

    int oneDayInSeconds = 20 * 60;

    public GameObject zombie;
    public GameObject creeper; 

    public GameObject player;

    float mobSpawnDelay = 10; // 1 sekund(?)
    float mobSpawnCount;
    float spawnDistance_max = 48; //35
    float spawnDistance_min = 20;

    float disableScriptDistance = 50; //30

    public Transform mobSpawnParent;

    //animals 
    public Transform animalSpawnParent;
    float animalSpawnDelay = 10; // 1 sekund(?)
    float animalSpawnCount;

    public GameObject pig;

    //time
    public Light light;
    public Material sky;

    int minutesPassed;
    float timerRotation;

    float highestIntensity = 1.5f;
    float lowestIntensity = 0f;

    float highestExposure = 1.1f;
    float lowestExposure = 0.65f;

    float startExposure = 0.9f;
    float startIntensity = 1.1f;

    int stopBrighterSeconds = 23;
    int startDarkerSeconds = 6 * 60 + 25;
    int stopDarkerSeconds = 11 * 60 + 32;
    int startBrighterSeconds = 18 * 60 + 47;

    int oneDaySeconds = 20 * 60;

    float skyRotation = 0.05f;

    //ta bort block som är för långt borta + ev.block optimering rendering 
    int despawnBlockDistance = 40;
    float positionMargin = 0.1f;

    public GameObject dirt, dirtTop, sand;
    public Transform blockSpawnParent;

    float boxCollideMargin = 0.2f;

    bool burnMobs = false;

    string verticalDirection = " ", horizontalDirection = " ";
    float prevVertical, prevHorizontal, vertical, horizontal;

    //dkljfhkasldhf
    float spawnHeight = 10; 

    void Start()
    {
        timer = 0;
        secondsPassed = 0;

        secondsPassed = 0;
        minutesPassed = 0;
        timerRotation = 0;

        light.intensity = startIntensity;
        sky.SetFloat("_Exposure", startExposure);
        sky.SetFloat("_Rotation", 0);

        mobSpawnCount = mobSpawnDelay;
        animalSpawnCount = animalSpawnDelay;

        vertical = player.transform.position.z;
        horizontal = player.transform.position.x;

    }

    

    void Update()
    {

        prevVertical = vertical;
        prevHorizontal = horizontal;
        vertical = player.transform.position.z;
        horizontal = player.transform.position.x;

        if (vertical >= prevVertical) // om vertical ökar 
            verticalDirection = "n";
        else
            verticalDirection = "s";

        if (horizontal >= prevHorizontal)
            horizontalDirection = "e";
        else
            horizontalDirection = "w";

        //Collider[] objectsCloseToPlayer = Physics.OverlapSphere(player.transform.position, spawnDistance_max);
        Collider[] objectsCloseToPlayer = Physics.OverlapBox(player.transform.position, new Vector3(spawnDistance_max, spawnDistance_max, spawnDistance_max));
        int amountOfAnimals = 0;
        int amountOfMobs = 0; 

        foreach (Collider foundCollider in objectsCloseToPlayer)
        {
            float distanceToPlayer = Vector3.Distance(foundCollider.gameObject.transform.position, player.transform.position);

            if (foundCollider.tag == "Mob")
            {
                amountOfMobs++; 
                if(foundCollider.gameObject.name.Substring(0, 6).Equals("zombie"))
                {
                    if (distanceToPlayer > disableScriptDistance)
                    {
                        foundCollider.GetComponent<ZombieController>().enabled = false;
                        foundCollider.GetComponent<Animator>().enabled = false;
                        foundCollider.GetComponent<Rigidbody>().isKinematic = true; 
                    }
                    else
                    {
                        foundCollider.GetComponent<ZombieController>().enabled = true;
                        foundCollider.GetComponent<Animator>().enabled = true;
                        foundCollider.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
                else if(foundCollider.gameObject.name.Substring(0, 7).Equals("creeper"))
                {
                    if (distanceToPlayer > disableScriptDistance)
                    {
                        foundCollider.GetComponent<CreeperController>().enabled = false;
                        foundCollider.GetComponent<Animator>().enabled = false;
                        foundCollider.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    else
                    {
                        foundCollider.GetComponent<CreeperController>().enabled = true;
                        foundCollider.GetComponent<Animator>().enabled = true;
                        foundCollider.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }

                if (burnMobs)
                    Destroy(foundCollider.gameObject);
            }
            else if (foundCollider.tag == "Animal")
            {
                amountOfAnimals++;
                if (distanceToPlayer > disableScriptDistance)
                {
                    foundCollider.gameObject.GetComponent<AnimalController>().enabled = false;
                    foundCollider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    foundCollider.gameObject.GetComponent<AnimalController>().enabled = true;
                    foundCollider.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                }

            }
            //else if (foundCollider.tag == "Block" && (foundCollider.gameObject.name.Substring(0, 4).Equals("dirt") || foundCollider.gameObject.name.Substring(0, 4).Equals("sand")))
            //{
            //    float x = Mathf.Abs(player.transform.position.x - foundCollider.gameObject.transform.position.x);
            //    float z = Mathf.Abs(player.transform.position.z - foundCollider.gameObject.transform.position.z);

            //    float xMargin = Mathf.Abs(x - despawnBlockDistance);
            //    float zMargin = Mathf.Abs(z - despawnBlockDistance);

            //    if ((xMargin <= 2 || zMargin <= 2) && Mathf.Sqrt(x * x + z * z) <= despawnBlockDistance * Mathf.Sqrt(2))
            //    {
            //        Destroy(foundCollider.gameObject);
            //    }

            //    if ((verticalDirection.Equals("n") && (player.transform.position.z - foundCollider.gameObject.transform.position.z > 0)) ||
            //        verticalDirection.Equals("s") && (player.transform.position.z - foundCollider.gameObject.transform.position.z < 0) &&
            //        (horizontalDirection.Equals("e") && (player.transform.position.x - foundCollider.gameObject.transform.position.x > 0)) ||
            //        verticalDirection.Equals("w") && (player.transform.position.x - foundCollider.gameObject.transform.position.x < 0))
            //    {
            //        if ((xMargin <= 2 || zMargin <= 2) && Mathf.Sqrt(x * x + z * z) <= despawnBlockDistance * Mathf.Sqrt(2))
            //        {
            //            Destroy(foundCollider.gameObject);
            //        }
            //    }
            //}
        }


        timer += Time.deltaTime;
        if (timer >= 1f) // 1f
        {
            timer = 0;
            secondsPassed++;

            if (secondsPassed % 60 == 0 && secondsPassed != 0)
            {
                minutesPassed++;
                Debug.Log("minuter: " + minutesPassed);
            }

            //mobs
            if (secondsPassed < 0)
            {
                secondsPassed = 0;
            }
            else if (secondsPassed >= startSpawnTimeInSeconds && secondsPassed < stopSpawnTimeInSeconds)
            {
                burnMobs = false; 

                if (mobSpawnCount <= 0 && amountOfMobs <= 10)
                {
                    mobSpawnCount = 0;
                    float rnd = UnityEngine.Random.Range(0f, 1f);
                    if (rnd < 0.2f) //en femtedels chans att en mob spawnar
                    {
                        SpawnMob();
                    }
                    mobSpawnCount = mobSpawnDelay;
                }
                else
                {
                    mobSpawnCount--;
                }
            }
            else if (secondsPassed == stopSpawnTimeInSeconds)
            {
                //bränn upp kvarlevande mobs 
                burnMobs = true; 
            }
            else if (secondsPassed >= oneDayInSeconds)
            {
                secondsPassed = 0;
            }

            //daytimesystem
            if (secondsPassed < stopBrighterSeconds)
            {
                light.intensity += (highestIntensity - startIntensity) / stopBrighterSeconds;
                sky.SetFloat("_Exposure", sky.GetFloat("_Exposure") + (highestExposure - startExposure) / stopBrighterSeconds);
            }
            else if ((secondsPassed >= startDarkerSeconds) && (secondsPassed < stopDarkerSeconds))
            {
                light.intensity -= ((highestIntensity - lowestIntensity) / (stopDarkerSeconds - startDarkerSeconds));
                sky.SetFloat("_Exposure", sky.GetFloat("_Exposure") - (highestExposure - lowestExposure) / (stopDarkerSeconds - startDarkerSeconds));
            }
            else if (secondsPassed >= startBrighterSeconds && secondsPassed < oneDaySeconds)
            {
                light.intensity += ((startIntensity - lowestIntensity) / (oneDaySeconds - startBrighterSeconds));
                sky.SetFloat("_Exposure", sky.GetFloat("_Exposure") + ((startExposure - lowestExposure) / (oneDaySeconds - startBrighterSeconds)));
            }
            else if (secondsPassed >= oneDaySeconds)
            {
                secondsPassed = 0;
                minutesPassed = 0;
                light.intensity = startIntensity;
                sky.SetFloat("_Exposure", startExposure);
            }
        }

        

        if (animalSpawnCount <= 0 && amountOfAnimals <= 5)
        {
            animalSpawnCount = 0;
            float rnd = UnityEngine.Random.Range(0f, 1f);
            if (rnd < 0.2f) //en femtedels chans att ett djur spawnar
            {
                SpawnAnimal();
            }
            animalSpawnCount = animalSpawnDelay;
        }
        else
        {
            animalSpawnCount--;
        }

        timerRotation += Time.deltaTime;
        if (timerRotation >= 0.1f) // per tiondels sekund
        {
            timerRotation = 0;

            if (sky.GetFloat("_Rotation") > 359)
            {
                sky.SetFloat("_Rotation", 0);
            }
            else
            {
                sky.SetFloat("_Rotation", sky.GetFloat("_Rotation") + skyRotation);
            }
        }
    }

    void SpawnMob()
    {

        Vector3 clonePos = UnityEngine.Random.insideUnitSphere * spawnDistance_max + player.transform.position;

        float distance = Vector3.Distance(clonePos, player.transform.position);

        while (distance < spawnDistance_min)
        {
            clonePos = UnityEngine.Random.insideUnitSphere * spawnDistance_max;
            distance = Vector3.Distance(clonePos, player.transform.position);
        }

        GameObject mobClone; 

        int i = Random.Range(0, 2);
        if(i == 0)
            mobClone = Instantiate(zombie, new Vector3(clonePos.x, spawnHeight, clonePos.z), Quaternion.Euler(0f, 0f, 0f), mobSpawnParent);
        else
            mobClone = Instantiate(creeper, new Vector3(clonePos.x, spawnHeight, clonePos.z), Quaternion.Euler(0f, 0f, 0f), mobSpawnParent);

        float rotation = UnityEngine.Random.Range(-180f, 180f);

        mobClone.transform.Rotate(new Vector3(0f, rotation, 0f), Space.Self);

    }
    void SpawnAnimal()
    {

        Vector3 clonePos = UnityEngine.Random.insideUnitSphere * spawnDistance_max + player.transform.position;

        float distance = Vector3.Distance(clonePos, player.transform.position);

        while (distance < spawnDistance_min)
        {
            clonePos = UnityEngine.Random.insideUnitSphere * spawnDistance_max;
            distance = Vector3.Distance(clonePos, player.transform.position);
        }

        GameObject animalClone = Instantiate(pig, new Vector3(clonePos.x, spawnHeight, clonePos.z), Quaternion.Euler(-90f, 0f, 0f), animalSpawnParent);

        float rotation = UnityEngine.Random.Range(-180f, 180f);

        animalClone.transform.Rotate(new Vector3(0f, 0f, rotation), Space.Self);

    }
}
