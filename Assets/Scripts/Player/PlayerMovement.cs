using System;
using UnityEngine;
// search, uielemetns, 

public class PlayerMovement : MonoBehaviour
{
    public PlayerStatManager statManager;
    private PlayerCombat playerCombat;
    private PlayerStats stats;
    private Camera cam;
    private Rigidbody rb;
    private Vector3 inputDir;
    private Animator animator;

    [Header("Movement Stats")]
    public float jumpVelocity = 5f;
    public float jumpCooldown = 2.0f;
    public float groundRadius = 0.2f;
    public Transform feetTransform;
    public LayerMask groundLayer;
    private bool jumpPressed = false;
    private bool holdShift = false;
    private bool isGrounded = true;
    private float timeSinceLastJump = 2f;
    private float speed = 0f;

    [Header("Camera Distances From Player")]
    public float camUpDistance = 10f;
    public float camBackDistance = 0f;
    private float camScrollValue = 1f;
    public bool useScrollValue = true;
    private float camScrollScale = 0.25f;
    public bool reverseCameraScrollDirection = false;
    [Header("Important Stats")]
    public float currentEnergy = 100f;
    public float strength = 20f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World! Although it sounds like bits and bytes! My circuitry is filled with mites...");
        rb = GetComponent<Rigidbody>();
        playerCombat = GetComponent<PlayerCombat>();
        statManager = GetComponent<PlayerStatManager>();
        animator = GetComponent<Animator>();
        rb.drag = 0; // remove slowdown drag
        cam = Camera.main;
        stats = statManager.stats;
    }

    // Update is called once per frame
    void Update()
    {
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        holdShift = Input.GetKey(KeyCode.LeftShift);
        isGrounded = Physics.CheckSphere(feetTransform.position, groundRadius, groundLayer);
        stats = statManager.stats;
        TryJump();
        TryInteract();
        Debug.Log(rb.velocity);
    }

    void FixedUpdate()
    {
        Move();
        FaceMouse();
        CamFollowPlayer();
    }

    void FaceMouse()
    {
        if (statManager.stats.inAttackAnim) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    stats.turnSpeed * Time.deltaTime
                );
            }
        }
    }
    void TryJump()
    {
        timeSinceLastJump += Time.deltaTime;
        // Debug.Log($"JUMP LOGIC!!! {timeSinceLastJump}, {isGrounded}, {jumpPressed}");
        if (!isGrounded) return;
        if (timeSinceLastJump < jumpCooldown) return;
        if (jumpPressed)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // reset Y
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
            timeSinceLastJump = 0f;
        }
    }
    void TryInteract()
    {
        
    }
    private float CalcSpeed(float speed)
    {
        float divider = 0;
       if (stats.isGuarding && playerCombat.secondaryAbility is GuardAbility guard)
       {
          divider += guard.guardStats.speedReduction;
       }


        if (divider <= 0) divider = 1f;
    
        return speed/divider;
    }
    void Move()
    {
        // don't do anything if there isn't any movement
        if ((inputDir.x == 0 && inputDir.y == 0 && inputDir.z == 0) || stats.inAttackAnim) {stats.isWalking = false; stats.isRunning = false; return;}
        // the sprint logic 
        if (holdShift && statManager.CanUseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime) && !stats.isGuarding && !stats.inAttackAnim)
        {
            statManager.UseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime);
            speed = CalcSpeed(stats.sprintSpeed);
            stats.isRunning = true;
            stats.isWalking = false;
        } else
        {
            speed = CalcSpeed(stats.walkSpeed);
            stats.isWalking = true;
            stats.isRunning = false;
        }
        Vector3 moveXZ = new Vector3(inputDir.x, 0f, inputDir.z);
        rb.MovePosition(rb.position + moveXZ * speed * Time.fixedDeltaTime);
        
    }
    
    void CamFollowPlayer()
    {
        if (useScrollValue)
        {
            int a = 1;
            if (reverseCameraScrollDirection) a = -1;
            float scrollDelta = Input.mouseScrollDelta.y * a;
            camScrollValue += scrollDelta * camScrollScale;
            camScrollValue = Math.Clamp(camScrollValue, 1f, 1.5f);

            cam.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + camUpDistance * camScrollValue, rb.transform.position.z - camBackDistance * camScrollValue);
        }
        else
        {
            cam.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + camUpDistance, rb.transform.position.z - camBackDistance);
        }
    }

   
}
