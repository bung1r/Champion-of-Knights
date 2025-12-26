using UnityEngine;

public class Footstep : MonoBehaviour
{
    public enum FootstepType
    {
        Grass,
        Concrete,
        Echo,
        Robotic
    }

    public FootstepType footstepType = FootstepType.Grass;

    public void PlayFootstep()
    {
        switch (footstepType)
        {
            case FootstepType.Grass:
                AudioManager.Instance.PlayFootstepGrassSFX(transform);
                break;
            case FootstepType.Concrete:
                AudioManager.Instance.PlayFootstepConcreteSFX(transform);
                break;
            case FootstepType.Echo:
                AudioManager.Instance.PlayFootstepEchoSFX(transform);
                break;
        }
    }
}