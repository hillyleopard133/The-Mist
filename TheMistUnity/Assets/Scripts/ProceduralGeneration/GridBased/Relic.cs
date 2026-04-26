using UnityEngine;

public class Relic : MonoBehaviour
{
    private static readonly int ActivateS = Animator.StringToHash("Activate");
    
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
        actions.Temple.ActivateRelic.performed += ctx => Activate();
        
        animator = GetComponent<Animator>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Activate()
    {
        if(!interactionBox.activeSelf || PauseGameManager.Instance.isPaused) return;

        animator.SetTrigger(ActivateS);
        circleCollider2D.enabled = false;
        TempleManager.Instance.ActivateRelic();
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
            actions.Temple.ActivateRelic.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.Temple.ActivateRelic.Disable();
        }
    }
}
