using UnityEngine;

public class Orb : MonoBehaviour, IInteractable
{
    public string customName = "Orb";
    public void Interact(GameObject interactor)
    {
        if (interactor.GetComponent<PlayerStatManager>() != null)
        {
            RoundManager.Instance.CollectOrb();
            Destroy(gameObject);
        }
    }
}