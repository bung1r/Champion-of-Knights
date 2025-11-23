using UnityEngine;
using System;
public class InputHandler : MonoBehaviour
{
    public event Action OnPrimaryDown;
    public event Action OnPrimaryUp;
    public event Action<float> OnPrimaryHold; 

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
    }
}