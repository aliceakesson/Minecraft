using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{

    Animator animator; 

    float changeDirectionDelay = 200;
    float changeDirectionCount;
    float mobWalkingSpeed = 0.5f;
    float step; 

    float followPlayerDistance = 15; 
    GameObject player;

    Material zombieSkinDefault, zombieSkinRed, prevMat, currentMat;
    string zombieSkinURL = "Materials/";

    bool hasBeenHit = false;
    int healthCount = 20;

    GameObject zombieMesh;

    List<Collider> collidingObjects = new List<Collider>();

    Player steve;
    string steveName = "stevemc11";

    float yMargin = -100; 

    void Start()
    {
        steve = GameObject.Find(steveName).GetComponent<Player>();

        zombieMesh = this.gameObject.transform.GetChild(1).gameObject;

        player = GameObject.FindGameObjectWithTag("Player");
        step = mobWalkingSpeed * Time.deltaTime; 

        animator = this.gameObject.GetComponent<Animator>();
        changeDirectionCount = changeDirectionDelay;

        zombieSkinDefault = Resources.Load<Material>(zombieSkinURL + "zombie-material-default");
        zombieSkinRed = Resources.Load<Material>(zombieSkinURL + "zombie-material-red");

    }


    void Update()
    {

        if (transform.position.y < yMargin)
            Destroy(this.gameObject); 

        prevMat = currentMat;
        currentMat = zombieMesh.GetComponent<SkinnedMeshRenderer>().material;

        if (currentMat.name.Substring(0, 19) == zombieSkinRed.name && prevMat.name.Substring(0, 23) == zombieSkinDefault.name)
        {
            HitPlayer();
            print("zombie hit");
            healthCount -= 3;
            hasBeenHit = true;
        }
        else
            hasBeenHit = false;

        if(hasBeenHit) {
            if(healthCount <= 0)
            {
                Die();
            }
            else
            {
                //int i = 1;
                //System.Random rnd = new System.Random();
                //i = rnd.Next(1, 3);
                //FindObjectOfType<AudioManager>().Play("Zombie Hurt " + i);
                if (collidingObjects.Count != 0)
                    this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 150);
            }
        }

        if (currentMat.name.Substring(0, 19) == zombieSkinRed.name)
        {
            zombieMesh.GetComponent<SkinnedMeshRenderer>().material = zombieSkinDefault;
        }

        transform.Translate(-Vector3.forward * Time.deltaTime * mobWalkingSpeed);

        animator.Play("Walking", 0);

        if (changeDirectionCount <= 0)
        {
            float rotation = UnityEngine.Random.Range(-180f, 180f);
            transform.Rotate(new Vector3(0f, rotation, 0f), Space.Self);
            changeDirectionCount = changeDirectionDelay;
        }
        else
        {
            changeDirectionCount--;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceToPlayer <= followPlayerDistance)
        {
            Vector3 playerPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(2 * transform.position - playerPos);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collidingObjects.Contains(collision.collider) && collision.collider.tag.Equals("Block"))
            collidingObjects.Add(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collidingObjects.Contains(collision.collider) && collision.collider.tag.Equals("Block"))
            collidingObjects.Add(collision.collider);
    }

    void Die()
    {
        print("zombie die");
        Destroy(this.gameObject);
    }

    void HitPlayer()
    {
        steve.Health -= 3; 
    }
}
