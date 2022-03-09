using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterMovement : MonoBehaviour
{

    public Transform head;
    public Transform body;

    public Transform transPlayer;
    public Rigidbody rigidPlayer;
    public Transform centreOfMass;

    public float sense = 50f;
    public float speed = 6f;
    float jumpingPower = 450;

    float yRotation = 0f;
    float xRotation = 0f;

    bool jump = false;
    bool sprint = false;
    bool crouch = false; 

    bool forward = false;
    bool back = false;
    bool left = false;
    bool right = false;

    Vector3 prevPos;

    List<Collider> collidingObjects = new List<Collider>();

    Player steve;
    string steveName = "stevemc11";

    public RawImage craftingTableScreen;
    public RawImage craftingScreen; 
    bool isMovable = true; 

    void Start()
    {
        steve = GameObject.Find(steveName).GetComponent<Player>();

        Cursor.lockState = CursorLockMode.Locked;

        rigidPlayer = rigidPlayer.GetComponent<Rigidbody>();

        prevPos = transPlayer.position;

    }

    void Update()
    {

        if (craftingTableScreen.enabled || craftingScreen.enabled)
            isMovable = false;
        else
            isMovable = true; 

        
        collidingObjects.Clear();

        if(isMovable)
        {
            if (Input.GetKey("space"))
            {
                jump = true;
            }
            else
                jump = false;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                sprint = !sprint;
            }

            if (Input.GetKey(KeyCode.W))
            {
                forward = true;
            }
            else
            {
                forward = false;
            }

            if (Input.GetKey(KeyCode.A))
            {
                left = true;
            }
            else
            {
                left = false;
            }

            if (Input.GetKey(KeyCode.S))
            {
                back = true;
            }
            else
            {
                back = false;
            }

            if (Input.GetKey(KeyCode.D))
            {
                right = true;
            }
            else
            {
                right = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                crouch = !crouch;
            }

            if (sprint)
            {
                if (Vector3.Distance(prevPos, transPlayer.position) >= 1)
                {
                    prevPos = transPlayer.position;
                    steve.Exhaustion += 0.1f;
                }
            }

            //rotation
            float xR = Input.GetAxis("Mouse X") * sense * Time.deltaTime;
            float yR = Input.GetAxis("Mouse Y") * sense * Time.deltaTime;

            yRotation -= yR;
            xRotation += xR;

            if (yRotation > 90f)
                yRotation = 90f;
            else if (yRotation < -90f)
                yRotation = -90f;

            if (xRotation > 360f || xRotation < -360f)
            {
                xRotation = 0;
            }

            head.transform.rotation = Quaternion.Euler(yRotation, xRotation, 0f);
            //transPlayer.RotateAround(centreOfMass.position, Vector3.up, xR);
            transPlayer.rotation = Quaternion.Euler(0f, xRotation, 0f);
        }

    }

    void FixedUpdate()
    {

        //position
        if (forward)
        {
            transPlayer.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (left)
        {
            transPlayer.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (back)
        {
            transPlayer.Translate(-Vector3.forward * Time.deltaTime * speed);
        }
        if (right)
        {
            transPlayer.Translate(Vector3.right * Time.deltaTime * speed);
        }

        //hoppa
        if (jump)
        {
            steve.Exhaustion += 0.05f;
        }

        if (jump && !crouch && collidingObjects.Count != 0)
        {
            rigidPlayer.AddForce(transform.up * jumpingPower, ForceMode.Force);
            jump = false; 
        }
        else if (jump && crouch && collidingObjects.Count != 0)
        {
            rigidPlayer.AddForce(transform.up * (jumpingPower/2), ForceMode.Force);
            jump = false; 
        }


        //hastighet gå
        if (sprint)
            speed = 10f;
        else if (crouch)
            speed = 3f;
        else
            speed = 8f;

    }

    void Jump()
    {
        if(jump)
        {
            if (jump && !crouch && collidingObjects.Count != 0)
            {
                rigidPlayer.AddForce(transform.up * jumpingPower, ForceMode.Impulse);
            }
            else if (jump && crouch && collidingObjects.Count != 0)
            {
                rigidPlayer.AddForce(transform.up * (jumpingPower / 2), ForceMode.Impulse);
            }

            jump = false; 
            steve.Exhaustion += 0.05f;
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

}
