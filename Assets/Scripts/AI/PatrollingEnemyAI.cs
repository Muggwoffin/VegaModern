using UnityEngine;
using UnityEngine.AI;

public class PatrollingEnemyAI : MonoBehaviour
{   
    [Header("Behaviour Mode")]
    [SerializeField] private BehaviourMode behaviourMode = BehaviourMode.Avoid;
    public enum BehaviourMode { Chase, Avoid }
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip avoidanceClip;
    
    [Header("Patrol Settings")] 
    [SerializeField] private Transform[] waypoints; 
    [SerializeField] private float waypointWaitTime = 2f; 
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private float hearingRange = 5f;
     
    [Header("Chase Settings")] 
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float chaseTimeout = 2f;
    [SerializeField] private LayerMask detectionMask;
    
    [Header("Avoidance Settings")]
    [Tooltip("How close the player must get to trigger avoidance.")]
    [SerializeField] private float avoidDistance = 3f;
    [Tooltip("How far the ghost steps away from the player.")]
    [SerializeField] private float avoidRadius = 5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color chaseColor = Color.red;
    
    [Header("Patrol Mode")]
    [Tooltip("Waypoint is a fixed route. Area is a random wandering inside a zone")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.Waypoint;
    
    [Header("Area Patrol Settings")] 
    [Tooltip("Centre of the wander zone.")] 
    [SerializeField] private Transform areaCenter; 
    [Tooltip("Radius of the wander zone.")] 
    [SerializeField] private float areaRadius = 10f; 
    [Tooltip("Seconds to wait at each random point.")] 
    [SerializeField] private float areaWaitTime = 2f; 
    
    private Renderer[] enemyRenderers;
    private Material[][] originalMaterials;
     
    private NavMeshAgent agent; 
    private Transform player; 
     
    private int currentWaypointIndex = 0; 
    private float waitTimer = 0f; 
    private float chaseTimer = 0f; 
    
    public enum PatrolMode { Waypoint, Area }
     
    private enum State { Patrolling, Waiting, Chasing, Returning, Avoiding } 
    private State currentState = State.Patrolling; 
 
    void Start() 
    { 
        agent = GetComponent<NavMeshAgent>(); 
        player = GameObject.FindGameObjectWithTag("Player").transform; 
         
        if (waypoints.Length > 0) 
        { 
            agent.SetDestination(waypoints[currentWaypointIndex].position); 
        } 
        else 
        { 
            Debug.LogError("No waypoints assigned to " + gameObject.name); 
        } 
        enemyRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[enemyRenderers.Length][];
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            originalMaterials[i] = enemyRenderers[i].sharedMaterials;
        }
    } 
 
    void Update() 
    { 
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        float distanceToPlayer = Vector3.Distance(transform.position, playerCenter); 
         
        switch (currentState) 
        { 
            case State.Patrolling:
                Patrol();
                if (behaviourMode == BehaviourMode.Chase && (CanSeePlayer() || CanHearPlayer()))
                    StartChasing();
                else if (behaviourMode == BehaviourMode.Avoid && distanceToPlayer <= avoidDistance)
                    StartAvoiding();
                break;
                 
            case State.Waiting: 
                Wait(); 
                if (behaviourMode == BehaviourMode.Chase && (CanSeePlayer() || CanHearPlayer()))
                    StartChasing();
                else if (behaviourMode == BehaviourMode.Avoid && distanceToPlayer <= avoidDistance)
                    StartAvoiding();
                break; 
                 
            case State.Chasing: 
                Chase(distanceToPlayer); 
                break; 
            
            case State.Avoiding:
                // Once the ghost has reached its avoid destination, return to patrol
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    StartReturning();
                break;

            case State.Returning:
                ReturnToPatrol();
                break;
            
        } 
    } 
 
    void Patrol() 
    { 
        // Check if we've reached the current waypoint 
        if (!agent.pathPending && agent.remainingDistance <= 
            agent.stoppingDistance) 
        { 
            currentState = State.Waiting; 
            waitTimer = 0f; 
        } 
    }

    void StartAvoiding()
    {
        Vector3 awayFromPlayer = (transform.position - player.position).normalized;
        Vector3 targetPoint = transform.position + awayFromPlayer * avoidRadius;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPoint, out hit, avoidRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            currentState = State.Avoiding;
        }
        
