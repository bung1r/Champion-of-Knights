using UnityEngine;
using System;
public class InputHandler : MonoBehaviour
{
    public event Action OnPrimaryDown;
    public event Action OnPrimaryUp;
    public event Action<float> OnPrimaryHold; 
    public event Action OnSecondaryDown;
    public event Action OnSecondaryUp;
    public event Action OnAbilitySlot1Down;
    public event Action OnAbilitySlot2Down;
    public event Action OnAbilitySlot3Down;
    public event Action OnAbilitySlot1Up;
    public event Action OnAbilitySlot2Up;
    public event Action OnAbilitySlot3Up;
    private bool holding;
    private float holdStart;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holding = true;
            holdStart = Time.time;
            OnPrimaryDown?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            holding = false;
            OnPrimaryUp?.Invoke();
        }
        if (holding)
        {
            OnPrimaryHold?.Invoke(Time.time - holdStart);
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnSecondaryDown?.Invoke();
        } 
        if (Input.GetMouseButtonUp(1))
        {
            OnSecondaryUp?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnAbilitySlot1Down?.Invoke();
        } 
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            OnAbilitySlot1Up?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnAbilitySlot2Down?.Invoke();
        } 
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            OnAbilitySlot2Up?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnAbilitySlot3Down?.Invoke();
        } 
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            OnAbilitySlot3Up?.Invoke();
        }
    }
}