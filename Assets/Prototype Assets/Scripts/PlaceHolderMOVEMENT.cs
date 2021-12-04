using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderMOVEMENT : MonoBehaviour
{



    public CharacterController controller;

    public GameObject FollowGO;

    

    public float speed = 6f;

    //gravity vars

    public float gravity = -9.81f;

    [SerializeField]
    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance=0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    [SerializeField]
   private Vector3 AimingPoint;

  
    void Start()
    {

    }

    void Update()
    {


        //GET AXIS MOVEMENT
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertiacal).normalized;
        
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
             
        

        //ROTATE PLAYER TOWARDS MOUSE  WORLD POSITION   

        Plane playerPlane = new Plane(Vector3.up,transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist =0f;

        if(playerPlane.Raycast(ray, out hitDist))
        {

            Vector3 targetPoint = ray.GetPoint(hitDist);
            AimingPoint = targetPoint;
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint-transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,7f*Time.deltaTime);


        }
     
        //WHEN AIMING SET FOLLOWGO GAMEOBJECT TO TARGET
        if (Input.GetKey(KeyCode.LeftShift))
        {

         
            Debug.DrawRay(transform.position, AimingPoint, Color.green);
            Vector3 directionToTarget = (AimingPoint - transform.position);
            float ToTargetDistance = directionToTarget.magnitude;
            FollowGO.transform.position = directionToTarget.normalized * (ToTargetDistance);//here we should apply a percentage of the length we want the camera to be centered, but as we are using cinemachine the camera computes an average between the group target which I set up

        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
           
            
            FollowGO.transform.position = transform.position;

        }


       
      

        //MOVE
        if (direction.magnitude>=0.1f)
        {

           controller.Move(direction.normalized * speed * Time.deltaTime);
           

        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);





    }



   


}







