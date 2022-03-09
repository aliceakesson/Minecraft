using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreeperController : MonoBehaviour
{

    float explodeDistance = 6;
    GameObject player;
    float step;
    float mobWalkingSpeed = 0.5f;
    bool isMoving = true; 

    float explodeDelay = 3;
    float explodeTimer;
    float explodeRadius = 4;

    float followPlayerDistance = 15;
    float currentDistance, previousDistance;

    float explodeScaleMultiplier = .05f;
    float timer = 0;
    float scale = 0.6f;

    string itemsURL = "Prefabs/Items/";
    Transform itemDropSpawnParent;
    float itemDropWidth = 0.2f;

    Player steve;
    string steveName = "stevemc11";

    float yMargin = -100; 

    void Start()
    {
        steve = GameObject.Find(steveName).GetComponent<Player>();

        player = GameObject.FindGameObjectWithTag("Player");
        step = mobWalkingSpeed * Time.deltaTime;
        explodeTimer = explodeDelay;

        currentDistance = Vector3.Distance(transform.position, player.transform.position);

        itemDropSpawnParent = GameObject.Find("itemDropSpawnParent").transform;


    }

    void Update()
    {

        if (transform.position.y < yMargin)
            Destroy(this.gameObject);

        previousDistance = currentDistance;
        currentDistance = Vector3.Distance(transform.position, player.transform.position);
        if (previousDistance <= explodeDistance && currentDistance > explodeDistance) //starta om timer om man går bort från spelare 
        {
            explodeTimer = explodeDelay;
            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            isMoving = true; 
        }
        
        timer += Time.deltaTime;
        if(timer >= 1f) // för varje sekund
        {
            timer = 0;
            if (currentDistance <= explodeDistance && explodeTimer > 0) // fortsätt nedräkning om man är nära nog spelaren 
                explodeTimer--;

        }
        else if(timer >= .25f) //var fjärdedels sekund
        {
            if (explodeTimer < explodeDelay) //om creeper håller på att explodera
            {
                isMoving = false;
                float scaleValue = (explodeScaleMultiplier / explodeDelay) / 4;
                Vector3 scaleChange = new Vector3(scaleValue, scaleValue, scaleValue);
                this.gameObject.transform.localScale += scaleChange;
            }
        }
        

        if (currentDistance <= followPlayerDistance && isMoving)
        {
            Vector3 playerPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(playerPos);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        }

        if (explodeTimer <= 0)
            Explode();

    }

    void Explode()
    {
        print("explodera");

        Collider[] blocksToDestroy = Physics.OverlapSphere(transform.position, explodeRadius, 1 << 8); // 1 << 8 vad betyder det ??
        for(int i = 0; i < blocksToDestroy.Length; i++)
        {
            SpawnItemDrop(blocksToDestroy[i]);
            Destroy(blocksToDestroy[i].gameObject);
        }

        steve.Health -= 43; 

        Destroy(this.gameObject);

    }

    void SpawnItemDrop(Collider col)
    {

        Vector3 colCenter = col.transform.position;

        Collider clone = new Collider();

        if (col.gameObject.name.Substring(0, 13).Equals("dirtTopPrefab"))
        {
            clone = Instantiate(Resources.Load<GameObject>(itemsURL + "dirtPrefab").GetComponent<Collider>(), new Vector3(colCenter.x, colCenter.y, colCenter.z), col.transform.rotation, itemDropSpawnParent);
        }
        else
        {
            clone = Instantiate(col, new Vector3(colCenter.x, colCenter.y, colCenter.z), col.transform.rotation, itemDropSpawnParent);
        }

        clone.gameObject.tag = "ItemDrop";
        clone.gameObject.AddComponent<ItemDrops>();

        BoxCollider boxCol = clone.gameObject.GetComponent<BoxCollider>();

        boxCol.size = new Vector3(boxCol.size.x, boxCol.size.y, boxCol.size.z);

        float sizeX = clone.GetComponent<Renderer>().bounds.size.x;
        float sizeY = clone.GetComponent<Renderer>().bounds.size.y;
        float sizeZ = clone.GetComponent<Renderer>().bounds.size.z;

        Vector3 rescale = clone.transform.localScale;

        rescale.x = itemDropWidth * rescale.x / sizeX;
        rescale.y = itemDropWidth * rescale.y / sizeY;
        rescale.z = itemDropWidth * rescale.z / sizeZ;

        clone.transform.localScale = rescale;

        Rigidbody boxRig = clone.gameObject.AddComponent<Rigidbody>();
        boxRig.freezeRotation = true;

        float forceX = UnityEngine.Random.Range(-2f, 2f);
        float forceZ = UnityEngine.Random.Range(-2f, 2f);
        boxRig.AddForce(forceX, 2f, forceZ, ForceMode.Impulse);
        

    }

}