        if (audioSource != null && avoidanceClip != null)
            audioSource.PlayOneShot(avoidanceClip);
    }

    void StartReturning()
    {
        currentState = State.Returning;
        ResetMaterialColors();
    }
 
    void Wait() 
    { 
        waitTimer += Time.deltaTime; 
        
        float waitDuration = (patrolMode == PatrolMode.Waypoint) ? waypointWaitTime : areaWaitTime;
         
        if (waitTimer >= waitDuration) 
        { 
            if (patrolMode == PatrolMode.Waypoint) 
            { 
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; 
                agent.SetDestination(waypoints[currentWaypointIndex].position); 
            } 
            else 
            { 
                MoveToRandomAreaPoint(); 
            } 
            currentState = State.Patrolling; 
        } 
    } 
 
    void StartChasing() 
    { 
        currentState = State.Chasing; 
        chaseTimer = 0f; 
        
        //Set materials to chase colour
        SetMaterialColors(chaseColor);
    } 
 
    void Chase(float distanceToPlayer) 
    { 
        // Move towards player 
        agent.SetDestination(player.position); 
         
        // Check if player is still in range 
        if ( CanSeePlayer() || CanHearPlayer()) 
        { 
            // Player is still close, reset timer 
            chaseTimer = 0f; 
        } 
        else 
        { 
            // Player is out of range, increment timer 
            chaseTimer += Time.deltaTime; 
             
            if (chaseTimer >= chaseTimeout) 
            { 
                // Give up and return to patrol 
                currentState = State.Returning; 
            } 
        } 
    }


    void ReturnToPatrol() 
    { 
        // Find the nearest waypoint 
        if (patrolMode == PatrolMode.Waypoint)
        {
            int nearestWaypointIndex = FindNearestWaypoint();
            currentWaypointIndex = nearestWaypointIndex;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else
        {
            MoveToRandomAreaPoint();
        }

        // Once we reach the waypoint, resume patrolling 
        if (!agent.pathPending && agent.remainingDistance <= 
            agent.stoppingDistance) 
        { 
            currentState = State.Waiting; 
            waitTimer = 0f; 
        } 
    }
    int FindNearestWaypoint() 
    { 
        int nearestIndex = 0; 
        float nearestDistance = Vector3.Distance(transform.position, 
            waypoints[0].position); 
         
        for (int i = 1; i < waypoints.Length; i++) 
        { 
            float distance = Vector3.Distance(transform.position, 
                waypoints[i].position); 
            if (distance < nearestDistance) 
            { 
                nearestDistance = distance; 
                nearestIndex = i; 
            } 
        } 
         
        return nearestIndex; 
    }

    void MoveToRandomAreaPoint()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomPoint = areaCenter.position + Random.insideUnitSphere * areaRadius;
            randomPoint.y = areaCenter.position.y;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, areaRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
        agent.SetDestination(areaCenter.position);
        Debug.Log("No Nav Mesh point selected so moving to centre");
    }
    
// Visualize detection range in Scene view
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        if (behaviourMode == BehaviourMode.Chase)
        {
            // Detection range sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, detectionRange);

            // FOV cone lines
            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView * 0.5f, 0)
                                   * transform.forward * detectionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView * 0.5f, 0)
                                    * transform.forward * detectionRange;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + leftBoundary);
            Gizmos.DrawLine(origin, origin + rightBoundary);

            // Arc across the front of the cone
            int arcSegments = 20;
            float angleStep = fieldOfView / arcSegments;
            float startAngle = -fieldOfView * 0.5f;

            for (int i = 0; i < arcSegments; i++)
            {
                Vector3 from = origin + (Quaternion.Euler(0, startAngle + angleStep * i, 0)
                                         * transform.forward * detectionRange);
                Vector3 to = origin + (Quaternion.Euler(0, startAngle + angleStep * (i + 1), 0)
                                       * transform.forward * detectionRange);
                Gizmos.DrawLine(from, to);
            }

            //hearing range
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, hearingRange);
            // Line to player — green if visible, red if not
            if (player != null)
            {
                Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
                Gizmos.DrawLine(origin, player.position + Vector3.up * 1f);
            }
        }
        else
        {
            // Avoidance trigger range
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, avoidDistance);

            // Step-away radius
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, avoidRadius);
        }

        // View area of Area Patrol mode
        if (patrolMode == PatrolMode.Area && areaCenter != null) 
        { 
            Gizmos.color = new Color(0f, 0.8f, 1f, 0.25f); // translucent fill 
            Gizmos.DrawSphere(areaCenter.position, areaRadius); 
            Gizmos.color = new Color(0f, 0.8f, 1f, 1f);   // solid outline 
            Gizmos.DrawWireSphere(areaCenter.position, areaRadius); 
        }
    }


    void SetMaterialColors(Color color)
    {
        foreach (Renderer renderer in enemyRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.color = color;
            }
        }
    }

    bool CanSeePlayer() 
    { 
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerCenter - origin).normalized;

        // Check angle first - is player within FOV cone?
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView * 0.5f) return false;

        RaycastHit hit; 
        if (Physics.Raycast(origin, directionToPlayer, out hit, detectionRange, detectionMask)) 
        { 
            return hit.transform.CompareTag("Player"); 
        } 
        return false; 
    }

    bool CanHearPlayer()
    {
        return Vector3.Distance(transform.position, player.position) <= hearingRange;
    }
    
    void ResetMaterialColors()
    {
        for (int i = 0; i < enemyRenderers.Length; i++) 
        { 
            enemyRenderers[i].materials = originalMaterials[i]; 
        } 
            
    }
}
