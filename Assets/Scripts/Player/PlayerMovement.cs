using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{

    
    private Camera cam;
    private Rigidbody rb;
    private Vector3 inputDir;

    [Header("Movement Stats")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 25f;
    public float jumpVelocity = 5f;
    public float jumpCooldown = 2f;
    public float checkForGroundDistance = 0.2f;
    public Transform feetTransform;
    private bool jumpPressed = false;
    private bool isGrounded = true;
    private float timeSinceLastJump = 2f;

    [Header("Camera Distances From Player")]
    public float camUpDistance = 10f;
    public float camBackDistance = 2f;
    private float camScrollValue = 1f;
    public bool useScrollValue = true;
    private float camScrollScale = 0.25f;
    public bool reverseCameraScrollDirection = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World! Although it sounds like bits and bytes! My circuitry is filled with mites...");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = 0; // remove slowdown drag
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputDir * moveSpeed * Time.fixedDeltaTime);
        FaceMouse();
        CamFollowPlayer();
        TryJump();
    }

    void FaceMouse()
    {
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
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    void TryJump()
    {
        timeSinceLastJump += Time.fixedDeltaTime;
        if (timeSinceLastJump < jumpCooldown) return;
        if (jumpPressed)
        {
            // do the jump logic here!
            Debug.Log("Congratulations on the jump!");
            timeSinceLastJump = 0f;
        }
    }
    void CamFollowPlayer()
    {
        if (useScrollValue)
        {
            int a = 1;
            if (reverseCameraScrollDirection) a = -1;
            float scrollDelta = Input.mouseScrollDelta.y * a;
            camScrollValue += scrollDelta * camScrollScale;
            Math.Clamp(camScrollValue, 0.5f, 1.5f);
      
            cam.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + camUpDistance * camScrollValue, rb.transform.position.z - camBackDistance * camScrollValue);
        } else
        {
            cam.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + camUpDistance, rb.transform.position.z - camBackDistance);
        }
    }
}
