using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerMovement), typeof(InputHandler))]
public class PlayerCombat : MonoBehaviour
{  
    public StatManager statManager;
    public InputHandler input;
    // below 3 are the base skills, assigned in inspector
    public AbilityBase primaryAbility;
    public AbilityBase secondaryAbility;
    public AbilityBase[] usableAbilities;
    // below are the 3 runtime skills, can and should be edited
    private AbilityRuntime RTPrimaryAbility;
    private AbilityRuntime RTSecondaryAbility;
    private AbilityRuntime[] RTUsableAbilities;

    private bool primaryDown = false;

    void Awake() {
        RTPrimaryAbility = primaryAbility.CreateRuntimeInstance(primaryAbility, statManager);     
    }
    
    void OnEnable()
    {
        input.OnPrimaryDown += HandlePrimaryDown;
        input.OnPrimaryUp += HandlePrimaryUp;
    }

    void OnDisable()
    {
        input.OnPrimaryDown -= HandlePrimaryDown;
        input.OnPrimaryUp -= HandlePrimaryUp;
    }

    void HandlePrimaryDown()
    {
        // wow so neat and nice!!! i'm so cool :D
        primaryDown = true;
        RTPrimaryAbility.Use();
        RTPrimaryAbility.BeginUse();
    }

    void HandlePrimaryUp()
    {
        primaryDown = false;
        RTPrimaryAbility.EndUse();
    }

    void Update()
    {
        if (primaryDown) {
            RTPrimaryAbility.WhileUse();
        }
    }
}
