using UnityEngine;

public enum EnemyMovementType
{
        Wander,
        Patrol
}
public class EnemyBrain : MonoBehaviour
{
        [Header("Config")]
        [SerializeField] public Sprite Icon;
        [SerializeField] public string Name;

        public EnemyMovementType movementType;
        [SerializeField] private string initialState;      //Patrol or wander
        [SerializeField] private FSMState[] states;
        
        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public EnemyAnimations animations;
        [HideInInspector] public int updateFrameNumber = 1;
        [HideInInspector] public bool isAlive = true;
        
        public FSMState CurrentState {get; set;}
        
        public Transform Player {get; set;}

        private void Awake()
        {
                rb = GetComponent<Rigidbody2D>();
                animations = GetComponent<EnemyAnimations>();
        }

        private void Start()
        {
                ChangeState(initialState);
        }

        private void Update()
        {
                //if (CurrentState == null)
                //{
                //        return;
                //}
                //CurrentState.UpdateState(this);
                //Alternative way to do the above (using a ?)
                CurrentState?.UpdateState(this);
        }

        public void ChangeState(string newStateID)
        {
                FSMState newState = GetState(newStateID);
                if (newState == null)
                {
                        return;
                }
                CurrentState = newState;
        }

        private FSMState GetState(string newStateID)
        {
                for (int i = 0; i < states.Length; i++)
                {
                        if (states[i].ID == newStateID)
                        {
                                return states[i];
                        }
                }
                return null;
        }
        
}
