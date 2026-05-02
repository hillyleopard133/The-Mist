using BayatGames.SaveGameFree;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private static readonly int OpenS = Animator.StringToHash("Open");
    
    [SerializeField] private ChestLoot chestLoot;
    
    [SerializeField] private GameObject interactionBox;
    
    private PlayerActions actions;
    private Animator animator;
    private CircleCollider2D circleCollider2D;

    [SerializeField] private string chestID;
    
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

        if (ChestManager.Instance.IsOpened(chestID))
        {
            OpenChest();
        }
    }

    private void Open()
    {
        if(!interactionBox.activeSelf || PauseGameManager.Instance.isPaused) return;

        OpenChest();
        GiveLoot();

        TempleChest chest = GetComponent<TempleChest>();
        if (chest != null)
        {
            chest.Open();
        }
        else
        {
            ChestManager.Instance.OpenChest(chestID);
        }
    }

    private void OpenChest()
    {
        animator.SetTrigger(OpenS);
        circleCollider2D.enabled = false;
        interactionBox.SetActive(false);
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
            actions.General.OpenChest.Enable();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(false);
            actions.General.OpenChest.Disable();
        }
    }
}
