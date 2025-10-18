using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class SlidingDoor : InteractableComponent
{

    public float constraintStrength = 10f;
    public Vector3 moveDir = Vector3.forward;
    public float negConstraintDist = -2f;
    public float posConstraintDist = -5f;
    public Gear drivenGear;
    private Vector3 beginPos;
    // Start is called before the first frame update
    private Vector3 negConstraintPos;
    private Vector3 posConstraintPos;
    void Start()
    {
        moveDir = moveDir.normalized;


        negConstraintPos = new Vector3(
            transform.localPosition.x - moveDir.x * negConstraintDist,
            transform.localPosition.y - moveDir.y * negConstraintDist,
            transform.localPosition.z - moveDir.z * negConstraintDist
        );
        posConstraintPos = new Vector3(
            transform.localPosition.x + moveDir.x * posConstraintDist,
            transform.localPosition.y + moveDir.y * posConstraintDist,
            transform.localPosition.z + moveDir.z * posConstraintDist
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (drivenGear == null) return;
        if (drivenGear.rotationAmount == 0) return;
    }

    public override void MoveComponent(float distance)
    {
        //Calculates the predicted pos and the clamped pos
        //So it doesn't go beyond its constraints.
        Vector3 newPos = transform.localPosition + moveDir.normalized * distance;
        Vector3 clampedPos = new Vector3(
            Mathf.Clamp(newPos.x, negConstraintPos.x, posConstraintPos.x),
            Mathf.Clamp(newPos.y, negConstraintPos.y, posConstraintPos.y),
            Mathf.Clamp(newPos.z, negConstraintPos.z, posConstraintPos.z)
        );

        bool clampedX = clampedPos.x != newPos.x;
        bool clampedY = clampedPos.y != newPos.y;
        bool clampedZ = clampedPos.z != newPos.z;

        if (clampedX || clampedY || clampedZ) constraintReached = true;
        else constraintReached = false;

        transform.position = clampedPos;
        
    }
}
