using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderMOVEMENT : MonoBehaviour
{
    public CharacterController controller;
    public GameObject FollowGO;
    public float speed = 6f;
    public Animator animator;

    public GameManager gameManager;
    public Peer2PeerClient client;

    //shooting
    public Transform fireOrigin;
    public GameObject bulletPrefab;
    public float bulletForce = 50f;
    public float fireRate;
    private float cdTimeStamp;
    public enum Weapon
    {
        Single,
        Auto,
        Burst
    }
    public Weapon activeWeapon;

    //gravity vars
    public float gravity = -9.81f;

    [SerializeField]
    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    [SerializeField]
    private Vector3 AimingPoint;

    void Start()
    {
        activeWeapon = Weapon.Single;
    }

    void Update()
    {
        if (!gameManager.paused)
        {


            //GET AXIS MOVEMENT
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            animator.SetFloat("velocity", Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2;
            }

            //ROTATE PLAYER TOWARDS MOUSE  WORLD POSITION   

            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitDist = 0f;

            if (playerPlane.Raycast(ray, out hitDist))
            {

                Vector3 targetPoint = ray.GetPoint(hitDist);
                AimingPoint = targetPoint;
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                targetRotation.x = 0;
                targetRotation.z = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
            }

            //WHEN AIMING SET FOLLOWGO GAMEOBJECT TO TARGET
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Debug.DrawRay(transform.position, AimingPoint, Color.green);
                Vector3 directionToTarget = (AimingPoint - transform.position);
                float ToTargetDistance = directionToTarget.magnitude;
                FollowGO.transform.position = directionToTarget.normalized * (ToTargetDistance);//here we should apply a percentage of the length we want the camera to be centered, but as we are using cinemachine the camera computes an average between the group target which I set up
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                FollowGO.transform.position = transform.position;
            }

            //MOVE
            if (direction.magnitude >= 0.1f)
            {
                controller.Move(direction.normalized * speed * Time.deltaTime);
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            //SHOOTING
            if (Input.GetButtonDown("Fire1"))
            {
                if (Time.time > cdTimeStamp)
                {
                    animator.SetTrigger(WeaponToString());
                    StartCoroutine("Shoot");
                    cdTimeStamp = Time.time + fireRate;
                }
            }
            // if (Input.GetButton("Fire1"))
            // {
            //     if (Time.time > cdTimeStamp)
            //     {
            //         animator.SetTrigger(WeaponToString());
            //         Shoot();
            //         cdTimeStamp = Time.time + fireRate;
            //     }
            // }
        }
    }

    IEnumerator Shoot()
    {
        switch (activeWeapon)
        {
            case Weapon.Single:
                {
                    GameObject bullet = Instantiate(bulletPrefab, fireOrigin.position, fireOrigin.rotation);
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.AddForce(fireOrigin.up * bulletForce, ForceMode.Impulse);
                    break;
                }

            case Weapon.Burst:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject bullet = Instantiate(bulletPrefab, fireOrigin.position, fireOrigin.rotation);
                        Rigidbody rb = bullet.GetComponent<Rigidbody>();
                        rb.AddForce(fireOrigin.up * bulletForce, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                }

            default:
                break;
        }
        yield return null;
    }

    string WeaponToString()
    {
        switch (activeWeapon)
        {
            case Weapon.Single:
                return "shooting_s";

            case Weapon.Auto:
                return "shooting_a";

            case Weapon.Burst:
                return "shooting_b";

            default:
                return "shooting_s";
        }
    }
}







