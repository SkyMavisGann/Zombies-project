using UnityEngine;
using UnityEngine.AI;

public class BackgroundNpc : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public float animationSpeedMultiplier = 0.25f;
    public Transform ChaseTarget = null;
    //amount of time an npc talks to another
    public int npcTalkTime = 60;
    //amount of time before that npc can talk again in seconds
    public int TalkBreakTime = 30;


    //which state it was at before it starting talking
    private BrainStates StateBeforeTalking;

    [Tooltip("The distance where if an npc happens to lock onto an npc " +
        "without stopping it will break the connection past this distance.")]
    public float maxTalkingDistance = 0.2f;

    public float ChasingRange = 3;
    public float AttackRange = 1;
    private Transform player;

    [HideInInspector] public bool IsOnStairs = false;

    //states of what its possible to be doing
    public enum BrainStates{
        WANDERING,
        STANDING,
        CHASING,
        ATTACKING,
        NONE,
    }

    public Transform wanderRegion;
    public BrainStates brainState;
    private BrainStates previousBrainState = BrainStates.NONE;

    [Tooltip("The amount of time in SECONDS, before changing destination, while Wandering")]
    public int ChangeDestMax = 5;
    public int ChangeDestMin = 1;
    private int currentWanderSwitch = 0;
    private int destinationTimer = 0;
    private Vector3 RandomWanderPos;
    private Vector3 prevRandomWanderPos;

    private bool isReabling = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        
        //this way the npc can stand off navmesh surface
        if (brainState == BrainStates.STANDING)
        {
            agent.enabled = false;
        }


        //npc prefabs cannot have a reference to their intial wander region
        //find it anyway if its just a prefab
        if (wanderRegion == null)
        {
            wanderRegion = GameObject.Find("ParkingLot").transform;
        }
    }
    private void FixedUpdate()
    {
        //each period for the switch choose a new
        //destination and random time
        destinationTimer++;
        if (brainState != BrainStates.STANDING)
        {
            if (destinationTimer >= currentWanderSwitch)
            {
                chooseNewWanderTarget();
            }
        }
       

    }
    public void BeginChasing(Collider other)
    {
        
    }
    private void chooseNewWanderTarget()
    {
        //using the scale here for radius, that way objects can be set up to be visually nice
        RandomWanderPos = RandomNavSphere(wanderRegion.position, wanderRegion.lossyScale.x, -1);
        currentWanderSwitch = Random.Range(ChangeDestMin * 60, ChangeDestMax * 60);
        destinationTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        
        ConstantBehavior();
        if (previousBrainState != brainState)
        {
            changeState();
            previousBrainState = brainState;
        }
    }

    private void changeState()
    {
        
        switch (brainState)
        {
            case BrainStates.STANDING:
                animator.SetTrigger("Dead");
                agent.updateRotation = true;
                break;
            case BrainStates.WANDERING:
                agent.enabled = true;
                agent.updateRotation = true;
                break;
            case BrainStates.CHASING:

                agent.enabled = true;
                agent.updateRotation = true;
                //stop going to current destination immediately
                
                agent.ResetPath();
                Vector3 direction = ChaseTarget.position - transform.position;
                //this calculations is as follows
                // your position + the difference in yalls position is the halfwaypoint between
                //THIS DETERMINES HOW FAR APART THEY ARE WHEN THEY ARE TALKING

                //agent.updateRotation = false;

                agent.isStopped = false;
                transform.LookAt(ChaseTarget.position);

                break;
            case BrainStates.ATTACKING:
                if (agent.isActiveAndEnabled)
                {
                    //stop going to current destination immediately
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.updateRotation = false;

                    agent.isStopped = false;
                }
                transform.LookAt(ChaseTarget.position);
                break;
        }
    }
    private void ConstantBehavior()
    {
        switch (brainState)
        {
            case BrainStates.STANDING:
                agent.enabled = false;
                break;
            case BrainStates.WANDERING:
                animator.speed = agent.speed * animationSpeedMultiplier;
                //update everytime it changes, but not constantly
                if (prevRandomWanderPos != RandomWanderPos)
                {
                    beginWalkingTo(RandomWanderPos);
                    
                    prevRandomWanderPos = RandomWanderPos;
                }
                
                WalkWhenMoving();


                if (player != null && Vector3.Distance(player.position, transform.position) < ChasingRange)
                {
                    ChaseTarget = player;
                    brainState = BrainStates.CHASING;
                }
                break;
            case BrainStates.CHASING:

                //transform.LookAt(ChaseTarget.position);
                animator.speed = agent.speed * animationSpeedMultiplier;
                WalkWhenMoving();
                beginWalkingTo(RandomNavSphere(player.position, 1, -1));
                //if the person you're talking to is really far away then give up
                if (ChaseTarget != null && Vector3.Distance(ChaseTarget.position, transform.position) > ChasingRange)
                {
                    brainState = BrainStates.WANDERING;
                    ChaseTarget = null;
                    if (agent.isActiveAndEnabled)
                        beginWalkingTo(RandomWanderPos);
                }


                if (Vector3.Distance(ChaseTarget.position, transform.position) < AttackRange)
                {
                    brainState = BrainStates.ATTACKING;
                }
                break;
                case BrainStates.ATTACKING:

                if (Vector3.Distance(ChaseTarget.position, transform.position) > AttackRange)
                {
                    brainState = BrainStates.CHASING;
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("zombie_attack"))
                {
                    animator.SetTrigger("Attack");
                    //attack
                }
                break;
        }
    }
    private void WalkWhenMoving() 
    {
        animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    //after blipping from disableComponentOnNpcMoving it needs to start walking again
    private void OnEnable()
    {
        if (isReabling)
        {
            if (brainState == BrainStates.WANDERING && RandomWanderPos != null)
            {
                beginWalkingTo(RandomWanderPos);
            }
        }
        
    }
    private void OnDisable()
    {
        isReabling = true;
    }

    //this is here because in the past I needed to do multiple things
    //to begin walking, now its only this one function, left it
    //incase I needed to add more
    private void beginWalkingTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh)
            agent.SetDestination(pos);
    }
    
}
