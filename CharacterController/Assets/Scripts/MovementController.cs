using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[AddComponentMenu("movement/movmentController")]
public class MovementController : RuleSystem
{
 
    [Header("Player Settings")]
    [Tooltip("Player speed, Can be in decimal number.")]
    public float playerSpeed = 5;
    [Tooltip("Player speed while running, Can be in decimal number.")]
    public float playerRunningSpeed = 10;
    [Tooltip("The rotation speed that applies to the player.")]
    public float playerRotateSpeed = 5;
    [Tooltip("The jumping height that applies to the player.")]
    public float playerJumpHeight = 5;
    public float playerSwimmingSpeed = 4;
    public float playerSwimmingRotationSpeed = 2;

    [Header("World Settings")]
    [Tooltip("The gravity in the playing world.")]
    public float gravityForce = -9.81f;

    public Transform orginPoint;
    [Range(0.5f, 15f)] public float pickupDistance = 1f;
    [Range(0.5f, 1f)] public float Sensitivity = 0.8f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 playerMoveDirection;
    private float movementX;
    private float movementY;
    private float rotationY;
    private float rotationX;
    private bool sprintPressed;
    private bool jumpPressed;
    private Vector3 swimingPosition;
    private RaycastHit rcHit;
    private Ray ray;
    private Rect rect1;
    private Rect rect2;
    private Rect rect3;
    private Texture fiets;
    private Texture sandaal;
    private Texture bal;
    private string winMessage;
    private bool winCondition;


    [HideInInspector] public bool isClimbing = false;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool isWalking = false;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isSwimming = false;
    [HideInInspector] public bool isFalling = false;

    private Vector3 playerVelocity;
    private bool slipper;
    private bool ball;
    private bool bicycle;
    private const float DISTTOGROUND = 0.1f;
    private const float DISTTOWATER = 1.1f;


    public MovementController()
    {
        CreateRules();
    }

    void CreateRules()
    {
        ruleList.Add(new Rule(MovementCondition, Walking(), 1, () => changeMovementFlag(MovementAction.WALKNG, true), () => changeMovementFlag(MovementAction.WALKNG, false), "walking rule"));
        ruleList.Add(new Rule(SprintCondition, Sprinting(), 2, () => changeMovementFlag(MovementAction.SPRINTING, true), () => changeMovementFlag(MovementAction.SPRINTING, false), "sprinting rule"));
        ruleList.Add(new Rule(JumpCondition, null, 3, () => JumpStart(), () => changeMovementFlag(MovementAction.JUMPING, false), "jumping rule"));
        ruleList.Add(new Rule(FallingCondition, Falling(), 3, () => changeMovementFlag(MovementAction.FALLING, true), () => changeMovementFlag(MovementAction.FALLING, false), "falling rule"));
       // ruleList.Add(new Rule(SwimingCondition, Swimming(), 1, () => changeMovementFlag(MovementAction.SWIMMING, true), () => changeMovementFlag(MovementAction.SWIMMING, false), "swiming rule"));
       // ruleList.Add(new Rule(ClimbingCondition, Climbing(), 1, () => changeMovementFlag(MovementAction.CLIMBING, true), () => changeMovementFlag(MovementAction.CLIMBING, false), "climbing rule"));
    }

