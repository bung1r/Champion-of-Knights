using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLever : InteractableComponent
{
    [Tooltip("0-1, 0 is on, 1 is off. In between is 0.")]
    public float leverPos = 0f;
    public Transform leverPart;
    public Vector3 leverPartPosAt0;
    public Vector3 leverPartPosAt1;
    public float power = 60f;
    public float strengthToPush = 10f;
    public float energyToPush = 20f;
    public float energyLeftToPush = 20f;
    public float timeToPush = 2f;
    private float savedLeverPos = 0f;
    // Update is called once per frame
    void Update()
    {
        if (savedLeverPos == 1)
        {
            foreach (InteractableComponent c in connectedComponents)
            {
                c.MoveComponent(savedLeverPos);
            }
        }
    }

    public override void MoveComponent(float distance, GameObject owner)
    {
        // Debug.Log("Attempting to interact");
        if (owner.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            // checks if the player has enough strength and energy to push
            if (playerMovement.strength < strengthToPush && playerMovement.currentEnergy > 0f) DisengageComponent(owner);
            playerMovement.currentEnergy -= timeToPush * Time.fixedDeltaTime;
            energyLeftToPush -= timeToPush * Time.fixedDeltaTime;
            //Depending on the pos, 
            if (savedLeverPos == 0) leverPos = Mathf.Clamp(1 - energyLeftToPush / energyToPush, 0f, 1f);
            if (savedLeverPos == 1) leverPos = Mathf.Clamp(energyLeftToPush / energyToPush, 0f, 1f);
 
            leverPart.localPosition = Vector3.Lerp(leverPartPosAt0, leverPartPosAt1, leverPos);

            if (leverPos == 0 || leverPos == 1) DisengageComponent(owner);
        }
        // throw new System.NotImplementedException();
    }

    public override void DisengageComponent(GameObject owner)
    {
        if (savedLeverPos == 0 && leverPos == 1 || savedLeverPos == 1 && leverPos == 0)
        {
            savedLeverPos = leverPos;
            Debug.Log("The lever has been pulled all the way");
        }
        else
        {
            leverPos = savedLeverPos;
            leverPart.localPosition = Vector3.Lerp(leverPartPosAt0, leverPartPosAt1, leverPos);
        }
        energyLeftToPush = 20f;
        if (canInteract) StartCoroutine(InteractDelay());
        // throw new System.NotImplementedException();
    }
    
    IEnumerator InteractDelay()
    {
        Debug.Log("Mm the delay, i'm delaying so good.");
        canInteract = false;
        yield return new WaitForSeconds(2f);
        canInteract = true;
    }
}
