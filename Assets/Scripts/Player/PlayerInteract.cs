using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerStatManager statManager;
    private StatsUIManager statsUIManager;
    private InputHandler inputHandler;
    private InventoryManager inventoryManager;
    private PickUpItem nearbyPickup;
    private IInteractable nearbyInteractable;
    private float lastCheckedForPickups = -999;
    public float checkPickupInterval = 0.2f;
    
    void Awake()
    {
        statManager = GetComponent<PlayerStatManager>();
        statsUIManager = statManager.GetStatsUIManager();
        inputHandler = GetComponent<InputHandler>();
        inventoryManager = GetComponent<InventoryManager>();
    }

    void OnEnable()
    {
        inputHandler.OnInteractDown += HandleInteractDown;
        inputHandler.OnInteractUp += HandleInteractUp;
        inputHandler.OnEscPressed += EscMenuHandler;
    }

    void OnDisable()
    {
        inputHandler.OnInteractDown -= HandleInteractDown;
        inputHandler.OnInteractUp -= HandleInteractUp;
        inputHandler.OnEscPressed -= EscMenuHandler;
    }

    void Update()
    {
        if (Time.time - lastCheckedForPickups > checkPickupInterval)
        {
            CheckForNearbyPickups();
            lastCheckedForPickups = Time.time;

            if (nearbyPickup == null && nearbyInteractable == null)
            {
                if (inventoryManager.GetSelectedItem().item == null)
                {
                    statsUIManager.HideInteractPrompt();
                } else
                {
                    statsUIManager.ShowInteractPrompt("F to use " + inventoryManager.GetSelectedItem().item.itemName);
                }
            }
        }
    }
    void HandleInteractDown()
    {
        if (nearbyInteractable != null)
        {
            nearbyInteractable.Interact(gameObject);
            nearbyInteractable = null;
            statsUIManager.HideInteractPrompt();
        } else
        {
            inventoryManager.BeginInteract();
        }
    }

    void HandleInteractUp()
    {
        // Currently no action on interact up
    }

    void EscMenuHandler()
    {
        statsUIManager.ToggleEscMenu();
    }
    void CheckForNearbyPickups()
    {
        if (statsUIManager == null) {statsUIManager = statManager.GetStatsUIManager(); return;}
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);  
        
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out PickUpItem pickup))
            {
                // Wow, found a pickup!
                Debug.Log("Found a pickup: " + pickup.item.itemName);
                nearbyPickup = pickup;
                nearbyInteractable = pickup;
                statsUIManager.ShowInteractPrompt($"F to pickup {pickup.item.itemName}");
                return;
            } else if (collider.TryGetComponent(out Orb orb))
            {
                nearbyInteractable = orb;
                nearbyPickup = null;
                statsUIManager.ShowInteractPrompt($"F to collect {orb.customName}");
                return;
            }
        }

        nearbyPickup = null;
        nearbyInteractable = null;
    }
}