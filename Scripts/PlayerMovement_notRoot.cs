using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement_notRoot : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSettings
    {
        public string forward = "forward";
        public string strafe = "strafe";
        public string jump = "Jump";
        public string isGrounded = "isGrounded";
        public string index = "BlendTreeIndex";
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
    }
    [SerializeField]
    public InputSettings inputSettings;

    [System.Serializable]
    public class MovementSettings
    {
        public float moveSpeed = 2f;
        public float jump_time = 0.19f;
        public float jumpSpeed;
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
    }
    [SerializeField]
    public PhysicsSettings physics;

    Animator anim;
    CharacterController controller;

    bool resetGravity = false;
    public Vector3 gravityVctor;
    Vector3 moveVector;
    float gravity;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        resetGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        SetupAnimation();

        Jump();
        ApplyGravity();
        Move();
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

        inputSettings.isGrounded = controller.isGrounded;
    }

    void Move()
    {
        moveVector = new Vector3(inputSettings.Horizontal, 0f, inputSettings.Vertical);
        controller.Move(moveVector * movementSettings.moveSpeed * Time.deltaTime);
    }

    void SetupAnimation()
    {
        anim.SetFloat(animSettings.forward, inputSettings.Vertical);
        anim.SetFloat(animSettings.strafe, inputSettings.Horizontal);
      //  anim.SetBool(animSettings.jump, inputSettings.jump);
    }

    void Jump()
    {
       // if (inputSettings.jump)
        //    return;

        if (inputSettings.jump)
        {
            StartCoroutine(StopJump());
        }
    }

    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(movementSettings.jump_time);
        Debug.Log(" aefgrthsry");
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
            }
                gravity += physics.gravityModifier;
        }
        else
        {
            gravity = physics.baseGravity;
            resetGravity = false;
        }

        if(!inputSettings.jump)
        {
            gravityVctor.y = -gravity;
            //controller.center = new Vector3(0, 1f, 0);
        }
        else
        {
            gravityVctor.y = movementSettings.jumpSpeed;
            //controller.center = new Vector3(0, 1.75f, 0);
        }

        controller.Move(gravityVctor * Time.deltaTime);

    }

}
