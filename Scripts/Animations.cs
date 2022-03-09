using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{

    public Animator animator;
    bool isWalking = false;
    bool isCrouching = false;
    bool isSprinting = false;

    public Camera firstPerson;

    void Start()
    {

        //Time.timeScale = 0f;
        //animator.updateMode = AnimatorUpdateMode.UnscaledTime;

    }

    void Update()
    {
        
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            isWalking = true; 
        }
        else
        {
            isWalking = false; 
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isCrouching = !isCrouching;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSprinting = !isSprinting;
        }


        if(!firstPerson.enabled)
        {
            if (isWalking && !isCrouching && !isSprinting)
            {
                animator.Play("Walking", 0);
            }
            else if (isWalking && !isCrouching && isSprinting)
            {
                animator.Play("Sprinting", 0);
            }
        }

        if (!isWalking && !isCrouching)
        {
            animator.Play("Idle", 0);
        }
        else if (isWalking && isCrouching)
        {
            animator.Play("CrouchWalking", 0);
        }
        else if (!isWalking && isCrouching)
        {
            animator.Play("CrouchIdle", 0);
        }

    }
}
