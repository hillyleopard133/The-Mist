using System.Collections;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCFollowPlayer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float followDistance = 1.2f;
    [SerializeField] private DialogueTrigger disappearAfterTrigger;

    [Header("Destination")] 
    [SerializeField] private string destinationScene;
    private bool isInDestinationScene;
    [SerializeField] private bool hasDestination;
    
    private float chaseSpeed;
    private bool isFollowing;
    private Player player;
    private Rigidbody2D rb;

    //Animation
    private Animator animator;
    private bool isSprinting;
    private readonly int moveX = Animator.StringToHash("MoveX");
    private readonly int moveY = Animator.StringToHash("MoveY");
    private readonly int moving = Animator.StringToHash("Moving");
    private readonly int sprinting = Animator.StringToHash("Sprinting");
    private readonly int dead = Animator.StringToHash("Dead");
    private readonly int revive = Animator.StringToHash("Revive");

    private string IS_FOLLOWING;

    private void Start()
    {
        IS_FOLLOWING = "IS_FOLLOWING" + gameObject.name;
        player = Player.Instance;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        LoadIsFollowing();
        if (isFollowing)
        {
            transform.position = player.transform.position;
        }
        if (transform.parent != NPCFollowerManager.Instance.transform)
        {
            if (DialogueManager.Instance.GetDialogueQuestManager().dialogueTriggers.Contains(disappearAfterTrigger))
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isFollowing)
        {
            if (transform.GetSiblingIndex() == 0)
            {
                FollowPlayer(player.gameObject);
            }
            else
            {
                FollowPlayer(NPCFollowerManager.Instance.transform.GetChild(transform.GetSiblingIndex() -1).gameObject);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero; 
        }
    }

    private void SaveIsFollowing()
    {
        SaveGame.Save(IS_FOLLOWING, isFollowing);
    }

    private void LoadIsFollowing()
    {
        if (SaveGame.Exists(IS_FOLLOWING))
        {
            isFollowing = SaveGame.Load<bool>(IS_FOLLOWING);
        }
    }

    public bool IsFollowing()
    {
        return isFollowing;
    }

    public void StartFollowing()
    {
        isFollowing = true;
        transform.SetParent(NPCFollowerManager.Instance.transform);
        SaveIsFollowing();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hasDestination)
        {
            if (scene.name == destinationScene)
            {
                isInDestinationScene = true;
                Debug.Log("Destination Scene Reached");
            }
            else
            {
                isInDestinationScene = false;
            }
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
        transform.SetParent(null);
        SaveIsFollowing();
        StartCoroutine(MoveToDestination());
    }

    private IEnumerator MoveToDestination(Vector2 destination)
    {
        float stopThreshold = 0.1f;
        while (Vector2.Distance(transform.position, destination) > stopThreshold)
        {
            Vector2 direction = (destination - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * chaseSpeed;
            yield return null;
        }
    
        rb.linearVelocity = Vector2.zero;
    }
    
    private IEnumerator MoveToDestination()
    {
        yield return null;
    }

    private void FollowPlayer(GameObject target)
    {
        Vector2 dirToPlayer = (target.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(target.transform.position, transform.position);
        
        if (player.GetComponent<PlayerMovement>().IsSprinting)
        {
            chaseSpeed = player.GetComponent<PlayerMovement>().SprintSpeed;
            isSprinting = true;
        }
        else
        {
            chaseSpeed = player.GetComponent<PlayerMovement>().WalkSpeed;
        }
        
        if (distanceToPlayer >= followDistance)
        {
            rb.linearVelocity = dirToPlayer * chaseSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        SetAnimations(dirToPlayer);
    }

    public void KillNPC()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(dead);
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void ReviveNPC()
    {
        animator.SetTrigger(revive);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void SetAnimations(Vector2 dir)
    {
        animator.SetFloat(moveX, dir.x);
        animator.SetFloat(moveY, dir.y);
        
        if (rb.linearVelocity == Vector2.zero)
        {
            animator.SetBool(moving, false);
        }
        else
        {
            animator.SetBool(moving, true);
            if (isSprinting)
            {
                animator.SetBool(sprinting, true);
            }
            else
            {
                animator.SetBool(sprinting, false);
            }
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}