    void Start()
    {
        var offset = 128;
        float size = Screen.width * 0.05f;
        rect1 = new Rect(Screen.width / 2 - offset - size / 2, Screen.height * 0.7f, size, size);
        fiets = Resources.Load("Icons/Fiets") as Texture;
        
        rect2 = new Rect(Screen.width / 2 - size / 2, Screen.height * 0.7f, size, size);
        bal = Resources.Load("Icons/Ball") as Texture;
        
        rect3 = new Rect(Screen.width / 2 + offset - size / 2, Screen.height * 0.7f, size, size);
        sandaal = Resources.Load("Icons/Sandal") as Texture;

        if (orginPoint == null)
        {
            orginPoint = transform;
        }

        
        winMessage = String.Format("You finished the game!");

        CheckPlayerInput();
        GetController();
        GetAnimator();
        StartCoroutine(RuleSystemCoroutine());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void GetController()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            gameObject.AddComponent<CharacterController>();
            controller = GetComponent<CharacterController>();
        }
    }

    void GetAnimator()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            gameObject.AddComponent<Animator>();
            animator = GetComponent<Animator>();
            RuntimeAnimatorController newController = (RuntimeAnimatorController)Resources.Load("EmptyMovmentAnimationsController");
            animator.runtimeAnimatorController = newController;
            animator.applyRootMotion = false;
        }
    }

    void CheckPlayerInput()
    {
        if (GetComponent<PlayerInput>() == null)
        {
            gameObject.AddComponent<PlayerInput>();
            InputActionAsset actions = (InputActionAsset)Resources.Load("PlayerInputRecource");
            GetComponent<PlayerInput>().actions = actions;
            GetComponent<PlayerInput>().enabled = false;
            this.Invoke("DelayedInput", 1f);
        }
    }

    void DelayedInput()
    {
        GetComponent<PlayerInput>().enabled = true;
    }



    void changeMovementFlag(MovementAction action, bool newValue)
    {
        switch (action)
        {
            case MovementAction.CLIMBING:
                isClimbing = newValue;
                animator.SetBool("isClimbing", newValue);
                break;
            case MovementAction.JUMPING:
                isJumping = newValue;
                animator.SetBool("isJumping", newValue);
                break;
            case MovementAction.SPRINTING:
                isSprinting = newValue;
                animator.SetBool("isSprinting", newValue);
                break;
            case MovementAction.SWIMMING:
                animator.SetBool("isSwimming", newValue);
                isSwimming = newValue;
                break;
            case MovementAction.WALKNG:
                isWalking = newValue;
                animator.SetBool("isWalking", newValue);
                break;
            case MovementAction.FALLING:
                isFalling = newValue;
                animator.SetBool("isFalling", newValue);
                break;
            default:
                break;
        }
    }

    //------------Movemenets--------------

    public IEnumerator Walking()
    {
        while (true)
        {
            playerMoveDirection = new Vector3(movementX, 0, movementY);
            playerMoveDirection = orginPoint.TransformDirection(playerMoveDirection);
            controller.Move(playerMoveDirection * (playerSpeed * Time.deltaTime));
            transform.Rotate(new Vector3(0, rotationY * (Time.deltaTime * playerRotateSpeed) * Sensitivity, 0));
            Camera.main.transform.Rotate(new Vector3(rotationX * (Time.deltaTime * playerRotateSpeed) * Sensitivity,0 , 0));
            yield return null;
        }
    }

    public IEnumerator Sprinting()
    {
        while (true)
        {
            playerMoveDirection = new Vector3(movementX, 0, movementY);
            playerMoveDirection = orginPoint.TransformDirection(playerMoveDirection);
            controller.Move(playerMoveDirection * (playerRunningSpeed * Time.deltaTime));
            transform.Rotate(new Vector3(0, rotationY * (Time.deltaTime * playerRotateSpeed) * Sensitivity, 0));
            Camera.main.transform.Rotate(new Vector3(rotationX * (Time.deltaTime * playerRotateSpeed) * Sensitivity,0 , 0));
            yield return null;
        }
    }

    public IEnumerator Swimming()
    {
        while (true)
        {
            playerMoveDirection = new Vector3(movementX, 0, movementY);
            playerMoveDirection = orginPoint.TransformDirection(playerMoveDirection);
            controller.Move(playerMoveDirection * (playerSwimmingSpeed * Time.deltaTime));
            transform.position = new Vector3(transform.position.x, swimingPosition.y, transform.position.z);
            transform.Rotate(new Vector3(0, rotationY * (Time.deltaTime * playerSwimmingRotationSpeed), 0));
            yield return null;
        }
    }

    public IEnumerator Falling()
    {
        while (true)
        {
            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            playerVelocity.y += gravityForce * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator Climbing()
    {
        while (true)
        {
            playerVelocity.y = 0;
            playerMoveDirection = new Vector3(movementX, movementY, movementY);
            playerMoveDirection = orginPoint.TransformDirection(playerMoveDirection);
            controller.Move(playerMoveDirection * (playerSpeed * Time.deltaTime));
            transform.Rotate(new Vector3(0, rotationY * (Time.deltaTime * playerRotateSpeed), 0));
            yield return null;
        }
    }

    //------------Input--------------

    private void OnMove(InputValue movementValue)
    {
        Vector2 movement = movementValue.Get<Vector2>();

        movementX = movement.x;
        movementY = movement.y;
    }

    private void OnLook(InputValue inputValue)
    {
        Vector2 lookVector = inputValue.Get<Vector2>();
        rotationY = lookVector.x;
        rotationX = -lookVector.y;
    }

    private void OnJump(InputValue inputValue)
    {
        jumpPressed = inputValue.Get<float>() > 0;
    }

    private void OnSprint(InputValue inputValue)
    {
        sprintPressed = inputValue.Get<float>() > 0;
    }

    private void OnFire(InputValue inputValue)
    {
        bool clicking = inputValue.Get<float>() > 0;
        if (clicking)
        {
            ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
            if (Physics.Raycast(ray, out rcHit, pickupDistance))
            {
                if (rcHit.transform.CompareTag("Collectible"))
                {
                    var child = rcHit.transform.GetChild(0);
                    child.gameObject.SetActive(true);
                    MovementModifier movementModifier =
                        child.gameObject.GetComponent<MovementModifier>();
                    if (movementModifier != null)
                    {
                        foreach (MovementMod mod in movementModifier.modifiers)
                        {
                            if (mod == MovementMod.FIETS) bicycle = true;
                            else if (mod == MovementMod.BAL) ball = true;
                            else if (mod == MovementMod.SLIPPER) slipper = true;
                        }
                    }
                    Destroy(child.gameObject, 1f);
                } 
                if (rcHit.transform.CompareTag("Door") && bicycle && ball && slipper)
                {
                    winCondition = true;
                    gameObject.GetComponent<PlayerInput>().enabled = false;
                    GameObject.Find("Enemy").SetActive(false);
                }
            }
        }
    }

    private void OnGUI()
    {
        if (slipper)
        {
            GUI.DrawTexture(rect1, sandaal);
        }
    
        if (ball)
        {
            GUI.DrawTexture(rect2, bal);
        }
    
        if (bicycle)
        {
            GUI.DrawTexture(rect3, fiets);
        }

        if (winCondition)
        {
            GUI.Label(new Rect(325, 150, 300, 30), winMessage, "box");
        }
    }


    //------------Conditions--------------

    public bool MovementCondition()
    {
        return (movementX != 0 || movementY != 0 || rotationY != 0) && !isSprinting && !isSwimming && !isClimbing;
    }
    

    public bool SprintCondition()
    {
        return (movementX != 0 || movementY != 0 || rotationY != 0) && sprintPressed && !isSwimming && !isClimbing;
    }

    public bool JumpCondition()
    {
        return jumpPressed && IsGrounded();
    }

    public bool SwimingCondition()
    {
        return Swimable() && !isClimbing;
    }

    public bool FallingCondition()
    {
        return !isSwimming && !isClimbing;
    }

    public bool ClimbingCondition()
    {
        return Climbable();
    }

    //------------extra functions--------------

    private void JumpStart()
    {
        jumpPressed = false;
        playerVelocity.y += Mathf.Sqrt(playerJumpHeight * -3.0f * gravityForce);
        changeMovementFlag(MovementAction.JUMPING, true);
    }

    private bool IsGrounded()
    {
        if ((controller.collisionFlags & CollisionFlags.Below) != 0)
        {
            return true;
        }
        return Physics.Raycast(transform.position, -Vector3.up, DISTTOGROUND);

    }

    private bool Swimable()
    {
        RaycastHit hitUp;
        RaycastHit hitDown;

        bool IsUp = Physics.Raycast(transform.position + (transform.up * 0.1f), -Vector3.up, out hitUp, DISTTOWATER);
        bool IsDown = Physics.Raycast(transform.position + (transform.up * 0.1f), -Vector3.down, out hitDown, DISTTOWATER);

        if (IsUp)
        {
            MovementModifier movementModifier = hitUp.transform.gameObject.GetComponent<MovementModifier>();
            if (movementModifier != null)
            {
                foreach (MovementMod mod in movementModifier.modifiers)
                {
                    if (mod == MovementMod.SWIMMABLE)
                    {
                        swimingPosition = hitUp.point;
                        return true;
                    }
                }
            }
        }

        if (IsDown)
        {
            MovementModifier movementModifier = hitDown.transform.gameObject.GetComponent<MovementModifier>();
            if (movementModifier != null)
            {
                foreach (MovementMod mod in movementModifier.modifiers)
                {
                    if (mod == MovementMod.SWIMMABLE)
                    {
                        swimingPosition = hitDown.point;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool Climbable()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (transform.up * 0.1f), transform.forward, out hit, 1))
        {
            MovementModifier movementModifier = hit.transform.gameObject.GetComponent<MovementModifier>();
            if (movementModifier != null)
            {
                foreach (MovementMod mod in movementModifier.modifiers)
                {
                    if (mod == MovementMod.NONCLIMABLE)
                    {
                        return false;
                    }
                }
            }
        }
        if ((controller.collisionFlags & CollisionFlags.Sides) != 0)
        {
            return true;
        }

        Vector3 hillAngle = hit.normal * (180 / Mathf.PI);
        if (Mathf.Abs(hillAngle.x) >= controller.slopeLimit - 5 || Mathf.Abs(hillAngle.z) >= controller.slopeLimit - 5 || Mathf.Abs(hillAngle.y) >= controller.slopeLimit)
        {
            return true;
        }
        return false;



    }



}
