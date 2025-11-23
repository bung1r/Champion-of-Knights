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

    void Awake() {
        if (primaryAbility is MeleeAbility)
        {
            RTPrimaryAbility = primaryAbility.CreateRuntimeInstance(primaryAbility, statManager);
        }
        
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
        RTPrimaryAbility.Use();
    }

    void HandlePrimaryUp()
    {
        
    }

}
