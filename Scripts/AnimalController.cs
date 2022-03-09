using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{

    float changeDirectionDelay = 200;
    float changeDirectionCountdown;

    Material defaultMaterial;
    Material redMaterial;

    float hurtTime = 1.5f; //1.5 sekunder????

    Transform itemDropSpawnParent;

    int healthCount = 3; //notering: riktigt antal för att döda gris utan vapen är 10-12 slag (efter tester) beroende på hur mycket fläsk osv man får 

    string itemsURL = "Prefabs/Items/";
    string materialURL = "Materials/";

    Transform objectTransform;

    Material prevMat; 
    Material currentMat;

    float itemDropWidth = 0.2f;

    bool hasBeenHit = false;

    float animalWalkingSpeed = 0.5f;

    List<Collider> collidingObjects = new List<Collider>();

    float yMargin = -100;


    void Start()
    {
        changeDirectionCountdown = changeDirectionDelay;

        objectTransform = this.gameObject.transform;

        defaultMaterial = Resources.Load<Material>(materialURL + "pig-material-default");
        redMaterial = Resources.Load<Material>(materialURL + "pig-material-red");

        itemDropSpawnParent = GameObject.Find("itemDropSpawnParent").transform;

    }

    void Update() //y 0 x -90
    {

        transform.rotation = Quaternion.Euler(-90, 0, transform.rotation.z);

        if (transform.position.y < yMargin)
            Destroy(this.gameObject);

        transform.Translate(Vector3.left * Time.deltaTime * animalWalkingSpeed);

        prevMat = currentMat;
        currentMat = this.gameObject.GetComponent<MeshRenderer>().material;

        if (currentMat.name.Substring(0, 16) == redMaterial.name && prevMat.name.Substring(0, 20) == defaultMaterial.name)
        {
            healthCount--;
            hasBeenHit = true;
        }
        else
            hasBeenHit = false;

        WaitIfHit(); //funkar inte 

        if(hasBeenHit)
        {

            if (healthCount <= 0)
                Die();
            else
            {
                //StartCoroutine(Wait(hurtTime * Time.deltaTime));
                int i = 1;
                System.Random rnd = new System.Random();
                i = rnd.Next(1,3);
                FindObjectOfType<AudioManager>().Play("Pig Hurt " + i);
                if(collidingObjects.Count != 0)
                    this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 150);
            }
        }

        if (currentMat.name.Substring(0, 16) == redMaterial.name)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        if (changeDirectionCountdown <= 0)
        {
            float rotation = UnityEngine.Random.Range(-180f, 180f);
            transform.Rotate(new Vector3(0f, 0f, rotation), Space.Self);
            changeDirectionCountdown = changeDirectionDelay;
        }
        else
        {
            changeDirectionCountdown--; 
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

    IEnumerator WaitIfHit()
    {
        if (hasBeenHit)
            yield return new WaitForSeconds(1);
        else
            yield return false; 
    }

    void Die()
    {
        FindObjectOfType<AudioManager>().Play("Pig Death");

        this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 70);

        for (float f = 90f; f > 0; f--) //vända sig ner
        {
            StartCoroutine(Wait(0.05f * Time.deltaTime)); //???
            this.gameObject.transform.Rotate(0f, 1f, 0f, Space.Self);
            Material mat = this.gameObject.GetComponent<MeshRenderer>().material;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, f/90f); //?????
        }

        StartCoroutine(Wait(2f * Time.deltaTime));

        Destroy(this.gameObject);

        GameObject pork = Resources.Load<GameObject>(itemsURL + "porkPrefab"); 
        GameObject porkClone = Instantiate(pork, objectTransform.position, Quaternion.identity, itemDropSpawnParent);

        porkClone.tag = "ItemDrop";
        porkClone.AddComponent<BoxCollider>();
        porkClone.AddComponent<ItemDrops>();
        porkClone.AddComponent<Rigidbody>();

    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
