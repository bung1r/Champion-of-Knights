using System;
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
    public List<SkillSlotAndAbility> otherUsableAbilities;
    // below are the 3 runtime skills, can and should be edited
    private AbilityRuntime RTPrimaryAbility;
    private AbilityRuntime RTSecondaryAbility;
    private Dictionary<int, AbilityRuntime> RTUsableAbilities = new Dictionary<int, AbilityRuntime>();

    // all the 'holding' variables
    private bool primaryDown = false;
    private bool secondaryDown = false;
    private bool slot1Down = false;
    private bool slot2Down = false;
    private bool slot3Down = false;

    void Awake() {
        RTPrimaryAbility = primaryAbility.CreateRuntimeInstance(primaryAbility, statManager);   
        RTSecondaryAbility = secondaryAbility.CreateRuntimeInstance(secondaryAbility, statManager);  
        foreach (SkillSlotAndAbility skillSlot in otherUsableAbilities)
        {
            RTUsableAbilities.Add(skillSlot.skillSlot, skillSlot.abilityBase.CreateRuntimeInstance(skillSlot.abilityBase, statManager));
        }
    }
    void OnEnable()
    {
        input.OnPrimaryDown += HandlePrimaryDown;
        input.OnPrimaryUp += HandlePrimaryUp;

        input.OnSecondaryDown += HandleSecondaryDown;
        input.OnSecondaryUp += HandleSecondaryUp;

        input.OnAbilitySlot1Down += HandleSlot1Down;
        input.OnAbilitySlot1Up += HandleSlot1Up;

        input.OnAbilitySlot2Down += HandleSlot2Down;
        input.OnAbilitySlot2Up += HandleSlot2Up;

        input.OnAbilitySlot3Down += HandleSlot3Down;
        input.OnAbilitySlot3Up += HandleSlot3Up;
    }
    void OnDisable()
    {
        input.OnPrimaryDown -= HandlePrimaryDown;
        input.OnPrimaryUp -= HandlePrimaryUp;

        input.OnSecondaryDown -= HandleSecondaryDown;
        input.OnSecondaryUp -= HandleSecondaryUp;
        
        input.OnAbilitySlot1Down -= HandleSlot1Down;
        input.OnAbilitySlot1Up -= HandleSlot1Up;

        input.OnAbilitySlot2Down -= HandleSlot2Down;
        input.OnAbilitySlot2Up -= HandleSlot2Up;
        
        input.OnAbilitySlot3Down -= HandleSlot3Down;
        input.OnAbilitySlot3Up -= HandleSlot3Up;
    }
    // evil handler methods
    // there's a lot, but it's not perfomance heavy!
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
    void HandleSecondaryDown()
    {
        secondaryDown = true;
        RTSecondaryAbility.Use();
        RTSecondaryAbility.BeginUse();
    }
    void HandleSecondaryUp()
    {
        secondaryDown = false;
        RTSecondaryAbility.EndUse();
    }
    void HandleSlot1Down()
    {
        slot1Down = true;
        if (RTUsableAbilities.ContainsKey(1))
        {
            RTUsableAbilities[1].Use();
            RTUsableAbilities[1].BeginUse();
        }
    }
    void HandleSlot1Up()
    {
        slot1Down = false;
        if (RTUsableAbilities.ContainsKey(1))
        {
            RTUsableAbilities[1].EndUse();
        }
    }
    void HandleSlot2Down()
    {
        slot2Down = true;
        if (RTUsableAbilities.ContainsKey(2))
        {
            RTUsableAbilities[2].Use();
            RTUsableAbilities[2].BeginUse();
        }
    }
    void HandleSlot2Up()
    {
        slot2Down = false;
        if (RTUsableAbilities.ContainsKey(2))
        {
            RTUsableAbilities[2].EndUse();
        }
    }
    void HandleSlot3Down()
    {
        slot3Down = true;
        if (RTUsableAbilities.ContainsKey(3))
        {
            RTUsableAbilities[3].Use();
            RTUsableAbilities[3].BeginUse();
        }
    }
    void HandleSlot3Up()
    {
        slot3Down = false;
        if (RTUsableAbilities.ContainsKey(3))
        {
            RTUsableAbilities[3].EndUse();
        }
    }
    void Update()
    {
        if (primaryDown) {
            RTPrimaryAbility.WhileUse();
        }
        if (secondaryDown)
        {
            RTSecondaryAbility.WhileUse();
        }
        if (slot1Down)
        {
            if (RTUsableAbilities.ContainsKey(1)) RTUsableAbilities[1].WhileUse();
        }
        if (slot2Down)
        {
            if (RTUsableAbilities.ContainsKey(2)) RTUsableAbilities[2].WhileUse();
        }
        if (slot3Down)
        {
            if (RTUsableAbilities.ContainsKey(3)) RTUsableAbilities[3].WhileUse();
        }
    }
}

[Serializable]
public class SkillSlotAndAbility
{
    public int skillSlot;
    public AbilityBase abilityBase;
}