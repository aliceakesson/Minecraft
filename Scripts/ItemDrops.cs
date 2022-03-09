using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrops : MonoBehaviour
{

    float itemDropYMargin = 0.065f;
    float spinSpeed = 0.9f;
    bool isOnGround = false; 

    void Update()
    {
        if (isOnGround)
            Float();

    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag.Equals("Block") && !isOnGround)
        {
            isOnGround = true; 
            GetComponent<Rigidbody>().isKinematic = true;

            transform.position = new Vector3(transform.position.x, transform.position.y + itemDropYMargin, transform.position.z);

        }
    }

    void Float()
    {
        string objectName = "";

        if (gameObject.name.Substring(gameObject.name.Length - 7).Equals("(Clone)"))
            objectName = gameObject.name.Substring(0, gameObject.name.Length - 13);
        else
            objectName = gameObject.name.Substring(0, gameObject.name.Length - 6);

        if(objectName.Equals("pork"))
            transform.Rotate(0, spinSpeed, 0);
        else
            transform.Rotate(0, 0, spinSpeed);

    }

}





