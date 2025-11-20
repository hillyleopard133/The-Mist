using System.Collections;
using System.Collections.Generic;
//using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [FormerlySerializedAs("speed")]
    [Header("Config")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    
    public float WalkSpeed  => walkSpeed;
    public float SprintSpeed => sprintSpeed;

    public Vector2 MoveDirection => moveDirection;

   // private PlayerAnimations playerAnimations;
    private PlayerActions actions;
    //private Player player;
    private Rigidbody2D rb2D;
    private Vector2 moveDirection;

    [Header("Sprinting")]
    [SerializeField] private Toggle sprintToggle;
    private bool sprintToggleActive;
    private bool isSprinting;
    
    public bool IsSprinting => isSprinting;
    
    private readonly string TOGGLE_SPRINT = "TOGGLE_SPRINT";

    private void Awake()
    {
        //player = GetComponent<Player>();
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
        //playerAnimations = GetComponent<PlayerAnimations>();
    }
    
    private void Start()
    {
        actions.Movement.Sprint.performed += ctx => Sprint(true);  
        actions.Movement.Sprint.canceled += ctx => Sprint(false); 
        
        //bool toggleSprintIsOn = SaveGame.Exists(TOGGLE_SPRINT) ? SaveGame.Load<bool>(TOGGLE_SPRINT) : false;
        bool toggleSprintIsOn = false;
        sprintToggle.isOn = toggleSprintIsOn;
        ToggleSprint(toggleSprintIsOn);
        sprintToggle.onValueChanged.AddListener(ToggleSprint);
    }

    // Update is called once per frame
    void Update()
    {
        ReadMovement();
    }    
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Sprint(bool sprint)
    {
        if (sprintToggleActive)
        {
            if (!sprint) return;
            isSprinting = !isSprinting;
        }
        else
        {
            isSprinting = sprint;
        }
        //playerAnimations.SetSprintBoolAnimation(isSprinting);
    }

    public void ToggleSprint(bool toggle)
    {
        sprintToggleActive = toggle;
        //SaveGame.Save(TOGGLE_SPRINT, toggle);
    }

    private void Move()
    {
        //if(player.Stats.Health <= 0)
        //{
        //    return;
        //}
        float speed = walkSpeed;
        if(isSprinting) speed = sprintSpeed;
        rb2D.MovePosition(rb2D.position + moveDirection * (speed * Time.fixedDeltaTime));
    }

    private void ReadMovement()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
        if(moveDirection == Vector2.zero)
        {
            //playerAnimations.SetMoveBoolTransition(false);
            isSprinting = false;
            //playerAnimations.SetSprintBoolAnimation(isSprinting);
            return;
        }
        
        //playerAnimations.SetMoveBoolTransition(true);
        //playerAnimations.SetMoveAnimation(moveDirection);
        //playerAnimations.SetSprintBoolAnimation(isSprinting);
    }

    public void EnableMovement()
    {
        actions.Movement.Enable();
    }

    public void DisableMovement()
    {
        actions.Movement.Disable();
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
}
