using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RabbitRoam : MonoBehaviour
{
    Animator animator;
    
    public enum Posture
    {
        Idel,
        Run,
    }

    public float moveSpeed = 1f;

    float walkTime;
    public float walkTimeCounter;
    float waitTime;
    public float waitTimeCounter;

    int walkDirection;

    public bool isWalking;

    void Start()
    {
        animator = GetComponent<Animator>();

        walkTime = Random.Range(2, 8);
        waitTime = Random.Range(3, 10);

        ChooseDirection();
    }
    
    public void ChooseDirection()
    {
        walkDirection = Random.Range(0, 8);

        isWalking = true;
        walkTimeCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking == true)
        {
            animator.SetBool("IsRunning", true);

            walkTimeCounter += Time.deltaTime;

            switch (walkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 45, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 135, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 4:
                    transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 5:
                    transform.localRotation = Quaternion.Euler(0f, 225, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 6:
                    transform.localRotation = Quaternion.Euler(0f, 270, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 7:
                    transform.localRotation = Quaternion.Euler(0f, 315, 0f);
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }
            
            if (walkTimeCounter >= walkTime)
            {
                isWalking = false;
                animator.SetBool("IsRunning", false);
                waitTimeCounter = 0f;
            }

        }

        else if(isWalking == false)
        {
            waitTimeCounter += Time.deltaTime;

            if(waitTimeCounter >= waitTime)
            {
                ChooseDirection();
            }

        }
    }
}
