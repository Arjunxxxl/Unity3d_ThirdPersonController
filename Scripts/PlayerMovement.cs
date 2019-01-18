using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSettings
    {
        public string forward = "forward";
        public string strafe = "strafe";
        public string jump = "Jump";
        public string isGrounded = "isGrounded";
        public string index = "BlendTreeIndex";
        public string Jump_onPlatform = "Jump_onPlatform";

        public string jumptoFall = "jumptoFall";
    }
    [SerializeField]
    public AnimationSettings animSettings;

    [System.Serializable]
    public class InputSettings
    {
        public float Horizontal;
        public float Vertical;
        public float mouseX;
        public float mouseY;
        public bool jump;

        public bool isGrounded;
        public bool groundConformation;
        public bool Jump_onPlatform1 = false;
        public bool Jump_onPlatform1_1 = false;
    }
    [SerializeField]
    public InputSettings inputSettings;

    [System.Serializable]
    public class MovementSettings
    {
        public float jump_time = 0.19f;
        public float jumpSpeed;
        public float groundConfirmTime = 3f;
        public float lookDistance = 10f;
        public float lookSpeed = 10f;
    }
    [SerializeField]
    public MovementSettings movementSettings;

    [System.Serializable]
    public class PhysicsSettings
    {
        public float gravityModifier = 9.81f;
        public float baseGravity = 50.0f;
        public float resetGravityValue = 1.2f;
        public LayerMask groundLayers;
        public float airSpeed = 2.5f;

        public float rayDistance = 100f;
    }
    [SerializeField]
    public PhysicsSettings physics;

    [System.Serializable]
    public class RaySettings
    {
        public LayerMask rayMask;

        
        public GameObject jumpOnto_Top_Obj;
        public bool jumpOnto;
        public bool jumpOnto_topObj;
        public GameObject jumpOnto_Obj;
        public float time1 = 2f;
        public float time1_1 = 2f;
        public float dis1 = 1f;
        public float dis1_1 = 1.5f;
        public float upSpeed = 2f;
        public float onto_distance;
    }
    [SerializeField]
    public RaySettings raySettings;

    Animator anim;
    CharacterController controller;

    bool ground_Ray;
    bool resetGravity = false;
    public Vector3 gravityVctor;
    public bool jumpAnimation;
    float gravity;

    public Transform pivot;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        resetGravity = false;
        ground_Ray = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Jump();
        RayCast_onto_fun();
        ApplyGravity();

        SetupAnimation();

        if(inputSettings.Horizontal != 0 || inputSettings.Vertical != 0)
        {
            CharacterLook();
        }
    }

    void GetInput()
    {
        inputSettings.Horizontal = Input.GetAxis("Horizontal");
        inputSettings.Vertical = Input.GetAxis("Vertical");
        if(Input.GetButtonDown("Jump"))
        {
            inputSettings.jump = true;
        }

        inputSettings.mouseX = Input.GetAxis("Mouse X");
        inputSettings.mouseY = Input.GetAxis("Mouse Y");

        inputSettings.isGrounded = controller.isGrounded || ground_Ray;
        JumpToFall();
    }

    void JumpToFall()
    {
        if(!inputSettings.isGrounded && inputSettings.jump)
        {
            StartCoroutine(FallTineIntervall());
        }
        if (inputSettings.isGrounded)
        {
            //StopCoroutine(StopJump());
            StopCoroutine(FallTineIntervall());
            inputSettings.groundConformation = true;
           //StopAllCoroutines();
        }
    }

    IEnumerator FallTineIntervall()
    {
        Debug.Log("1");
        yield return new WaitForSeconds(movementSettings.groundConfirmTime);
        Debug.Log("2");
        if (!inputSettings.isGrounded)
            inputSettings.groundConformation = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Grounded")
        {
            ground_Ray = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Grounded")
        {
            ground_Ray = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Grounded")
        {
            ground_Ray = false;
        }
    }

    void SetupAnimation()
    {
        anim.SetFloat(animSettings.forward, inputSettings.Vertical);
        anim.SetFloat(animSettings.strafe, inputSettings.Horizontal);
        anim.SetBool(animSettings.isGrounded, inputSettings.isGrounded);
        anim.SetBool(animSettings.jumptoFall, inputSettings.groundConformation || inputSettings.isGrounded);

        
        if (jumpAnimation)
        {   
            //if(raySettings.onto_distance > 1.3f)
            {
                anim.SetBool(animSettings.Jump_onPlatform, inputSettings.Jump_onPlatform1);
            }
            
        }
        if (jumpAnimation)
        {
            anim.SetBool(animSettings.jump, inputSettings.jump);
        }
    }

    void Jump()
    {
       // if (inputSettings.jump)
        //    return;

        if(inputSettings.Jump_onPlatform1)
        {
            StartCoroutine(StopJump2());
        }
        else if (inputSettings.jump)
        {
            StartCoroutine(StopJump());
        }
    }

    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(movementSettings.jump_time);
        //Debug.Log(" aefgrthsry");
        inputSettings.jump = false;
    }
    IEnumerator StopJump2()
    {
        yield return new WaitForSeconds(movementSettings.jump_time + 2f);
       //Debug.Log(" aefgrthsry");
        inputSettings.jump = false;
    }

    void ApplyGravity()
    {   
        if(!inputSettings.isGrounded)
        {
            if(!resetGravity)
            {
                gravity = physics.resetGravityValue;
                resetGravity = true;
                //Debug.Log("1");
            }
                gravity += physics.gravityModifier;
            //Debug.Log("2");
        }
        else
        {
            gravity = physics.baseGravity;
            //Debug.Log("3");
            resetGravity = false;
        }

        if (inputSettings.Jump_onPlatform1_1)
        {
            gravityVctor.y = 0f;
            SnapCharacter();
        }
        else if(!inputSettings.jump && !inputSettings.Jump_onPlatform1_1)
        {
            gravityVctor.y = -gravity;
        }
        else if(inputSettings.jump && !inputSettings.Jump_onPlatform1_1)
        {
            gravityVctor.y = movementSettings.jumpSpeed;
        }

        controller.Move(gravityVctor * Time.deltaTime);

    }


    void RayCast_onto_fun()
    {
        Ray ray = new Ray(raySettings.jumpOnto_Obj.transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, raySettings.dis1, raySettings.rayMask))
        {
            raySettings.jumpOnto = true;
            raySettings.onto_distance = (hit.point - transform.position).magnitude;
            //StartCoroutine(SetOnToFalse());
        }
        else
        {
            raySettings.jumpOnto = false;
        }

        Ray ray2 = new Ray(raySettings.jumpOnto_Top_Obj.transform.position, transform.forward);
        RaycastHit hit2;

        if (Physics.Raycast(ray2, out hit2, raySettings.dis1_1, raySettings.rayMask))
        {
            raySettings.jumpOnto_topObj = true;
            //StartCoroutine(SetOnToFalse());
        }
        else
        {
            raySettings.jumpOnto_topObj = false;
        }


        Debug.DrawRay(raySettings.jumpOnto_Obj.transform.position, transform.forward * raySettings.dis1, Color.green);
        Debug.DrawRay(raySettings.jumpOnto_Top_Obj.transform.position, transform.forward * raySettings.dis1, Color.green);

        if (raySettings.jumpOnto && inputSettings.jump && !raySettings.jumpOnto_topObj)
        {
            inputSettings.Jump_onPlatform1 = true;
            inputSettings.Jump_onPlatform1_1 = true;
            StartCoroutine(SetOnToFalse());
            StartCoroutine(SetOnToFalse2());
        }
        
    }

    IEnumerator SetOnToFalse()
    {
        yield return new WaitForSeconds(raySettings.time1);
        inputSettings.Jump_onPlatform1 = false;

        //SnapCharacter();
    }

    IEnumerator SetOnToFalse2()
    {
        yield return new WaitForSeconds(raySettings.time1_1);
        inputSettings.Jump_onPlatform1_1 = false;

        //SnapCharacter();
    }

    void SnapCharacter()
    {
        controller.Move(Vector3.up * raySettings.upSpeed * Time.deltaTime);
    }

    void CharacterLook()
    {
        if(!pivot)
        {
            pivot = Camera.main.transform.parent;
        }

        Vector3 lookPos = pivot.position + (pivot.forward * movementSettings.lookDistance);
        Vector3 dir = lookPos - transform.position;

        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * movementSettings.lookSpeed);
    }

}