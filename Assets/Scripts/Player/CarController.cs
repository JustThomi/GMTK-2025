using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Range(0, 6)] public int distanceOffset;
    [Range(0, 15)] public float steerForce;
    public float waypointCheckDistance;

    public float acceleration;
    public float turnSpeed;

    public Transform carModel;
    private Vector3 startModelOffset;

    public float groundCheckRate;
    private float lastGroundCheckTime;

    private float curYRot;
    public bool canControl;

    private bool accelerateInput;
    private float turnInput;

    public TrackZone curTrackZone;
    public int zonesPassed;
    public int racePosition;
    public int curLap;

    public Rigidbody rig;

    private int currentWaypointIndex = 0;

    private void Start()
    {
        startModelOffset = carModel.transform.localPosition;
        GameManager.instance.cars.Add(this);
        rig.position = GameManager.instance.spawnPoints[GameManager.instance.cars.Count - 1].position;

        waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
        nodes = waypoints.nodes;
    }

    private void Update()
    {
        if (!canControl)
            turnInput = 0.0f;

        CalculateDistanceOfWaypoints();

        switch (driverController)
        {
            case DriverType.AI:
                AISteer();
                break;
            case DriverType.Player:
                PlayerSteer();
                break;
        }

        carModel.position = transform.position + startModelOffset;

        CheckGround();
    }

    private void FixedUpdate()
    {
        if(!canControl)
            return;
        
        if (accelerateInput == true)
        
       CalculateDistanceOfWaypoints();

        switch (driverController)
        {
            case DriverType.AI:
                AIDrive();
                break;
            case DriverType.Player:
                PlayerDrive();
                break;
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, -0.75f, 0), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            carModel.up = hit.normal;
        }
        else
        {
            carModel.up = Vector3.up;
        }

        carModel.Rotate(new Vector3(0, curYRot, 0), Space.Self);
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

    private void AIDrive()
    {
        rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
    }

    private void AISteer()
    {
        Vector3 localTarget = transform.InverseTransformPoint(currentWaypoint.position);

        localTarget /= localTarget.magnitude;

        float desiredTurn = localTarget.x;

        turnInput = Mathf.Lerp(turnInput, desiredTurn, Time.deltaTime * 5f);

        float turnRate = Mathf.Abs(Vector3.Dot(rig.linearVelocity.normalized, carModel.forward));

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    }

    private void PlayerDrive()
    {
        if (accelerateInput == true)
        {
            rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
        }
    }

    private void PlayerSteer()
    {
        float turnRate = Vector3.Dot(rig.linearVelocity.normalized, carModel.forward);

        turnRate = Mathf.Abs(turnRate);

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    }

    private void CalculateDistanceOfWaypoints()
    {
        if (nodes.Count == 0) return;

        float distanceToWaypoint = Vector3.Distance(transform.position, nodes[currentWaypointIndex].position);
        if (distanceToWaypoint < waypointCheckDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % nodes.Count;
        }

        int targetIndex = (currentWaypointIndex + distanceOffset) % nodes.Count;
        currentWaypoint = nodes[targetIndex];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentWaypoint.position, 3f);
    }
}