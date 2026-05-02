using UnityEngine;

public class Burnable : MonoBehaviour
{
    [SerializeField] private GameObject interactionBox;
    [SerializeField] private Dialogue dialogue;
    
    private PlayerActions actions;
    private TempleManager templeManager;
    private DialogueManager dialogueManager;
    
    private void Awake()
    {
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }

    private void Start()
    {
        templeManager = TempleManager.Instance;
        
        actions.General.UsePower.performed += ctx => Burn();
    }

    private void Burn()
    {
        if (templeManager.IsTempleCleared(Temples.Fire))
        {
            actions.General.UsePower.Disable();
            //TODO burning shader
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            actions.General.UsePower.Enable();
            interactionBox.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            actions.General.UsePower.Disable();
            interactionBox.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (actions != null)
        {
            actions.General.UsePower.Enable();
        }
    }
}
