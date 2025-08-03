using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class CarController : MonoBehaviour
{
    internal enum DriverType
    {
        AI,
        Player
    }

    [SerializeField]
    private DriverType driverController;
    [Header("AI Drive")]
    public TrackWaypoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    public Transform currentWaypoint;
    [Range(0, 6)] private int distanceOffset;
    [Range(0, 15)] public float steerForce;
    public float waypointCheckDistance;

    public float acceleration;
    public float reverse;
    public float turnSpeed;

    public Transform carModel;
    private Vector3 startModelOffset;

    public float groundCheckRate;
    private float lastGroundCheckTime;

    private float curYRot;
    public bool canControl;

    private bool accelerateInput;
    private bool reverseInput;
    private float turnInput;

    public TrackZone[] trackZones;
    public TrackZone curTrackZone;
    public int zonesPassed;
    public int racePosition;
    public int curLap;

    public Rigidbody rig;
    private NavMeshAgent agent;

    public List<Transform> raceCheckpoints = new List<Transform>();

    private int currentTargetIndex = 0;

    private void Start()
    {
        startModelOffset = carModel.transform.localPosition;
        GameManager.instance.cars.Add(this);
        rig.position = GameManager.instance.spawnPoints[GameManager.instance.cars.Count - 1].position;
        
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
        nodes = waypoints.nodes;

        GameObject parentObject = GameObject.FindGameObjectWithTag("TrackZone");
        trackZones = parentObject.GetComponentsInChildren<TrackZone>()
            .OrderBy(tz => tz.transform.GetSiblingIndex())
            .ToArray();

        if (driverController == DriverType.AI && nodes.Count > 0)
        {
            currentTargetIndex = 0;
            agent.SetDestination(nodes[currentTargetIndex].position);
        }
    }

    private void Update()
    {
        if (agent != null)
            agent.enabled = canControl;

        if (!canControl)
            turnInput = 0.0f;

        switch (driverController)
        {
            case DriverType.AI:
                // AISteer();
                break;
            case DriverType.Player:
                PlayerSteer();
                break;
        }

        CheckGround();
    }
    private void FixedUpdate()
    {
        if (!canControl)
            return;

        if (driverController == DriverType.AI)
        {
            UpdateDestinationIfNeeded();

            Vector3 desiredVelocity = agent.desiredVelocity;

            desiredVelocity.y = rig.linearVelocity.y;

            rig.linearVelocity = Vector3.Lerp(rig.linearVelocity, desiredVelocity, Time.fixedDeltaTime * 5f);

            Vector3 horizontalVelocity = rig.linearVelocity;
            horizontalVelocity.y = 0;

            if (horizontalVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized);
                rig.MoveRotation(Quaternion.Slerp(rig.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
            }

            agent.nextPosition = rig.position;
        }
        else if (driverController == DriverType.Player)
        {
            PlayerDrive();
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            carModel.up = Vector3.Lerp(carModel.up, hit.normal, 10f * Time.deltaTime);
        }
        else
        {
            carModel.up = Vector3.Lerp(carModel.up, Vector3.up, 10f * Time.deltaTime);
        }

        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(rig.transform.forward, carModel.up).normalized;
        if (forwardOnPlane.sqrMagnitude > 0.001f)
        {
            carModel.rotation = Quaternion.LookRotation(forwardOnPlane, carModel.up);
        }
    }

    // called when we press down the accelerate input
    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            accelerateInput = true;
        else
            accelerateInput = false;
    }

    // called when we modify the turn input
    public void OnTurnInput(InputAction.CallbackContext context)
    {
        turnInput = context.ReadValue<float>();
    }

    public void OnReverseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            reverseInput = true;
        else
            reverseInput = false;
    }

    //private void AIDrive()
    //{
    //    rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
    //    UpdateDestinationIfNeeded();
    //}

    //private void AISteer()
    //{
    //    if (agent.path == null || agent.path.corners.Length < 2) return;

    //    Vector3 nextCorner = agent.path.corners[1];
    //    Vector3 localTarget = transform.InverseTransformPoint(nextCorner);

    //    if (localTarget.z < 0)
    //    {
    //        turnInput = 0f;
    //        return;
    //    }

    //    localTarget.Normalize();

    //    float desiredTurn = localTarget.x;
    //    if (Mathf.Abs(desiredTurn) < 0.07f)
    //        desiredTurn = 0f;

    //    turnInput = Mathf.MoveTowards(turnInput, desiredTurn, Time.deltaTime * steerForce);

    //    float turnRate = Mathf.Abs(Vector3.Dot(rig.linearVelocity.normalized, carModel.forward));
    //    curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    //}

    private void UpdateDestinationIfNeeded()
    {
        if (nodes.Count == 0) return;

        float dist = Vector3.Distance(transform.position, agent.destination);

        if (dist < waypointCheckDistance)
        {
            currentTargetIndex = (currentTargetIndex + 1) % nodes.Count;
            agent.SetDestination(nodes[currentTargetIndex].position);
        }

        currentWaypoint = nodes[currentTargetIndex];
    }

    private void PlayerDrive()
    {
        if (accelerateInput == true)
        {
            rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
        }
        else if(reverseInput == true)
        {
            rig.AddForce(carModel.forward * -reverse, ForceMode.Acceleration);
        }
    }

    private void PlayerSteer()
    {
        float turnRate = Vector3.Dot(rig.linearVelocity.normalized, carModel.forward);

        turnRate = Mathf.Abs(turnRate);

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    }

    //private void CalculateDistanceOfWaypoints()
    //{
    //    if (nodes.Count == 0) return;

    //    float distanceToWaypoint = Vector3.Distance(transform.position, nodes[currentWaypointIndex].position);
    //    if (distanceToWaypoint < waypointCheckDistance)
    //    {
    //        currentWaypointIndex = (currentWaypointIndex + 1) % nodes.Count;
    //    }

    //    int targetIndex = (currentWaypointIndex + distanceOffset) % nodes.Count;
    //    currentWaypoint = nodes[targetIndex];
    //}

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawWireSphere(currentWaypoint.position, 3f);
    // }
}