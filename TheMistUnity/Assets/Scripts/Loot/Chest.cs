using UnityEngine;

public class Chest : MonoBehaviour
{
    private static readonly int OpenS = Animator.StringToHash("Open");
    
    [SerializeField] private ChestLoot chestLoot;
    
    [SerializeField] private GameObject interactionBox;
    
    private PlayerActions actions;
    private Animator animator;
    private CircleCollider2D circleCollider2D;
    
    private void Awake()
    {
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }

    private void Start()
    {
        actions.General.OpenChest.performed += ctx => Open();
        
        animator = GetComponent<Animator>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Open()
    {
        if(!interactionBox.activeSelf || PauseGameManager.Instance.isPaused) return;

        animator.SetTrigger(OpenS);
        circleCollider2D.enabled = false;
        interactionBox.SetActive(false);
        GiveLoot();
    }

    private void GiveLoot()
    {
        CoinManager.Instance.AddCoins(chestLoot.GetCoins());
        foreach (InventoryItem item in chestLoot.GetLoot())
        {
            Inventory.Instance.AddItem(item, 1);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (actions != null)
        {
            actions.General.OpenChest.Enable();
        }
    }
}
