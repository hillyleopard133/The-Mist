using UnityEngine;

public class RegenPoint : MonoBehaviour
{
    [SerializeField] private GameObject interactionBox;
    
    private CombatManager combatManager;
    private SaveLoadManager saveLoadManager;
    
    private PlayerActions actions;

    private void Awake()
    {
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }

    private void Start()
    {
        actions.General.RegenPoint.performed += ctx => Regen();
        
        combatManager = CombatManager.Instance;
    }

    private void Regen()
    {
        if(!interactionBox.activeSelf || PauseGameManager.Instance.isPaused) return;

        combatManager.FullRecovery();
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
            actions.General.RegenPoint.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.General.RegenPoint.Disable();
        }
    }
}
