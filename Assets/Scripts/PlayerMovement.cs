using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -13f;
    public float jumpHeight = 20f;
    public bool isSprinting = false, isGrounded = false;
    public float sprintingMultiplier;
    public float movementSpeed = 1f;
    public float currentVelY = 0;
    private AudioSource stepSound;
    public GameObject npc;
    public GameObject aCamera;
    public bool isMachineGunGrabbed, isGunGrabbed = false;
    public static PlayerMovement singelton;
    public GameObject gun;
    public GameObject machineGun;
    public Text textForGun;
    public Text textForMachineGun;



    public Transform groundCheck;
    public float groundDistance = 2f;
    public LayerMask groundMask;

    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        stepSound = GetComponent<AudioSource>();
    }
    private void Awake()
    {
        singelton = this;

    }
    void npcStand()
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        Animator animator = npc.GetComponent<Animator>();
        animator.SetInteger("state", 3);
        //agent.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(aCamera.transform.position, aCamera.transform.forward, out hit))
        {
            if (hit.transform.tag == "MachineGun")
            {
                float distanceFromPlayer = Vector3.Distance(this.transform.position, hit.transform.position);
                if (distanceFromPlayer < 70 && !isMachineGunGrabbed)
                {
                    textForMachineGun.transform.gameObject.SetActive(true);
                    textForMachineGun.text = "Press [e] to take the machine gun";
                    textForMachineGun.enabled = true;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        textForMachineGun.text = "";
                        hit.transform.gameObject.SetActive(false);
                        machineGun.SetActive(true);
                        gun.SetActive(false);
                        isMachineGunGrabbed = true;
                        textForMachineGun.enabled = false;
                    }
                }
            }
            else if (hit.transform.tag == "Gun")
            {
                float distanceFromPlayer = Vector3.Distance(this.transform.position, hit.transform.position);
                if (distanceFromPlayer < 70 && !isGunGrabbed)
                {

                    textForGun.transform.gameObject.SetActive(true);
                    textForGun.text = "Press [e] to take the gun";
                    textForGun.enabled = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        textForGun.enabled = false;
                        machineGun.SetActive(false);
                        hit.transform.gameObject.SetActive(false);
                        gun.SetActive(true);
                        isGunGrabbed = true;
                    }
                }
            }
        }
        float distance = Vector3.Distance(npc.transform.position, this.transform.position);
        if (distance < 25)
        {
            npcStand();
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
        if (z < -0.1 || z > 0.1 || x < -0.1 || x > 0.1)
        {

            if (!stepSound.isPlaying)
            {
                stepSound.Play();
            }
            if (distance > 25)
            {
                NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
                agent.enabled = true; // this starts npc motion
                                      // and let npc walk
                Animator animator = npc.GetComponent<Animator>();
                // Debug.Log(animator.GetInteger("state"));
                animator.SetInteger("state", 1);
            }
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        Vector3 movement = new Vector3();

        movement = inputX * transform.right + inputY * transform.forward;
        if (isGrounded == false)
        {
            currentVelY += gravity * Time.deltaTime;
        }
        else if (isGrounded == true)
        {
            currentVelY = -2f;
        }


        if (isSprinting == true)
        {
            movement *= sprintingMultiplier;
        }

        controller.Move(movement * movementSpeed * Time.deltaTime);
        controller.Move(new Vector3(0, currentVelY * Time.deltaTime, 0));
    }
}


